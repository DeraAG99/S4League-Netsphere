using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "card_system_info")]
  public class CardSystemResourceDto
  {
    [XmlAttribute] public bool active { get; set; }

    [XmlElement("current_season")] public CardSystemCurrentSeasonDto CurrentSeason { get; set; }

    [XmlElement("season")] public CardSystemSeasonDto[] Seasons { get; set; }

    [XmlElement("formula")] public CardSystemFormulaDto Formula { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class CardSystemCurrentSeasonDto
  {
    [XmlAttribute] public int num { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class CardSystemSeasonDto
  {
    [XmlAttribute] public int num { get; set; }

    [XmlAttribute] public uint buy_capsule { get; set; }

    [XmlAttribute] public int shop_id { get; set; }

    [XmlElement("card")] public CardSystemCardDto[] Cards { get; set; }

    [XmlElement("reward")] public CardSystemRewardDto Reward { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class CardSystemCardDto
  {
    [XmlAttribute] public int num { get; set; }

    [XmlAttribute] public uint item_id { get; set; }

    [XmlAttribute] public int shop_id { get; set; }

    [XmlAttribute] public string period_type { get; set; }

    [XmlAttribute] public int period_value { get; set; }

    [XmlAttribute] public byte color { get; set; }

    [XmlAttribute] public uint effect_id { get; set; }

    [XmlAttribute] public int grade { get; set; }

    [XmlAttribute] public int play_prob { get; set; }

    [XmlAttribute] public int try_prob { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class CardSystemRewardDto
  {
    [XmlAttribute] public uint item_id { get; set; }

    [XmlAttribute] public int shop_id { get; set; }

    [XmlAttribute] public string period_type { get; set; }

    [XmlAttribute] public int period_value { get; set; }

    [XmlAttribute] public byte color { get; set; }

    [XmlAttribute] public uint effect_id { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class CardSystemFormulaDto
  {
    [XmlAttribute] public int play_limit_time { get; set; }

    [XmlAttribute] public int play_limit_min_count { get; set; }

    [XmlAttribute] public int play_default_time { get; set; }

    [XmlAttribute] public int play_default_count { get; set; }

    [XmlAttribute] public int gamble_pen { get; set; }

    [XmlAttribute] public int gamble_limit_min_count { get; set; }

    [XmlAttribute] public int complete_card_count { get; set; }
  }
}
