using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "element")]
  public class CombineElementDto
  {
    [XmlElement("value")] public CombineElementValueDto[] Values { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class CombineElementValueDto
  {
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public int ui_slot { get; set; }
    [XmlAttribute] public string use { get; set; }
  }
}
