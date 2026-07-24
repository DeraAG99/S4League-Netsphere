using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class MissionInfo
  {
    public uint MissionCheckInterval { get; set; }
    public uint MaxMissionCount { get; set; }
    public uint MissionRewardMailExpireDays { get; set; }
    public uint DailyPvpMissionCount { get; set; }
    public uint DailyPveMissionCount { get; set; }
    public uint DailyMissionRewardMailExpireDays { get; set; }
    public IReadOnlyList<MissionEntry> DailyPvpMissions { get; set; }
    public IReadOnlyList<MissionEntry> DailyPveMissions { get; set; }
  }

  internal class MissionEntry
  {
    public uint Id { get; set; }
    public string NameKey { get; set; }
    public IReadOnlyList<MissionCondition> Conditions { get; set; }
    public IReadOnlyList<MissionReward> Rewards { get; set; }
  }

  internal class MissionCondition
  {
    public uint ConditionType { get; set; }
    public uint ConditionValue { get; set; }
    public uint MapId { get; set; }
    public string GameType { get; set; }
  }

  internal class MissionReward
  {
    public uint RewardType { get; set; }
    public uint RewardValue { get; set; }
    public uint ItemKey { get; set; }
    public uint ShopId { get; set; }
    public string PeriodType { get; set; }
    public uint Period { get; set; }
    public uint Color { get; set; }
    public uint EffectId { get; set; }
  }
}
