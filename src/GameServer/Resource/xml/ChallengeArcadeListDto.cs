using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false, ElementName = "challenge_arcade_list")]
  public class ChallengeArcadeListDto
  {
    [XmlElement("list_setting")] public ChallengeArcadeListSettingDto[] list_setting { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class ChallengeArcadeListSettingDto
  {
    [XmlElement("condition")] public ChallengeArcadeListConditionDto condition { get; set; }
    [XmlElement("reward")] public ChallengeArcadeListRewardDto reward { get; set; }

    [XmlAttribute] public uint id { get; set; }
    [XmlAttribute] public string name_key { get; set; }
    [XmlAttribute] public uint map_id { get; set; }
    [XmlAttribute] public uint difficulty { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class ChallengeArcadeListConditionDto
  {
    [XmlAttribute] public uint condition_type { get; set; }
    [XmlAttribute] public uint condition_value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class ChallengeArcadeListRewardDto
  {
    [XmlAttribute] public uint exp { get; set; }
    [XmlAttribute] public uint pen { get; set; }
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public uint shop_id { get; set; }
    [XmlAttribute] public string period_type { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effect_id { get; set; }
  }
}
