using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false, ElementName = "combination")]
  public class CombinationInfoDto
  {
    [XmlElement("argon_component")] public CombinationInfoArgonComponentDto argon_component { get; set; }
    [XmlElement("krypton_component")] public CombinationInfoKryptonComponentDto krypton_component { get; set; }
    [XmlElement("enchant_option")] public CombinationInfoEnchantOptionDto enchant_option { get; set; }
    [XmlElement("overcount_weight")] public CombinationInfoOvercountWeightDto overcount_weight { get; set; }
    [XmlElement("component")] public CombinationInfoComponentDto[] component { get; set; }

    [XmlAttribute] public uint pen_price { get; set; }
    [XmlAttribute] public uint min_hours { get; set; }
    [XmlAttribute] public uint min_days { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class CombinationInfoArgonComponentDto
  {
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public uint shop_id { get; set; }
    [XmlAttribute] public string period_type { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effect_id { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class CombinationInfoKryptonComponentDto
  {
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public uint shop_id { get; set; }
    [XmlAttribute] public string period_type { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effect_id { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class CombinationInfoEnchantOptionDto
  {
    [XmlAttribute] public uint enchant_item_key { get; set; }
    [XmlAttribute] public uint enchant_shop_id { get; set; }
    [XmlAttribute] public string enchant_period_type { get; set; }
    [XmlAttribute] public uint enchant_period { get; set; }
    [XmlAttribute] public uint protect_item_key { get; set; }
    [XmlAttribute] public uint protect_shop_id { get; set; }
    [XmlAttribute] public string protect_period_type { get; set; }
    [XmlAttribute] public uint protect_period { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class CombinationInfoOvercountWeightDto
  {
    [XmlAttribute] public uint max_level { get; set; }
    [XmlAttribute] public uint weight_max { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class CombinationInfoComponentDto
  {
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public uint shop_id { get; set; }
    [XmlAttribute] public string period_type { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effect_id { get; set; }
    [XmlAttribute] public uint min_use_cnt { get; set; }
    [XmlAttribute] public uint max_use_cnt { get; set; }
  }
}
