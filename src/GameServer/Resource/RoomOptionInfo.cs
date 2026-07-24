using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class RoomOptionInfo
  {
    public int RewardConditionTime { get; set; }
    public IReadOnlyList<RoomOptionModeEntry> Modes { get; set; }
    public IReadOnlyList<RoomOptionRewardEntry> ModeRewards { get; set; }
  }

  internal class RoomOptionModeEntry
  {
    public int ModeId { get; set; }
    public int Probability { get; set; }
    public int ScoreLimit { get; set; }
    public int TimeLimit { get; set; }
    public int LimitPlayer { get; set; }
    public int SpectatorCount { get; set; }
    public int LimitPlayTime { get; set; }
  }

  internal class RoomOptionRewardEntry
  {
    public int MinPlayer { get; set; }
    public IReadOnlyList<RoomOptionRequitalEntry> Requitals { get; set; }
  }

  internal class RoomOptionRequitalEntry
  {
    public int Key { get; set; }
    public string GiftType { get; set; }
    public uint ItemKey { get; set; }
    public int ShopId { get; set; }
    public string PeriodType { get; set; }
    public int Period { get; set; }
    public int Color { get; set; }
    public int EffectId { get; set; }
    public int Probability { get; set; }
  }
}
