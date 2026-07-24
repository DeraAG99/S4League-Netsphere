using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class SeizeModeInfo
  {
    public int Gauge { get; set; }
    public int GaugeUpDelay { get; set; }
    public int CoreFootholder { get; set; }
    public int AssistFootholder { get; set; }
    public bool ResetOnLeaveOrDeath { get; set; }
    public int PointPerCapture { get; set; }
    public int AssistPointPerCapture { get; set; }
    public bool UpkeepEnabled { get; set; }
    public int UpkeepDelay { get; set; }
    public int UpkeepScore { get; set; }
    public bool TimeBonusEnabled { get; set; }
    public int TimeBonusDelay { get; set; }
    public int TimeBonusDefault { get; set; }
    public int TimeBonusAdd { get; set; }
    public int TimeBonusAddLimit { get; set; }
  }

  internal class StadiumInfo
  {
    public IReadOnlyList<StadiumMapInfo> MapInfos { get; set; }
  }

  internal class StadiumMapInfo
  {
    public int MapId { get; set; }
    public int Mode { get; set; }
    public IReadOnlyList<StadiumBlastInfo> BlastInfos { get; set; }
  }

  internal class StadiumBlastInfo
  {
    public int Index { get; set; }
    public string Name { get; set; }
    public int DefaultHp { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
    public int IncHp { get; set; }
    public int UsePoint { get; set; }
  }
}
