using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class ChallengeArcadeInfo
  {
    public IReadOnlyList<ChallengeArcadeEntry> Entries { get; set; }
  }

  internal class ChallengeArcadeEntry
  {
    public uint Id { get; set; }
    public string NameKey { get; set; }
    public uint MapId { get; set; }
    public uint Difficulty { get; set; }
    public uint ConditionType { get; set; }
    public uint ConditionValue { get; set; }
    public uint ExpReward { get; set; }
    public uint PenReward { get; set; }
    public uint ItemKey { get; set; }
    public uint ShopId { get; set; }
    public string PeriodType { get; set; }
    public uint Period { get; set; }
    public uint Color { get; set; }
    public uint EffectId { get; set; }
  }
}
