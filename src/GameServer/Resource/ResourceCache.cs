using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlubLib.Caching;
using Dapper.FastCrud;
using NeoNetsphere.Database.Auth;
using NeoNetsphere.Database.Game;
using NeoNetsphere.Network;
using Serilog;
using Serilog.Core;

namespace NeoNetsphere.Resource
{
  internal class ResourceCache
  {
    // ReSharper disable once InconsistentNaming
    private static readonly ILogger Logger =
        Log.ForContext(Constants.SourceContextPropertyName, nameof(ResourceCache));

    private readonly ICache _cache = new MemoryCache();
    public readonly ResourceLoader _loader;

    public ResourceCache()
    {
      var path = AppDomain.CurrentDomain.BaseDirectory;
      path = Path.Combine(path, "data");
      _loader = new ResourceLoader(path);
    }

    public void PreCache()
    {
      Logger.Information("Caching: Effects");
      GetEffects();

      Logger.Information("Caching: Items");
      GetItems();

      Logger.Information("Caching: DefaultItems");
      GetDefaultItems();

      Logger.Information("Caching: Shop");
      GetShop();

      Logger.Information("Caching: Experience");
      GetExperience();

      Logger.Information("Caching: Maps");
      GetMaps();

      Logger.Information("Caching: GameTempos");
      GetGameTempos();

      Logger.Information("Caching: Capsules");
      GetItemRewards();

      Logger.Information("Caching: RandomShop");
      GetRandomShop();

      Logger.Information("Caching: CardSystem");
      GetCardSystem();

      Logger.Information("Caching: ExperienceBonus");
      GetExperienceBonus();

      Logger.Information("Caching: PointBonus");
      GetPointBonus();

      Logger.Information("Caching: MasterExperience");
      GetMasterExperience();

      Logger.Information("Caching: BurningTime");
      GetBurningTime();

      Logger.Information("Caching: BurningTimePve");
      GetBurningTimePve();

      Logger.Information("Caching: EquipLimit");
      GetEquipLimit();

      Logger.Information("Caching: RoomOption");
      GetRoomOption();

      Logger.Information("Caching: EnchantData");
      GetEnchantData();

      Logger.Information("Caching: EnchantList");
      GetEnchantList();

      Logger.Information("Caching: EnchantExtractKey");
      GetEnchantExtractKey();

      Logger.Information("Caching: EsperEnchantPrice");
      GetEsperEnchantPrice();

      Logger.Information("Caching: ItemGrade");
      GetItemGrade();

      Logger.Information("Caching: CombineElement");
      GetCombineElement();

      Logger.Information("Caching: DecompositionElement");
      GetDecompositionElement();

      Logger.Information("Caching: SeizeModeNewInfo");
      GetSeizeModeNewInfo();

      Logger.Information("Caching: StadiumInfo");
      GetStadiumInfo();

      Logger.Information("Caching: DecompositionInfo");
      GetDecompositionInfo();

      Logger.Information("Caching: CombinationInfo");
      GetCombinationInfo();

      Logger.Information("Caching: MissionInfo");
      GetMissionInfo();

      Logger.Information("Caching: ArcadeRewardInfo");
      GetArcadeRewardInfo();

      Logger.Information("Caching: ArcadeItemInfo");
      GetArcadeItemInfo();

      Logger.Information("Caching: ChallengeArcadeInfo");
      GetChallengeArcadeInfo();

      Logger.Information("Caching: TaskListInfo");
      GetTaskListInfo();

      Logger.Information("Caching: PromotionInfo");
      GetPromotionInfo();

      Logger.Information("Caching: MakeCharacterInfo");
      GetMakeCharacterInfo();

      Logger.Information("Caching: SupportItemInfo");
      GetSupportItemInfo();
    }

    public IReadOnlyList<ChannelDto> GetChannels()
    {
      var value = _cache.Get<IReadOnlyList<ChannelDto>>(ResourceCacheType.Channels);
      if (value == null)
      {
        Logger.Information("Caching: Channels");
        Logger.Information("Caching...");
        using (var db = GameDatabase.Open())
        {
          value = DbUtil.Find<ChannelDto>(db).ToList();
        }

        _cache.Set(ResourceCacheType.Channels, value);
      }

      return value;
    }

    public IReadOnlyList<DBClubInfoDto> GetClubs()
    {
      var value = _cache.Get<IReadOnlyList<DBClubInfoDto>>(ResourceCacheType.Clubs);
      if (value == null)
      {
        Logger.Information("Caching: Clubs");
        Logger.Information("Caching...");
        using (var db = GameDatabase.Open())
        {
          var clubs = DbUtil.Find<ClubDto>(db).ToList();
          var clubPlayers = DbUtil.Find<ClubPlayerDto>(db).ToList();

          var dbClubInfoList = new List<DBClubInfoDto>();
          foreach (var clubDto in clubs)
          {
            var clubInfo = new DBClubInfoDto { ClubDto = clubDto };
            var dbPlayerInfoList = new List<ClubPlayerInfo>();
            foreach (var playerInfoDto in clubPlayers.Where(p => p.ClubId == clubDto.Id))
            {
              using (var dbC = AuthDatabase.Open())
              {
                var account = DbUtil.Find<AccountDto>(dbC, statement => statement
                        .Where($"{nameof(AccountDto.Id):C} = @{nameof(playerInfoDto.PlayerId)}")
                        .WithParameters(new { playerInfoDto.PlayerId }))
                    .FirstOrDefault();

                dbPlayerInfoList.Add(new ClubPlayerInfo
                {
                  AccountId = (ulong)playerInfoDto.PlayerId,
                  State = (ClubState)playerInfoDto.State,
                  Rank = (ClubRank)playerInfoDto.Rank,
                  Account = account
                });
              }
            }

            clubInfo.PlayerDto = dbPlayerInfoList.ToArray();
            dbClubInfoList.Add(clubInfo);
          }

          value = dbClubInfoList.ToArray();
        }

        _cache.Set(ResourceCacheType.Clubs, value);
      }

      return value;
    }

    public IReadOnlyDictionary<uint, ItemEffect> GetEffects()
    {
      var value = _cache.Get<IReadOnlyDictionary<uint, ItemEffect>>(ResourceCacheType.Effects);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadEffects().ToDictionary(effect => effect.Id);
        _cache.Set(ResourceCacheType.Effects, value);
      }

      return value;
    }

    public IReadOnlyDictionary<ItemNumber, ItemInfo> GetItems()
    {
      var value = _cache.Get<IReadOnlyDictionary<ItemNumber, ItemInfo>>(ResourceCacheType.Items);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadItems_3().ToDictionary(item => item.ItemNumber);
        _cache.Set(ResourceCacheType.Items, value);
      }

      return value;
    }

    public IReadOnlyList<DefaultItem> GetDefaultItems()
    {
      var value = _cache.Get<IReadOnlyList<DefaultItem>>(ResourceCacheType.DefaultItems);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadDefaultItems().ToList();
        _cache.Set(ResourceCacheType.DefaultItems, value);
      }

      return value;
    }

    public ShopResources GetShop()
    {
      var value = _cache.Get<ShopResources>(ResourceCacheType.Shop);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = new ShopResources();
        _cache.Set(ResourceCacheType.Shop, value);
      }

      if (string.IsNullOrWhiteSpace(value.Version))
        value.Load();

      return value;
    }

    public IReadOnlyDictionary<int, Experience> GetExperience()
    {
      var value = _cache.Get<IReadOnlyDictionary<int, Experience>>(ResourceCacheType.Exp);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadExperience().ToDictionary(e => e.Level);
        _cache.Set(ResourceCacheType.Exp, value);
      }

      return value;
    }

    public IReadOnlyDictionary<int, MapInfo> GetMaps()
    {
      var value = _cache.Get<IReadOnlyDictionary<int, MapInfo>>(ResourceCacheType.Maps);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadMaps().ToDictionary(map => map.Id);
        _cache.Set(ResourceCacheType.Maps, value);
      }

      return value;
    }

    public IReadOnlyDictionary<string, GameTempo> GetGameTempos()
    {
      var value = _cache.Get<IReadOnlyDictionary<string, GameTempo>>(ResourceCacheType.GameTempo);
      if (value == null)
      {
        Logger.Information("Caching...");

        value = _loader.LoadGameTempos().ToDictionary(t => t.Name);
        _cache.Set(ResourceCacheType.GameTempo, value);
      }

      return value;
    }

    public IReadOnlyDictionary<ItemNumber, AddCapsule> GetCapsules()
    {
      var value = _cache.Get<IReadOnlyDictionary<ItemNumber, AddCapsule>>(ResourceCacheType.Capsules);
      if (value == null)
      {
        Logger.Information("Caching...");

        value = _loader.LoadCapsules().ToDictionary(t => t.CapsuleItemId);
        _cache.Set(ResourceCacheType.Capsules, value);
      }

      return value;
    }

    public IReadOnlyDictionary<ulong, CapsuleRewards> GetItemRewards()
    {
      var value = _cache.Get<IReadOnlyDictionary<ulong, CapsuleRewards>>(ResourceCacheType.ItemRewards);
      if (value == null)
      {
        Logger.Information("Caching...");

        value = _loader.LoadItemRewards().ToDictionary(t => (ulong)t.Item);
        _cache.Set(ResourceCacheType.ItemRewards, value);
      }

      return value;
    }
    public IReadOnlyList<RandomShopPool> GetRandomShop()
    {
      var value = _cache.Get<IReadOnlyList<RandomShopPool>>(ResourceCacheType.RandomShop);
      if (value == null)
      {
        Logger.Information("Caching...");

        value = _loader.LoadRandomShop().ToList();
        _cache.Set(ResourceCacheType.RandomShop, value);
      }

      return value;
    }

    public CardSystemInfo GetCardSystem()
    {
      var value = _cache.Get<CardSystemInfo>(ResourceCacheType.CardSystem);
      if (value == null)
      {
        Logger.Information("Caching...");

        value = _loader.LoadCardSystem();
        _cache.Set(ResourceCacheType.CardSystem, value);
      }

      return value;
    }

    public ExperienceBonusConfig GetExperienceBonus()
    {
      var value = _cache.Get<ExperienceBonusConfig>(ResourceCacheType.ExperienceBonus);
      if (value == null)
      {
        Logger.Information("Caching...");

        value = _loader.LoadExperienceBonus();
        _cache.Set(ResourceCacheType.ExperienceBonus, value);
      }

      return value;
    }

    public PointBonusConfig GetPointBonus()
    {
      var value = _cache.Get<PointBonusConfig>(ResourceCacheType.PointBonus);
      if (value == null)
      {
        Logger.Information("Caching...");

        value = _loader.LoadPointBonus();
        _cache.Set(ResourceCacheType.PointBonus, value);
      }

      return value;
    }

    public MasterExperience GetMasterExperience()
    {
      var value = _cache.Get<MasterExperience>(ResourceCacheType.MasterExperience);
      if (value == null)
      {
        Logger.Information("Caching...");

        value = _loader.LoadMasterExperience();
        _cache.Set(ResourceCacheType.MasterExperience, value);
      }

      return value;
    }

    public BurningTimeInfo GetBurningTime()
    {
      var value = _cache.Get<BurningTimeInfo>(ResourceCacheType.BurningTime);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadBurningTime();
        _cache.Set(ResourceCacheType.BurningTime, value);
      }

      return value;
    }

    public BurningTimeInfo GetBurningTimePve()
    {
      var value = _cache.Get<BurningTimeInfo>(ResourceCacheType.BurningTimePve);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadBurningTimePve();
        _cache.Set(ResourceCacheType.BurningTimePve, value);
      }

      return value;
    }

    public EquipLimitInfo GetEquipLimit()
    {
      var value = _cache.Get<EquipLimitInfo>(ResourceCacheType.EquipLimit);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadEquipLimit();
        _cache.Set(ResourceCacheType.EquipLimit, value);
      }

      return value;
    }

    public RoomOptionInfo GetRoomOption()
    {
      var value = _cache.Get<RoomOptionInfo>(ResourceCacheType.RoomOption);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadRoomOption();
        _cache.Set(ResourceCacheType.RoomOption, value);
      }

      return value;
    }

    public EnchantInfo GetEnchantData()
    {
      var value = _cache.Get<EnchantInfo>(ResourceCacheType.EnchantData);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadEnchantData();
        _cache.Set(ResourceCacheType.EnchantData, value);
      }

      return value;
    }

    public IReadOnlyList<EnchantEffect> GetEnchantList()
    {
      var value = _cache.Get<IReadOnlyList<EnchantEffect>>(ResourceCacheType.EnchantList);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadEnchantList();
        _cache.Set(ResourceCacheType.EnchantList, value);
      }

      return value;
    }

    public IReadOnlyDictionary<uint, int> GetEnchantExtractKey()
    {
      var value = _cache.Get<IReadOnlyDictionary<uint, int>>(ResourceCacheType.EnchantExtractKey);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadEnchantExtractKey();
        _cache.Set(ResourceCacheType.EnchantExtractKey, value);
      }

      return value;
    }

    public IReadOnlyList<EsperEnchantPriceEntry> GetEsperEnchantPrice()
    {
      var value = _cache.Get<IReadOnlyList<EsperEnchantPriceEntry>>(ResourceCacheType.EsperEnchantPrice);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadEsperEnchantPrice();
        _cache.Set(ResourceCacheType.EsperEnchantPrice, value);
      }

      return value;
    }

    public ItemGradeInfo GetItemGrade()
    {
      var value = _cache.Get<ItemGradeInfo>(ResourceCacheType.ItemGrade);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadItemGrade();
        _cache.Set(ResourceCacheType.ItemGrade, value);
      }

      return value;
    }

    public CombineElementInfo GetCombineElement()
    {
      var value = _cache.Get<CombineElementInfo>(ResourceCacheType.CombineElement);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadCombineElement();
        _cache.Set(ResourceCacheType.CombineElement, value);
      }

      return value;
    }

    public DecompositionElementInfo GetDecompositionElement()
    {
      var value = _cache.Get<DecompositionElementInfo>(ResourceCacheType.DecompositionElement);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadDecompositionElement();
        _cache.Set(ResourceCacheType.DecompositionElement, value);
      }

      return value;
    }

    public SeizeModeInfo GetSeizeModeNewInfo()
    {
      var value = _cache.Get<SeizeModeInfo>(ResourceCacheType.SeizeModeNewInfo);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadSeizeModeNewInfo();
        _cache.Set(ResourceCacheType.SeizeModeNewInfo, value);
      }

      return value;
    }

    public StadiumInfo GetStadiumInfo()
    {
      var value = _cache.Get<StadiumInfo>(ResourceCacheType.StadiumInfo);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadStadiumInfo();
        _cache.Set(ResourceCacheType.StadiumInfo, value);
      }

      return value;
    }

    public DecompositionInfo GetDecompositionInfo()
    {
      var value = _cache.Get<DecompositionInfo>(ResourceCacheType.DecompositionInfo);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadDecompositionInfo();
        _cache.Set(ResourceCacheType.DecompositionInfo, value);
      }

      return value;
    }

    public CombinationInfo GetCombinationInfo()
    {
      var value = _cache.Get<CombinationInfo>(ResourceCacheType.CombinationInfo);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadCombinationInfo();
        _cache.Set(ResourceCacheType.CombinationInfo, value);
      }

      return value;
    }

    public MissionInfo GetMissionInfo()
    {
      var value = _cache.Get<MissionInfo>(ResourceCacheType.MissionInfo);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadMissionInfo();
        _cache.Set(ResourceCacheType.MissionInfo, value);
      }

      return value;
    }

    public ArcadeRewardInfo GetArcadeRewardInfo()
    {
      var value = _cache.Get<ArcadeRewardInfo>(ResourceCacheType.ArcadeRewardInfo);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadArcadeRewardInfo();
        _cache.Set(ResourceCacheType.ArcadeRewardInfo, value);
      }

      return value;
    }

    public ArcadeItemInfo GetArcadeItemInfo()
    {
      var value = _cache.Get<ArcadeItemInfo>(ResourceCacheType.ArcadeItemInfo);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadArcadeItemInfo();
        _cache.Set(ResourceCacheType.ArcadeItemInfo, value);
      }

      return value;
    }

    public ChallengeArcadeInfo GetChallengeArcadeInfo()
    {
      var value = _cache.Get<ChallengeArcadeInfo>(ResourceCacheType.ChallengeArcadeInfo);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadChallengeArcadeInfo();
        _cache.Set(ResourceCacheType.ChallengeArcadeInfo, value);
      }

      return value;
    }

    public TaskListInfo GetTaskListInfo()
    {
      var value = _cache.Get<TaskListInfo>(ResourceCacheType.TaskListInfo);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadTaskListInfo();
        _cache.Set(ResourceCacheType.TaskListInfo, value);
      }

      return value;
    }

    public PromotionInfo GetPromotionInfo()
    {
      var value = _cache.Get<PromotionInfo>(ResourceCacheType.PromotionInfo);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadPromotionInfo();
        _cache.Set(ResourceCacheType.PromotionInfo, value);
      }

      return value;
    }

    public MakeCharacterInfo GetMakeCharacterInfo()
    {
      var value = _cache.Get<MakeCharacterInfo>(ResourceCacheType.MakeCharacterInfo);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadMakeCharacterInfo();
        _cache.Set(ResourceCacheType.MakeCharacterInfo, value);
      }

      return value;
    }

    public SupportItemInfo GetSupportItemInfo()
    {
      var value = _cache.Get<SupportItemInfo>(ResourceCacheType.SupportItemInfo);
      if (value == null)
      {
        Logger.Information("Caching...");
        value = _loader.LoadSupportItemInfo();
        _cache.Set(ResourceCacheType.SupportItemInfo, value);
      }

      return value;
    }

    public void Clear()
    {
      Logger.Information("Clearing cache");
      _cache.Clear();
    }

    public void Clear(ResourceCacheType type)
    {
      Logger.Information($"Clearing cache for {type}");

      if (type == ResourceCacheType.Shop)
      {
        GetShop().Clear();
        return;
      }

      _cache.Remove(type.ToString());
    }
  }

  internal static class ResourceCacheExtensions
  {
    public static T Get<T>(this ICache cache, ResourceCacheType type)
        where T : class
    {
      return cache.Get<T>(type.ToString());
    }

    public static void Set(this ICache cache, ResourceCacheType type, object value)
    {
      cache.Set(type.ToString(), value);
    }

    public static void Set(this ICache cache, ResourceCacheType type, object value, TimeSpan ts)
    {
      cache.Set(type.ToString(), value, ts);
    }
  }
}