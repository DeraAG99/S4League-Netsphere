using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false, ElementName = "support_item")]
  public class SupportItemInfoDto
  {
    [XmlElement("item")] public SupportItemInfoItemDto[] item { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class SupportItemInfoItemDto
  {
    [XmlAttribute] public uint category { get; set; }
    [XmlAttribute] public uint sub_category { get; set; }
    [XmlAttribute] public uint number { get; set; }
    [XmlAttribute] public uint product { get; set; }
    [XmlAttribute] public uint slot { get; set; }
  }
}
