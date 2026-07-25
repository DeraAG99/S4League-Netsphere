using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "room_option")]
  public class RoomOptionRootDto
  {
    [XmlElement("random_room_mode")] public RoomOptionDto Mode { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class RoomOptionDto
  {
    [XmlAttribute] public int reward_condition_time { get; set; }
    [XmlElement("mode_type")] public RoomOptionModeTypeDto ModeType { get; set; }
    [XmlElement("mode_reward")] public RoomOptionRewardDto[] ModeRewards { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class RoomOptionModeTypeDto
  {
    [XmlElement("mode")] public RoomOptionModeEntryDto[] Modes { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class RoomOptionModeEntryDto
  {
    [XmlAttribute] public int mode_id { get; set; }
    [XmlAttribute] public int prob { get; set; }
    [XmlAttribute] public int score { get; set; }
    [XmlAttribute] public int time { get; set; }
    [XmlAttribute] public int limit_player { get; set; }
    [XmlAttribute] public int spectator_count { get; set; }
    [XmlAttribute] public int limit_play_time { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class RoomOptionRewardDto
  {
    [XmlAttribute] public int min_player { get; set; }
    [XmlElement("requital")] public RoomOptionRequitalDto[] Requitals { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class RoomOptionRequitalDto
  {
    [XmlAttribute] public int key { get; set; }
    [XmlAttribute] public string gift_type { get; set; }
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public int shop_id { get; set; }
    [XmlAttribute] public string period_type { get; set; }
    [XmlAttribute] public int period { get; set; }
    [XmlAttribute] public int color { get; set; }
    [XmlAttribute] public int effect_id { get; set; }
    [XmlAttribute] public int prob { get; set; }
  }
}
