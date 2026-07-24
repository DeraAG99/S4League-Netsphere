using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false, ElementName = "arcade_reward")]
  public class ArcadeRewardInfoDto
  {
    [XmlElement("arcade_reward_grade")] public ArcadeRewardInfoGradeDto[] arcade_reward_grade { get; set; }
    [XmlElement("arcade_reward_item")] public ArcadeRewardInfoItemDto[] arcade_reward_item { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class ArcadeRewardInfoGradeDto
  {
    [XmlAttribute] public uint grade { get; set; }
    [XmlAttribute] public string name_key { get; set; }
    [XmlAttribute] public uint min_score { get; set; }
    [XmlAttribute] public uint max_score { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class ArcadeRewardInfoItemDto
  {
    [XmlAttribute] public uint map_id { get; set; }
    [XmlAttribute] public uint difficulty { get; set; }
    [XmlAttribute] public uint grade { get; set; }
    [XmlAttribute] public uint category { get; set; }
    [XmlAttribute] public uint sub_category { get; set; }
    [XmlAttribute] public uint item_number { get; set; }
    [XmlAttribute] public uint product_number { get; set; }
    [XmlAttribute] public uint probability { get; set; }
    [XmlAttribute] public uint min_score { get; set; }
    [XmlAttribute] public uint max_score { get; set; }
  }
}
