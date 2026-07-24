using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "point_bonus")]
  public class PointBonusDto
  {
    [XmlElement("football")] public PointBonusModeDto Football { get; set; }
    [XmlElement("death_match")] public PointBonusModeDto DeathMatch { get; set; }
    [XmlElement("survival")] public PointBonusModeDto Survival { get; set; }
    [XmlElement("mission")] public PointBonusMissionDto Mission { get; set; }
    [XmlElement("arcade")] public PointBonusArcadeDto Arcade { get; set; }
    [XmlElement("slaughter")] public PointBonusModeDto Slaughter { get; set; }
    [XmlElement("freeforall")] public PointBonusModeDto FreeForAll { get; set; }
    [XmlElement("captain")] public PointBonusModeDto Captain { get; set; }
    [XmlElement("level_bonus")] public PointBonusLevelDto[] LevelBonuses { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PointBonusModeDto
  {
    [XmlAttribute] public float ranking_factor { get; set; }
    [XmlAttribute] public float player_count_factor { get; set; }
    [XmlAttribute] public float point_per_min { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PointBonusMissionDto
  {
    [XmlAttribute] public float win_point_per_min { get; set; }
    [XmlAttribute] public float lose_point_per_min { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PointBonusArcadeDto
  {
    [XmlAttribute] public float point_per_min { get; set; }
    [XmlAttribute] public float basic_point_scale { get; set; }
    [XmlAttribute] public float time_scale { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PointBonusLevelDto
  {
    [XmlAttribute] public int min { get; set; }
    [XmlAttribute] public int max { get; set; }
    [XmlAttribute] public int pen_bonus { get; set; }
  }
}
