using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false, ElementName = "decomposition")]
  public class DecompositionInfoDto
  {
    [XmlElement("method")] public DecompositionInfoMethodDto[] method { get; set; }
    [XmlElement("bonus_data")] public DecompositionInfoBonusDataDto bonus_data { get; set; }
    [XmlElement("prohibition")] public DecompositionInfoProhibitionDto prohibition { get; set; }
    [XmlElement("Subprohibition")] public DecompositionInfoSubprohibitionDto subprohibition { get; set; }

    [XmlAttribute] public uint pen_price { get; set; }
    [XmlAttribute] public uint min_hours { get; set; }
    [XmlAttribute] public uint min_days { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class DecompositionInfoMethodDto
  {
    [XmlElement("component")] public DecompositionInfoMethodComponentDto[] component { get; set; }

    [XmlAttribute] public string period_type { get; set; }
    [XmlAttribute] public uint effect_min_cnt { get; set; }
    [XmlAttribute] public uint effect_max_cnt { get; set; }
    [XmlAttribute] public string use { get; set; }
    [XmlAttribute] public string bonus { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class DecompositionInfoMethodComponentDto
  {
    [XmlAttribute] public uint condition { get; set; }
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public uint shop_id { get; set; }
    [XmlAttribute] public string period_type { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effect_id { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class DecompositionInfoBonusDataDto
  {
    [XmlElement("bonus")] public DecompositionInfoBonusDataBonusDto[] bonus { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class DecompositionInfoBonusDataBonusDto
  {
    [XmlAttribute] public uint period_multiple_value { get; set; }
    [XmlAttribute] public string item_main_type { get; set; }
    [XmlAttribute] public string item_sub_type { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class DecompositionInfoProhibitionDto
  {
    [XmlElement("data")] public DecompositionInfoProhibitionDataDto[] data { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class DecompositionInfoProhibitionDataDto
  {
    [XmlAttribute] public uint item_key { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class DecompositionInfoSubprohibitionDto
  {
  }
}
