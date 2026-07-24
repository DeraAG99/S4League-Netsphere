using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class CardSystemInfo
  {
    public bool Active { get; set; }
    public int CurrentSeason { get; set; }
    public List<CardSeasonInfo> Seasons { get; set; } = new List<CardSeasonInfo>();
    public CardFormula Formula { get; set; } = new CardFormula();
  }

  internal class CardSeasonInfo
  {
    public int Num { get; set; }
    public uint BuyCapsule { get; set; }
    public int ShopId { get; set; }
    public List<CardEntry> Cards { get; set; } = new List<CardEntry>();
    public CardReward Reward { get; set; }
  }

  internal class CardEntry
  {
    public int Num { get; set; }
    public ItemNumber ItemId { get; set; }
    public int ShopId { get; set; }
    public ItemPeriodType PeriodType { get; set; }
    public int PeriodValue { get; set; }
    public byte Color { get; set; }
    public uint EffectId { get; set; }
    public int Grade { get; set; }
    public int PlayProb { get; set; }
    public int TryProb { get; set; }
  }

  internal class CardReward
  {
    public ItemNumber ItemId { get; set; }
    public int ShopId { get; set; }
    public ItemPeriodType PeriodType { get; set; }
    public int PeriodValue { get; set; }
    public byte Color { get; set; }
    public uint EffectId { get; set; }
  }

  internal class CardFormula
  {
    public int PlayLimitTime { get; set; }
    public int PlayLimitMinCount { get; set; }
    public int PlayDefaultTime { get; set; }
    public int PlayDefaultCount { get; set; }
    public int GamblePen { get; set; }
    public int GambleLimitMinCount { get; set; }
    public int CompleteCardCount { get; set; }
  }
}
