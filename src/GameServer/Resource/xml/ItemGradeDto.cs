using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "itemgrade")]
  public class ItemGradeDto
  {
    [XmlElement("mode")] public ItemGradeModeDto Mode { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class ItemGradeModeDto
  {
    [XmlElement("condition")] public ItemGradeConditionDto[] Conditions { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class ItemGradeConditionDto
  {
    [XmlAttribute] public uint effect_id { get; set; }
    [XmlAttribute] public string item_grade { get; set; }
  }
}
