using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "experience")]
  public class MasterExperienceDto
  {
    [XmlAttribute] public string string_table { get; set; }
    [XmlAttribute] public int maxLevel { get; set; }
    [XmlElement("exp")] public MasterExperienceEntryDto[] Entries { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MasterExperienceEntryDto
  {
    [XmlAttribute] public int require { get; set; }
    [XmlAttribute] public int accumulate { get; set; }
    [XmlAttribute] public string name_key { get; set; }
  }
}
