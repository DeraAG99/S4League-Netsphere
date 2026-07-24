using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "enchant_LIST")]
  public class EnchantListDto
  {
    [XmlElement("enchant_renewal")] public EnchantRenewalDto[] Entries { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class EnchantRenewalDto
  {
    [XmlAttribute] public int main_type { get; set; }
    [XmlAttribute] public int upper { get; set; }
    [XmlAttribute] public int lower { get; set; }
    [XmlAttribute] public int EFFECT_TYPE { get; set; }
    [XmlAttribute] public int EFFECT_CONDITION { get; set; }
    [XmlAttribute] public int effect_level { get; set; }
    [XmlAttribute] public int select_prob { get; set; }
    [XmlAttribute] public string effect_key { get; set; }
    [XmlAttribute] public float Value_Min { get; set; }
    [XmlAttribute] public float Value_max { get; set; }
    [XmlAttribute] public float Rate_min { get; set; }
    [XmlAttribute] public float Rate_Max { get; set; }
    [XmlAttribute] public float Value_time { get; set; }
    [XmlAttribute] public int POSITION { get; set; }
    [XmlAttribute] public string Text_KEY { get; set; }
  }
}
