using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false, ElementName = "arcade_item")]
  public class ArcadeItemInfoDto
  {
    [XmlElement("arcade_item_effect")] public ArcadeItemInfoEffectDto[] arcade_item_effect { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class ArcadeItemInfoEffectDto
  {
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public uint effect_type { get; set; }
    [XmlAttribute] public uint effect_value { get; set; }
    [XmlAttribute] public uint effect_rate { get; set; }
    [XmlAttribute] public uint effect_time { get; set; }
    [XmlAttribute] public uint cooldown_time { get; set; }
    [XmlAttribute] public uint max_stack { get; set; }
  }
}
