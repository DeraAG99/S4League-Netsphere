using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "modeInfo")]
  public class SeizeModeNewInfoDto
  {
    [XmlElement("foothold")] public SeizeFootholdDto Foothold { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class SeizeFootholdDto
  {
    [XmlElement("base")] public SeizeFootholdBaseDto Base { get; set; }
    [XmlElement("score")] public SeizeFootholdScoreDto Score { get; set; }
    [XmlElement("upkeep")] public SeizeFootholdUpkeepDto Upkeep { get; set; }
    [XmlElement("time_bonus")] public SeizeFootholdTimeBonusDto TimeBonus { get; set; }
    [XmlElement("chapture_bonus")] public SeizeFootholdCaptureBonusDto CaptureBonus { get; set; }
    [XmlElement("random_bonus")] public SeizeFootholdRandomBonusDto RandomBonus { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class SeizeFootholdBaseDto
  {
    [XmlAttribute] public int gauge { get; set; }
    [XmlAttribute] public int gauge_up_delay { get; set; }
    [XmlAttribute] public int core_footholder { get; set; }
    [XmlAttribute] public int assist_footholder { get; set; }
    [XmlAttribute] public bool reset { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class SeizeFootholdScoreDto
  {
    [XmlAttribute] public int point { get; set; }
    [XmlAttribute] public int assist_point { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class SeizeFootholdUpkeepDto
  {
    [XmlAttribute] public bool actived { get; set; }
    [XmlAttribute] public int delay { get; set; }
    [XmlAttribute] public int score { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class SeizeFootholdTimeBonusDto
  {
    [XmlAttribute] public bool actived { get; set; }
    [XmlAttribute] public int delay { get; set; }
    [XmlAttribute] public int default_bonus { get; set; }
    [XmlAttribute] public int add_bonus { get; set; }
    [XmlAttribute] public int add_bonus_limit { get; set; }
    [XmlAttribute] public int default_assist_bonus { get; set; }
    [XmlAttribute] public int add_assist_bonus { get; set; }
    [XmlAttribute] public bool reset { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class SeizeFootholdCaptureBonusDto
  {
    [XmlAttribute] public bool actived { get; set; }
    [XmlAttribute] public int default_bonus { get; set; }
    [XmlAttribute] public int add_bonus { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class SeizeFootholdRandomBonusDto
  {
    [XmlAttribute] public bool actived { get; set; }
    [XmlAttribute] public int start_time { get; set; }
    [XmlAttribute] public int start_rand_time { get; set; }
    [XmlAttribute] public int delay { get; set; }
    [XmlAttribute] public int bonus { get; set; }
  }
}
