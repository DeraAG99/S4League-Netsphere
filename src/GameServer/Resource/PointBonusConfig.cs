using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class PointBonusConfig
  {
    public PointBonusEntry Touchdown { get; set; } = new PointBonusEntry();
    public PointBonusEntry Deathmatch { get; set; } = new PointBonusEntry();
    public PointBonusEntry Survival { get; set; } = new PointBonusEntry();
    public PointBonusEntry Captain { get; set; } = new PointBonusEntry();
    public PointBonusEntry Chaser { get; set; } = new PointBonusEntry();
    public PointBonusEntry BattleRoyal { get; set; } = new PointBonusEntry();
    public PointBonusEntry SnowballFight { get; set; } = new PointBonusEntry();
    public PointBonusEntry Arcade { get; set; } = new PointBonusEntry();
    public PointBonusEntry Horde { get; set; } = new PointBonusEntry();
    public PointBonusEntry Siege { get; set; } = new PointBonusEntry();
    public List<PointBonusLevelEntry> LevelBonuses { get; set; } = new List<PointBonusLevelEntry>();
  }

  internal class PointBonusEntry
  {
    public float RankingFactor { get; set; }
    public float PlayerCountFactor { get; set; }
    public float PointPerMin { get; set; }
    public bool IsValid { get; set; }
  }

  internal class PointBonusLevelEntry
  {
    public int Min { get; set; }
    public int Max { get; set; }
    public int PenBonus { get; set; }
  }
}
