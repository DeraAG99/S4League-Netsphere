using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "experience_bonus")]
  public class ExperienceBonusDto
  {
    [XmlElement("football")] public ExperienceBonusModeDto Football { get; set; }
    [XmlElement("death_match")] public ExperienceBonusModeDto DeathMatch { get; set; }
    [XmlElement("survival")] public ExperienceBonusModeDto Survival { get; set; }
    [XmlElement("mission")] public ExperienceBonusModeDto Mission { get; set; }
    [XmlElement("arcade")] public ExperienceBonusModeDto Arcade { get; set; }
    [XmlElement("slaughter")] public ExperienceBonusModeDto Slaughter { get; set; }
    [XmlElement("freeforall")] public ExperienceBonusModeDto FreeForAll { get; set; }
    [XmlElement("captain")] public ExperienceBonusModeDto Captain { get; set; }
    [XmlElement("seize")] public ExperienceBonusModeDto Seize { get; set; }
    [XmlElement("horde")] public ExperienceBonusModeDto Horde { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class ExperienceBonusModeDto
  {
    [XmlAttribute] public float ranking_factor { get; set; }
    [XmlAttribute] public float player_count_factor { get; set; }
    [XmlAttribute] public float variable_exp_per_min { get; set; }
    [XmlAttribute] public float constant_exp_per_min { get; set; }
    [XmlAttribute] public float damageranking_1st_point { get; set; }
    [XmlAttribute] public float damageranking_2nd_point { get; set; }
    [XmlAttribute] public float damageranking_3rd_point { get; set; }
  }
}
