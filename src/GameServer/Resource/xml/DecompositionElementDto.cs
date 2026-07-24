using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "element")]
  public class DecompositionElementDto
  {
    [XmlElement("value")] public DecompositionElementValueDto[] Values { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class DecompositionElementValueDto
  {
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public int ui_slot { get; set; }
  }
}
