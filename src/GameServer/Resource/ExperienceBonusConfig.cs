using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class ExperienceBonusConfig
  {
    public ExperienceBonusEntry Touchdown { get; set; } = new ExperienceBonusEntry();
    public ExperienceBonusEntry Deathmatch { get; set; } = new ExperienceBonusEntry();
    public ExperienceBonusEntry Survival { get; set; } = new ExperienceBonusEntry();
    public ExperienceBonusEntry Captain { get; set; } = new ExperienceBonusEntry();
    public ExperienceBonusEntry Chaser { get; set; } = new ExperienceBonusEntry();
    public ExperienceBonusEntry BattleRoyal { get; set; } = new ExperienceBonusEntry();
    public ExperienceBonusEntry SnowballFight { get; set; } = new ExperienceBonusEntry();
    public ExperienceBonusEntry Slaughter { get; set; } = new ExperienceBonusEntry();
    public ExperienceBonusEntry FreeForAll { get; set; } = new ExperienceBonusEntry();
    public ExperienceBonusEntry Horde { get; set; } = new ExperienceBonusEntry();
    public ExperienceBonusEntry Seize { get; set; } = new ExperienceBonusEntry();
  }

  internal class ExperienceBonusEntry
  {
    public float RankingFactor { get; set; }
    public float PlayerCountFactor { get; set; }
    public float VariableExpPerMin { get; set; }
    public float ConstantExpPerMin { get; set; }
    public float DamageRanking1stPoint { get; set; }
    public float DamageRanking2ndPoint { get; set; }
    public float DamageRanking3rdPoint { get; set; }
    public bool IsValid { get; set; }
  }
}
