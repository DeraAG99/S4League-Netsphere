using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "enchantchip_table")]
  public class EnchantExtractKeyDto
  {
    [XmlElement("enchant_extraction_serial")] public EnchantExtractionSerialDto[] Entries { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class EnchantExtractionSerialDto
  {
    [XmlAttribute] public uint key { get; set; }
    [XmlAttribute] public int extracting_key { get; set; }
  }
}
