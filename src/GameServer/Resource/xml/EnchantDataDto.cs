using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "enchant_data")]
  public class EnchantDataDto
  {
    [XmlElement("enchant_config")] public EnchantConfigDto Config { get; set; }
    [XmlElement("mastery_need_table")] public EnchantMasteryNeedTableDto MasteryTable { get; set; }
    [XmlElement("enchant_price_table")] public EnchantPriceTableDto PriceTable { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class EnchantConfigDto
  {
    [XmlElement("data")] public EnchantConfigDataDto Data { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class EnchantConfigDataDto
  {
    [XmlAttribute] public float mastery_per_min { get; set; }
    [XmlAttribute] public int bonus_prob { get; set; }
    [XmlAttribute] public int prob_unit { get; set; }
    [XmlAttribute] public int notice_enchant_cnt { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class EnchantMasteryNeedTableDto
  {
    [XmlElement("mastery_need")] public EnchantMasteryNeedDto[] Entries { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class EnchantMasteryNeedDto
  {
    [XmlAttribute] public string item_type { get; set; }
    [XmlAttribute] public int enchant_cnt { get; set; }
    [XmlAttribute] public int durability { get; set; }
    [XmlAttribute] public int period { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class EnchantPriceTableDto
  {
    [XmlElement("enchant_price")] public EnchantPriceDto[] Entries { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class EnchantPriceDto
  {
    [XmlAttribute] public string item_type { get; set; }
    [XmlAttribute] public int enchant_cnt { get; set; }
    [XmlAttribute] public int enchant_price { get; set; }
  }
}
