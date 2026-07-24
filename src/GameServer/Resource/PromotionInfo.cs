using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class PromotionInfo
  {
    public bool RouletteActive { get; set; }
    public uint RouletteUseItemKey { get; set; }
    public uint RouletteUseItemCount { get; set; }
    public IReadOnlyList<PromotionEventInfo> EventInfos { get; set; }
    public IReadOnlyList<PromotionAttendanceDay> AttendanceDays { get; set; }
    public bool DailyGiftActive { get; set; }
    public IReadOnlyList<PromotionRequital> DailyGiftRequitals { get; set; }
    public bool DailyPlayTimeActive { get; set; }
    public IReadOnlyList<PromotionRequital> DailyPlayTimeRequitals { get; set; }
  }

  internal class PromotionEventInfo
  {
    public uint EventType { get; set; }
    public bool Active { get; set; }
    public string EventTitle { get; set; }
    public uint RewardType { get; set; }
    public uint MinPlayer { get; set; }
    public uint MinTime { get; set; }
    public uint ChannelId { get; set; }
    public uint MapId { get; set; }
    public uint GameMode { get; set; }
    public uint MinScore { get; set; }
    public string GiftType { get; set; }
    public uint ItemKey { get; set; }
    public uint ShopId { get; set; }
    public string PeriodType { get; set; }
    public uint Period { get; set; }
    public uint Color { get; set; }
    public uint EffectId { get; set; }
    public uint Probability { get; set; }
    public uint RewardItemLimitCount { get; set; }
  }

  internal class PromotionAttendanceDay
  {
    public uint ItemIndex { get; set; }
    public uint UserType { get; set; }
    public uint Year { get; set; }
    public uint Week { get; set; }
    public uint DayOfWeek { get; set; }
    public uint ItemKey { get; set; }
    public uint ShopId { get; set; }
    public string PeriodType { get; set; }
    public uint Period { get; set; }
    public uint Color { get; set; }
    public uint EffectId { get; set; }
  }

  internal class PromotionRequital
  {
    public uint Key { get; set; }
    public string GiftType { get; set; }
    public uint GiftValue { get; set; }
    public uint ItemKey { get; set; }
    public uint ShopId { get; set; }
    public string PeriodType { get; set; }
    public uint Period { get; set; }
    public uint Color { get; set; }
    public uint EffectId { get; set; }
    public uint Probability { get; set; }
  }
}
