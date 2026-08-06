using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlubLib;
using BlubLib.DotNetty.Handlers.MessageHandling;
using BlubLib.IO;
using Dapper.FastCrud;
using ExpressMapper.Extensions;
using NeoNetsphere.Database.Game;
using NeoNetsphere.Network.Data.Game;
using NeoNetsphere.Network.Message.Game;
using NeoNetsphere.Resource;
using ProudNetSrc;
using ProudNetSrc.Handlers;
using Serilog;
using Serilog.Core;

namespace NeoNetsphere.Network.Services
{
  internal class ShopService : ProudMessageHandler
  {
    // ReSharper disable once InconsistentNaming
    private static readonly ILogger Logger =
        Log.ForContext(Constants.SourceContextPropertyName, nameof(ShopService));

    public static async Task ShopUpdateMsg(ProudSession session = (ProudSession)null, bool broadcast = false)
    {
      if (session == null && broadcast == false)
        return;

      var targets = new List<ProudSession>();
      if (broadcast)
      {
        foreach (var sessionsValue in GameServer.Instance.Sessions.Values)
        {
          targets.Add(sessionsValue);
        }
      }
      else
      {
        targets.Add(session);
      }

      var shop = GameServer.Instance.ResourceCache.GetShop();
      var version = shop.Version;

      foreach (var proudSession in targets)
      {
        await proudSession.SendAsync(
            new NewShopUpdateCheckAckMessage
            {
              Date01 = version,
              Date02 = version,
              Date03 = version,
              Date04 = version,
              Unk = 1
            });

        await proudSession.SendAsync(
            new NewShopUpdataInfoAckMessage
            {
              Type = ShopResourceType.NewShopPrice,
              Data = shop.ShopPrices,
              Date = version
            }, SendOptions.ReliableSecureCompress);

        await proudSession.SendAsync(
            new NewShopUpdataInfoAckMessage
            {
              Type = ShopResourceType.NewShopEffect,
              Data = shop.ShopEffects,
              Date = version
            }, SendOptions.ReliableSecureCompress);

        await proudSession.SendAsync(
            new NewShopUpdataInfoAckMessage
            {
              Type = ShopResourceType.NewShopItem,
              Data = shop.ShopItems,
              Date = version
            }, SendOptions.ReliableSecureCompress);

        // ToDo
        await proudSession.SendAsync(
            new NewShopUpdataInfoAckMessage
            {
              Type = ShopResourceType.NewShopUniqueItem,
              Data = Array.Empty<byte>(),
              Date = version
            }, SendOptions.ReliableSecureCompress);

        // Unused in official
        // await proudSession.SendAsync(new NewShopUpdateEndAckMessage());
      }
    }

    [MessageHandler(typeof(NewShopUpdateCheckReqMessage))]
    public async Task ShopUpdateCheckHandler(GameSession session, NewShopUpdateCheckReqMessage message)
    {
      var shop = GameServer.Instance.ResourceCache.GetShop();
      var version = shop.Version;

      if (message.Date01 == version &&
          message.Date02 == version &&
          message.Date03 == version &&
          message.Date04 == version)
      {
        await session.SendAsync(new NewShopUpdateCheckAckMessage
        {
          Date01 = version,
          Date02 = version,
          Date03 = version,
          Date04 = version,
          Unk = 0
        });
        return;
      }

      if (session.Player != null)
        ShopUpdateMsg(session, false);
      else
        session.UpdateShop = true;
    }

    [MessageHandler(typeof(RandomShopUpdateCheckReqMessage))]
    public async Task RandomShopUpdateCheckHandler(GameSession session, RandomShopUpdateCheckReqMessage message)
    {
      var version = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");

      await session.SendAsync(new RandomShopUpdateRequestAckMessage());
      await session.SendAsync(new RandomShopUpdateCheckAckMessage(version));

      var pools = GameServer.Instance.ResourceCache.GetRandomShop();

      var allItems = pools.SelectMany(p => p.Items).ToList();

      Logger.ForAccount(session)
          .Information("RandomShop: pools={Pools} items={Items}", pools.Count, allItems.Count);

      var dto = new RandomShopDto
      {
        Items = allItems.Select(i => new RandomShopItemDto
        {
          Unk1 = (uint)i.ItemNumber,
          Unk2 = (int)i.Effect,
          Unk3 = 0,
          Unk4 = i.Color,
          Unk5 = (int)i.ItemNumber.Id,
          Unk6 = 0,
          Unk7 = i.Rate
        }).ToArray()
      };

      byte[] data;
      using (var w = new BinaryWriter(new MemoryStream()))
      {
        BlubLib.Serialization.Serializer.Serialize(w, dto);
        data = w.ToArray();
      }

      await session.SendAsync(
          new RandomShopUpdateInfoAckMessage(
              (byte)RandomShopResourceType.EUNewRandomShop,
              data,
              data.Length,
              0,
              version),
          SendOptions.ReliableSecureCompress);
    }

    [MessageHandler(typeof(RandomShopRollingStartReqMessage))]
    public async Task RandomShopRollingStartHandler(GameSession session, RandomShopRollingStartReqMessage message)
    {
      try
      {
        var plr = session.Player;
        var pools = GameServer.Instance.ResourceCache.GetRandomShop();

        if (pools.Count == 0)
        {
          await session.SendAsync(new RandomShopRollingStartAckMessage { Unk1 = 1, Unk2 = Array.Empty<RandomShopItemDto>() });
          return;
        }

        var poolIndex = message.Unk % pools.Count;
        var pool = pools[poolIndex];

        if (pool.Items.Count == 0)
        {
          await session.SendAsync(new RandomShopRollingStartAckMessage { Unk1 = 1, Unk2 = Array.Empty<RandomShopItemDto>() });
          return;
        }

        if (plr.PEN < pool.Price && pool.PriceType == ItemPriceType.PEN ||
            plr.AP < pool.Price && pool.PriceType == ItemPriceType.AP)
        {
          await session.SendAsync(new RandomShopRollingStartAckMessage { Unk1 = 1, Unk2 = Array.Empty<RandomShopItemDto>() });
          return;
        }

        var rolledItem = pool.Roll();
        var periodTier = pool.RollPeriod();

        // Package id is only used for client display; grant the real reward item
        var rewardNumber = rolledItem.RewardNumber != 0 ? rolledItem.RewardNumber : rolledItem.ItemNumber;

        var priceInfo = GameServer.Instance.ResourceCache.GetShop().GetFirstItemInfo(rewardNumber);
        if (priceInfo == null)
        {
          Logger.ForAccount(session).Error("RandomShop: No shop entry for {item}", rewardNumber);
          await session.SendAsync(new RandomShopRollingStartAckMessage { Unk1 = 1, Unk2 = Array.Empty<RandomShopItemDto>() });
          return;
        }

        var price = priceInfo.PriceGroup.GetPrice(periodTier.PeriodType, periodTier.Period);
        if (price == null)
        {
          Logger.ForAccount(session).Error("RandomShop: No price for {item} periodType={pt} period={p}",
              rewardNumber, periodTier.PeriodType, periodTier.Period);
          await session.SendAsync(new RandomShopRollingStartAckMessage { Unk1 = 1, Unk2 = Array.Empty<RandomShopItemDto>() });
          return;
        }

        switch (pool.PriceType)
        {
          case ItemPriceType.PEN:
            plr.PEN -= pool.Price;
            break;
          case ItemPriceType.AP:
            plr.AP -= pool.Price;
            break;
        }

        var itemEffects = new List<EffectNumber> { (EffectNumber)rolledItem.Effect };
        var plrItem = plr.Inventory.Create(priceInfo, price, rolledItem.Color,
            itemEffects.ToArray(), 0);

        await session.SendAsync(new RandomShopRollingStartAckMessage
        {
          Unk1 = 0,
          Unk2 = new[]
          {
            new RandomShopItemDto
            {
              Unk1 = (uint)rolledItem.ItemNumber,
              Unk2 = (int)rolledItem.Effect,
              Unk3 = 0,
              Unk4 = rolledItem.Color,
              Unk5 = (int)rolledItem.ItemNumber.Id,
              Unk6 = 0,
              Unk7 = 0
            }
          }
        });
        await session.SendAsync(new MoneyRefreshCashInfoAckMessage(plr.PEN, plr.AP));

        Logger.ForAccount(session).Information("RandomShop: Rolled {reward} (pkg {pkg}) from pool {pool} period={pt}/{p}",
            rewardNumber, rolledItem.ItemNumber, pool.Id, periodTier.PeriodType, periodTier.Period);
      }
      catch (Exception ex)
      {
        Logger.Error(ex, "RandomShop: Error in RollingStart");
        await session.SendAsync(new RandomShopRollingStartAckMessage { Unk1 = 1, Unk2 = Array.Empty<RandomShopItemDto>() });
      }
    }

    [MessageHandler(typeof(CollectBookItemRegistReqMessage))]
    public void CollectBookItemRegistReq(GameSession session, CollectBookItemRegistReqMessage message)
    {
      // Todo
    }

    [MessageHandler(typeof(CardGambleReqMessage))]
    public async Task CardGambleHandler(GameSession session, CardGambleReqMessage message)
    {
      try
      {
        var plr = session.Player;
        var cardSystem = GameServer.Instance.ResourceCache.GetCardSystem();

        if (!cardSystem.Active)
        {
          await session.SendAsync(new CardGambleAckMessage { Unk1 = 1 });
          return;
        }

        var season = cardSystem.Seasons.FirstOrDefault(s => s.Num == cardSystem.CurrentSeason);
        if (season == null || season.Cards.Count == 0)
        {
          await session.SendAsync(new CardGambleAckMessage { Unk1 = 1 });
          return;
        }

        // Deduct gamble PEN cost if configured
        if (cardSystem.Formula.GamblePen > 0)
        {
          if (plr.PEN < (uint)cardSystem.Formula.GamblePen)
          {
            await session.SendAsync(new CardGambleAckMessage { Unk1 = 1 });
            return;
          }
          plr.PEN -= (uint)cardSystem.Formula.GamblePen;
        }

        // Pick random card based on try_prob weights
        var totalTryProb = season.Cards.Sum(c => c.TryProb);
        if (totalTryProb <= 0)
        {
          await session.SendAsync(new CardGambleAckMessage { Unk1 = 1 });
          return;
        }

        var roll = new Random().Next(0, totalTryProb);
        CardEntry selectedCard = null;
        var cumulative = 0;
        foreach (var card in season.Cards)
        {
          cumulative += card.TryProb;
          if (roll < cumulative)
          {
            selectedCard = card;
            break;
          }
        }
        selectedCard = selectedCard ?? season.Cards[season.Cards.Count - 1];

        // Create the card item in inventory
        var shop = GameServer.Instance.ResourceCache.GetShop();
        var shopItemInfo = shop.GetItemInfo(selectedCard.ItemId, ItemPriceType.PEN);
        if (shopItemInfo == null)
        {
          Logger.ForAccount(session).Error("CardGamble: No shop entry for card {card}", selectedCard.ItemId);
          await session.SendAsync(new CardGambleAckMessage { Unk1 = 1 });
          return;
        }

        var price = shopItemInfo.PriceGroup.GetPrice(ItemPeriodType.Units, 1);
        if (price == null)
        {
          Logger.ForAccount(session).Error("CardGamble: No price for card {card}", selectedCard.ItemId);
          await session.SendAsync(new CardGambleAckMessage { Unk1 = 1 });
          return;
        }

        var plrItem = plr.Inventory.Create(shopItemInfo, price, selectedCard.Color,
            new[] { (EffectNumber)selectedCard.EffectId }, 1);

        await session.SendAsync(new CardGambleAckMessage
        {
          Unk1 = 0,
          ShopItem = new NeoNetsphere.Network.Data.Game.ShopItemDto
          {
            ItemNumber = selectedCard.ItemId,
            PriceType = ItemPriceType.PEN,
            PeriodType = ItemPeriodType.Units,
            Period = 1,
            Color = selectedCard.Color,
            Effect = selectedCard.EffectId
          }
        });

        if (cardSystem.Formula.GamblePen > 0)
          await session.SendAsync(new MoneyRefreshCashInfoAckMessage(plr.PEN, plr.AP));

        Logger.ForAccount(session).Information("CardGamble: Got card {card} ({name})",
            selectedCard.ItemId, selectedCard.ItemId);

        // Check if all cards collected — auto-reward
        if (season.Reward != null)
        {
          var hasAllCards = season.Cards.All(c =>
              plr.Inventory.Any(i => i.ItemNumber == c.ItemId && i.Count > 0));

          if (hasAllCards)
          {
            // Check if reward already claimed this season
            using (var db = GameDatabase.Open())
            {
              var existing = DbUtil.Find<PlayerCardCollectionDto>(db, statement => statement
                  .Where($"{nameof(PlayerCardCollectionDto.PlayerId):C} = @{nameof(plr.Account.Id)}")
                  .WithParameters(new { plr.Account.Id }))
                  .FirstOrDefault();

              if (existing == null)
              {
                // First completion — give reward and record it
                var rewardShopItem = shop.GetItemInfo(season.Reward.ItemId, ItemPriceType.PEN);
                if (rewardShopItem != null)
                {
                  var rewardPrice = rewardShopItem.PriceGroup.GetPrice(season.Reward.PeriodType,
                      (ushort)season.Reward.PeriodValue);
                  if (rewardPrice != null)
                  {
                    plr.Inventory.Create(rewardShopItem, rewardPrice, season.Reward.Color,
                        new[] { (EffectNumber)season.Reward.EffectId },
                        (uint)(rewardPrice.PeriodType == ItemPeriodType.Units ? rewardPrice.Period : 0));
                  }
                }

                DbUtil.Insert(db, new PlayerCardCollectionDto
                {
                  PlayerId = (int)plr.Account.Id,
                  Season = cardSystem.CurrentSeason,
                  RewardClaimed = true
                });

                Logger.ForAccount(session).Information("CardGamble: Completed card collection! Reward given.");
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error(ex, "CardGamble: Error");
        await session.SendAsync(new CardGambleAckMessage { Unk1 = 1 });
      }
    }

    [MessageHandler(typeof(ItemBuyItemReqMessage))]
    public void BuyItemHandler(GameSession session, ItemBuyItemReqMessage message)
    {
      try
      {
        var shop = GameServer.Instance.ResourceCache.GetShop();
        var plr = session.Player;
        foreach (var item in message.Items)
        {
          var shopItemInfo = shop.GetItemInfo(item.ItemNumber, item.PriceType);
          if (shopItemInfo == null)
          {
            Logger.ForAccount(session)
                .Error("No shop entry found for {item}",
                    new { item.ItemNumber, item.PriceType, item.Period, item.PeriodType });

            session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.UnkownItem));
            return;
          }

          if (shopItemInfo.ShopInfoType == 0)
          {
            Logger.ForAccount(session)
                .Error("Shop entry is not enabled {item}",
                    new { item.ItemNumber, item.PriceType, item.Period, item.PeriodType });

            session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.UnkownItem));
            return;
          }

          var priceGroup = shopItemInfo.PriceGroup;
          var price = priceGroup.GetPrice(item.PeriodType, item.Period);
          if (price == null)
          {
            Logger.ForAccount(session)
                .Error("Invalid price group for shop entry {item}",
                    new { item.ItemNumber, item.PriceType, item.Period, item.PeriodType });

            session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.UnkownItem));
            return;
          }

          if (!price.IsEnabled)
          {
            Logger.ForAccount(session)
                .Error("Shop entry is not enabled {item}",
                    new { item.ItemNumber, item.PriceType, item.Period, item.PeriodType });

            session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.UnkownItem));
            return;
          }

          if (item.Color > shopItemInfo.ShopItem.ColorGroup)
          {
            Logger.ForAccount(session)
                .Error("Shop entry has no color {color} {item}",
                    item.Color, new { item.ItemNumber, item.PriceType, item.Period, item.PeriodType });

            session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.UnkownItem));
            return;
          }

          var itemeffects = new List<EffectNumber>();
          if (item.Effect != 0)
          {
            if (shopItemInfo.EffectGroup.MainEffect == item.Effect)
            {
              foreach (var effect in shopItemInfo.EffectGroup.Effects)
                itemeffects.Add(effect.Effect);
            }
            else
            {
              Logger.ForAccount(session)
                  .Error("Shop entry has no effect {effect} {item}",
                      item.Effect, new { item.ItemNumber, item.PriceType, item.Period, item.PeriodType });

              session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.UnkownItem));
              return;
            }
          }
          else
          {
            itemeffects.Add(0);
          }

          var oldPen = plr.PEN;
          var oldAP = plr.AP;

          // ToDo missing price types
          switch (shopItemInfo.PriceGroup.PriceType)
          {
            case ItemPriceType.PEN:
              if (plr.PEN < price.Price)
              {
                session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.NotEnoughMoney));
                return;
              }

              plr.PEN -= (uint)price.Price;
              break;

            case ItemPriceType.AP:
            case ItemPriceType.Premium:
              if (plr.AP < price.Price)
              {
                session.SendAsync(new ItemBuyItemAckMessage(ItemBuyResult.NotEnoughMoney));
                return;
              }

              plr.AP -= (uint)price.Price;
              break;

            default:
              Logger.ForAccount(session)
                  .Error("Unknown PriceType {priceType}", shopItemInfo.PriceGroup.PriceType);
              return;
          }

          PlayerItem stackitem = null;
          var stacked = false;
          switch (item.PeriodType)
          {
            case ItemPeriodType.None:
              break;
            case ItemPeriodType.Units:
              stackitem = session.Player.Inventory.GetItemByShopInfoId((uint)shopItemInfo.Id);
              if (stackitem != null)
              {
                stackitem.Count += item.Period;
                stackitem.NeedsToSave = true;
                stacked = true;
              }

              break;
            case ItemPeriodType.Days:
              stackitem = session.Player.Inventory.GetItemByShopInfoId((uint)shopItemInfo.Id);
              if (stackitem != null)
              {
                stackitem.DaysLeft += item.Period;
                stackitem.NeedsToSave = true;
                stacked = true;
              }
              break;
            case ItemPeriodType.Hours:
              break;
            default:
              Logger.ForAccount(session)
                  .Error("Unknown PriceType {priceType}", item.PeriodType);
              break;
          }

          var plrItem = stackitem;

          if (!stacked)
          {
            plrItem = session.Player.Inventory.Create(shopItemInfo, price, item.Color,
                itemeffects.ToArray(),
                (uint)(price.PeriodType == ItemPeriodType.Units ? price.Period : 0));
          }
          else
          {
            session.SendAsync(new ItemUpdateInventoryAckMessage(InventoryAction.Update,
                plrItem.Map<PlayerItem, ItemDto>()));
          }

          var result = OnBuyAction(plr, plrItem);
          if (result.Item1 && result.Item2) plr.Inventory.Remove(plrItem);
          if (result.Item1 && !result.Item2)
          {
            plr.AP = oldAP;
            plr.PEN = oldPen;
          }

          session.SendAsync(new ItemBuyItemAckMessage(new[] { plrItem.Id }, item));
          session.SendAsync(new MoneyRefreshCashInfoAckMessage(plr.PEN, plr.AP));
        }
      }
      catch (Exception ex)
      {
        Logger.Information(ex.ToString());
      }
    }

    public Tuple<bool, bool> OnBuyAction(Player plr, PlayerItem item)
    {
      switch (item.ItemNumber)
      {
        default:
          return new Tuple<bool, bool>(false, false);
      }
    }
  }
}
