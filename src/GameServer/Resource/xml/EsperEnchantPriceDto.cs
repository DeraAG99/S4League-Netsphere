using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "RENEWAL_ENCHANT_PRICE")]
  public class EsperEnchantPriceDto
  {
    [XmlElement("ENCHANT_PRICE")] public EsperEnchantPriceEntryDto[] Entries { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class EsperEnchantPriceEntryDto
  {
    [XmlAttribute] public int INDEX { get; set; }
    [XmlAttribute] public int PRICE { get; set; }
    [XmlAttribute] public int SUCCESS_PROB { get; set; }
    [XmlAttribute] public int FAIL_KEEP { get; set; }
    [XmlAttribute] public int FAIL_DOWN { get; set; }
    [XmlAttribute] public int FAIL_DESTRUCTION { get; set; }
  }
}
