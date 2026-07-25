using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "burningsystem")]
  public class BurningTimeDto
  {
    [XmlElement("mode")] public BurningTimeModeDto[] Modes { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class BurningTimeModeDto
  {
    [XmlAttribute] public uint mode { get; set; }
    [XmlElement("condition")] public BurningTimeConditionDto Condition { get; set; }
    [XmlElement("status")] public BurningTimeStatusDto Status { get; set; }
    [XmlElement("country")] public BurningTimeCountryDto Country { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class BurningTimeConditionDto
  {
    [XmlAttribute] public int ca_lv_min { get; set; }
    [XmlAttribute] public int ca_lv_max { get; set; }
    [XmlAttribute] public int point { get; set; }
    [XmlAttribute] public int burning_time { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class BurningTimeStatusDto
  {
    [XmlAttribute] public float multi_ap { get; set; }
    [XmlAttribute] public float plus_as { get; set; }
    [XmlAttribute] public string us { get; set; }
    [XmlAttribute] public int av_hp { get; set; }
    [XmlAttribute] public float multi_dp { get; set; }
    [XmlAttribute] public int av_sp { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class BurningTimeCountryDto
  {
    [XmlAttribute] public string kr { get; set; }
    [XmlAttribute] public string eu { get; set; }
    [XmlAttribute] public string cn { get; set; }
    [XmlAttribute] public string th { get; set; }
    [XmlAttribute] public string tw { get; set; }
    [XmlAttribute] public string jp { get; set; }
    [XmlAttribute] public string id { get; set; }
  }
}
