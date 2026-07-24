using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "STADIUM_INFO")]
  public class StadiumInfoDto
  {
    [XmlElement("MAP_INFO")] public StadiumMapInfoDto[] MapInfos { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class StadiumMapInfoDto
  {
    [XmlAttribute] public int MAPID { get; set; }
    [XmlAttribute] public int MODE { get; set; }
    [XmlElement("BLAST_INFO")] public StadiumBlastInfoDto[] BlastInfos { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class StadiumBlastInfoDto
  {
    [XmlAttribute] public int INDEX { get; set; }
    [XmlAttribute] public string NAME { get; set; }
    [XmlAttribute] public int DEFAULT_HP { get; set; }
    [XmlAttribute] public int MIN { get; set; }
    [XmlAttribute] public int MAX { get; set; }
    [XmlAttribute] public int INC_HP { get; set; }
    [XmlAttribute] public int USE_POINT { get; set; }
    [XmlAttribute] public string RESOURSE_PATH { get; set; }
  }
}
