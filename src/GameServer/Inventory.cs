using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BlubLib.Collections.Concurrent;
using ExpressMapper.Extensions;
using NeoNetsphere.Database.Game;
using NeoNetsphere.Network;
using NeoNetsphere.Network.Data.Game;
using NeoNetsphere.Network.Message.Game;
using NeoNetsphere.Shop;
using Serilog;

namespace NeoNetsphere
{
    internal class Inventory : IReadOnlyCollection<PlayerItem>
    {
        private readonly ConcurrentDictionary<ulong, PlayerItem> _items = new ConcurrentDictionary<ulong, PlayerItem>();
        private readonly ConcurrentStack<PlayerItem> _itemsToDelete = new ConcurrentStack<PlayerItem>();

        internal Inventory(Player plr, PlayerDto dto)
        {
            Player = plr;

            foreach (var item in dto.Items.Select(i => new PlayerItem(this, i)))
                if (!item.IsInvalid)
                    _items.TryAdd(item.Id, item);
                else if (item.ExpireDate != 0)
                    _items.TryAdd(item.Id, item);
                else
                    _itemsToDelete.Push(item);
        }

        public Player Player { get; }

        /// <summary>
        ///     Returns the item with the given id or null if not found
        /// </summary>
        public PlayerItem this[ulong id] => GetItem(id);

        public int Count => _items.Count;

        public IEnumerator<PlayerItem> GetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Returns the item with the given id or null if not found
        /// </summary>
        public PlayerItem GetItem(ulong id)
        {
            PlayerItem item;
            _items.TryGetValue(id, out item);
            return item;
        }

        /// <summary>
        ///     Returns the item with the given id or null if not found
        /// </summary>
        public PlayerItem GetItemByShopInfoId(uint id)
        {
            try
            {
                var item = _items.Values.Where(item_ => item_.GetShopItemInfo().Id == id).ToList();
                if (item.Count < 1)
                    return null;

                return item.LastOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        ///     Creates a new item
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public PlayerItem Create(ItemNumber itemNumber, ItemPriceType priceType, ItemPeriodType periodType,
            ushort period, byte color, EffectNumber[] effects, uint count)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();

            var shopItemInfo = shop.GetItemInfo(itemNumber, priceType);
            if (shopItemInfo == null)
                throw new ArgumentException($"Item not found : {itemNumber.Id}");

            var price = shopItemInfo.PriceGroup.GetPrice(periodType, period);
            if (price == null)
                throw new ArgumentException($"Price not found : {priceType}");
            return Create(shopItemInfo, price, color, effects, count);
        }

        /// <summary>
        ///     Creates a new item
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public PlayerItem Create(ItemNumber itemNumber,
            ushort period, byte color, EffectNumber[] effects, uint count)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();

            var shopItemInfo = shop.GetFirstItemInfo(itemNumber);
            if (shopItemInfo == null)
                throw new ArgumentException($"Item not found : {itemNumber.Id}");

            var itemEffects = new List<EffectNumber>();
            foreach (var effect in shopItemInfo.EffectGroup.Effects) itemEffects.Add(effect.Effect);

            var priceType = shopItemInfo.PriceGroup.PriceType;
            var periodType = shopItemInfo.PriceGroup.Prices.FirstOrDefault().PeriodType;
            var periodNr = shopItemInfo.PriceGroup.Prices.FirstOrDefault().Period;
            return Create(itemNumber, priceType, periodType, periodNr, color, effects, count);
        }

        /// <summary>
        ///     Creates a new item
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public PlayerItem Create(CapsuleRewardDto rewardDto)
        {
            if (rewardDto.RewardType != CapsuleRewardType.Item)
                // Return null karena PEN bukan item
                return null;
            var shop = GameServer.Instance.ResourceCache.GetShop();
            var shopItemInfo = shop.GetItemInfo(rewardDto.ItemNumber, rewardDto.PriceType);

            if (shopItemInfo == null)
                throw new ArgumentException($"ShopItemInfo not found for item {rewardDto.ItemNumber}");

            var period = rewardDto.Period > ushort.MaxValue ? ushort.MaxValue : (ushort)rewardDto.Period;
            var price = shopItemInfo.PriceGroup.GetPrice(rewardDto.PeriodType, period);

            if (price == null)
                throw new ArgumentException(
                    $"Price not found for item {rewardDto.ItemNumber} with period type {rewardDto.PeriodType}");

            var itemEffects = new List<EffectNumber>();
            foreach (var effect in shopItemInfo.EffectGroup.Effects) itemEffects.Add(effect.Effect);
            if (itemEffects.Count == 0)
                itemEffects.Add(0);

            var rewardItem = new PlayerItem(this, shopItemInfo, price, rewardDto.Color, itemEffects.ToArray(),
                DateTimeOffset.UtcNow, 1);
            var existingItem = FindMatchingItem(rewardItem);
            if (existingItem != null)
            {
                var updated = false;

                switch (existingItem.PeriodType)
                {
                    case ItemPeriodType.Days:
                        existingItem.Period = (ushort)Math.Min(existingItem.Period + period, ushort.MaxValue);
                        existingItem.DaysLeft = existingItem.Period;
                        updated = true;
                        break;

                    case ItemPeriodType.Units:
                        existingItem.Count += period;
                        updated = true;
                        break;
                }

                if (updated)
                {
                    existingItem.NeedsToSave = true;
                    Player.Session.SendAsync(new ItemUpdateInventoryAckMessage(
                        InventoryAction.Update, existingItem.Map<PlayerItem, ItemDto>()));
                }

                return existingItem;
            }

            _items.TryAdd(rewardItem.Id, rewardItem);
            Player.Session.SendAsync(new ItemUpdateInventoryAckMessage(
                InventoryAction.Add, rewardItem.Map<PlayerItem, ItemDto>()));
            return rewardItem;
        }

        /// <summary>
        ///     Creates a new item
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public PlayerItem CreateSilent(ItemNumber itemNumber,
            ushort period, byte color, uint count)
        {
            var shop = GameServer.Instance.ResourceCache.GetShop();

            var shopItemInfo = shop.GetFirstItemInfo(itemNumber);
            if (shopItemInfo == null)
                throw new ArgumentException($"Item not found : {itemNumber.Id}");

            if (shopItemInfo == null)
                throw new ArgumentException($"Item not found : {itemNumber.Id}");

            var price = shopItemInfo.PriceGroup.Prices.FirstOrDefault();
            if (price == null)
                throw new ArgumentException("Item has no price");

            var effects = shopItemInfo.EffectGroup.Effects.Select(x => (EffectNumber)x.Effect).ToArray();
            return CreateSilent(shopItemInfo, price, color, effects, count);
        }

        /// <summary>
        ///     Creates a new item
        /// </summary>
        /// <exception cref="CharacterException"></exception>
        public PlayerItem Create(ShopItemInfo shopItemInfo, ShopPrice price, byte color, EffectNumber[] effects,
            uint count)
        {
            if (effects.Length == 0)
                effects = new EffectNumber[] { 0 };
            var item = new PlayerItem(this, shopItemInfo, price, color, effects, DateTimeOffset.Now, count);
            _items.TryAdd(item.Id, item);
            Player.Session.SendAsync(
                new ItemUpdateInventoryAckMessage(InventoryAction.Add, item.Map<PlayerItem, ItemDto>()));
            return item;
        }

        /// <summary>
        ///     Creates a new item
        /// </summary>
        /// <exception cref="CharacterException"></exception>
        public PlayerItem CreateSilent(ShopItemInfo shopItemInfo, ShopPrice price, byte color, EffectNumber[] effects,
            uint count)
        {
            if (effects.Length == 0)
                effects = new EffectNumber[] { 0 };

            var item = new PlayerItem(this, shopItemInfo, price, color, effects, DateTimeOffset.Now, count);
            _items.TryAdd(item.Id, item);
            return item;
        }

        /// <summary>
        ///     Removes the item from the inventory
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Remove(PlayerItem item)
        {
            Remove(item.Id);
        }

        /// <summary>
        ///     Removes or decreases the count of the item from/in the inventory
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void RemoveOrDecrease(PlayerItem item)
        {
            if (item.PeriodType == ItemPeriodType.Units)
            {
                item.Count--;
                if (item.Count <= 0)
                    Remove(item.Id);
                else
                    Player.SendAsync(new ItemUpdateInventoryAckMessage(InventoryAction.Update,
                        item.Map<PlayerItem, ItemDto>()));
            }
            else
            {
                Remove(item.Id);
            }
        }

        /// <summary>
        ///     Removes the item from the inventory
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Remove(ulong id)
        {
            var item = GetItem(id);
            if (item == null)
                throw new ArgumentException($"Item {id} not found", nameof(id));

            _items.Remove(item.Id);
            if (item.ExistsInDatabase)
                _itemsToDelete.Push(item);

            Player.Session.SendAsync(new ItemInventroyDeleteAckMessage(item.Id));
        }

        public void RemoveInvalid(PlayerItem item)
        {
            _items.Remove(item.Id);
            if (item.ExistsInDatabase)
                _itemsToDelete.Push(item);
        }

        internal PlayerItem FindMatchingItem(PlayerItem Item, bool includePermanent = false)
        {
            if (!includePermanent && Item.PeriodType == ItemPeriodType.None)
                return null;
            var ItemEffects = Item.Effects?.OrderBy(e => e.Id).ToArray() ?? Array.Empty<EffectNumber>();

            return _items.Values.FirstOrDefault(x =>
                x.ItemNumber == Item.ItemNumber &&
                x.PriceType == Item.PriceType &&
                x.PeriodType == Item.PeriodType &&
                x.Color == Item.Color &&
                x.Effects?.OrderBy(e => e.Id).SequenceEqual(ItemEffects) == true);
        }

        internal void Save(IDbConnection db)
        {
            if (Player.Room == null)
            {
                var expireItems = _items.Values
                    .Where(it => it.ExpireDate == 0)
                    .ToList();

                foreach (var it in expireItems)
                    Remove(it);
            }

            if (!_itemsToDelete.IsEmpty)
            {
                var idsToRemove = new StringBuilder();
                var firstRun = true;
                PlayerItem itemToDelete;

                while (_itemsToDelete.TryPop(out itemToDelete))
                {
                    if (firstRun)
                        firstRun = false;
                    else
                        idsToRemove.Append(',');

                    idsToRemove.Append(itemToDelete.Id);
                }

                DbUtil.BulkDelete<PlayerItemDto>(db, statement => statement
                    .Where($"{nameof(PlayerItemDto.Id):C} IN ({idsToRemove})"));
            }

            foreach (var item in _items.Values)
            {
                var rawEffects = item.Effects?.ToList() ?? new List<EffectNumber> { 0 };
                string dtoEffects;

                try
                {
                    dtoEffects = string.Join(",", rawEffects);
                }
                catch
                {
                    dtoEffects = "0";
                }

                // Ambil ShopItemInfo
                var shopItem = item.GetShopItemInfo();
                if (shopItem == null)
                {
                    Log.Error(
                        $"[Inventory.Save] ShopItemInfo null for ItemId={item.Id}, ItemNumber={item.ItemNumber.Id}");
                    continue;
                }

                // Ambil harga berdasarkan period type + period
                var price = shopItem?.PriceGroup?.GetPrice(item.PeriodType, item.Period);

                if (price == null)
                {
                    price = shopItem.PriceGroup?.Prices
                        .Where(p => p.PeriodType == item.PeriodType && p.Period <= item.Period)
                        .OrderByDescending(p => p.Period)
                        .FirstOrDefault();

                    if (price == null)
                        price = shopItem.PriceGroup?.Prices
                            .OrderByDescending(p => p.Period)
                            .FirstOrDefault();
                    if (price != null)
                        Log.Warning(
                            $"[Inventory.Save] Used fallback price for ItemId={item.Id} Period={item.Period}, fallback to Period={price.Period}");
                }

                if (price == null)
                {
                    Log.Error(
                        $"[Inventory.Save] No price found for ItemId={item.Id}, ItemNumber={item.ItemNumber.Id} PeriodType={item.PeriodType} Period={item.Period}, skipping save");
                    continue;
                }


                var dto = new PlayerItemDto
                {
                    Id = (int)item.Id,
                    PlayerId = (int)Player.Account.Id,
                    ShopItemInfoId = shopItem.Id,
                    ShopPriceId = price.Id,
                    Period = item.Period,
                    DaysLeft = item.DaysLeft,
                    Effects = dtoEffects,
                    Color = item.Color,
                    PurchaseDate = item.PurchaseDate.ToUnixTimeSeconds(),
                    Durability = item.Durability,
                    Count = (int)item.Count
                };

                if (!item.ExistsInDatabase)
                {
                    DbUtil.Insert(db, dto);
                    item.ExistsInDatabase = true;
                }
                else
                {
                    if (!item.NeedsToSave)
                        continue;

                    DbUtil.Update(db, dto);
                    item.NeedsToSave = false;
                }
            }
        }

        public bool Contains(ulong id)
        {
            return _items.ContainsKey(id);
        }
    }
}