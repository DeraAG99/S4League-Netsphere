using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false, ElementName = "mission_info")]
  public class MissionInfoDto
  {
    [XmlElement("mission_config")] public MissionInfoMissionConfigDto mission_config { get; set; }
    [XmlElement("daily_pvp_mission")] public MissionInfoDailyPvpMissionDto[] daily_pvp_mission { get; set; }
    [XmlElement("daily_pve_mission")] public MissionInfoDailyPveMissionDto[] daily_pve_mission { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MissionInfoMissionConfigDto
  {
    [XmlAttribute] public uint mission_check_interval { get; set; }
    [XmlAttribute] public uint max_mission_count { get; set; }
    [XmlAttribute] public uint mission_reward_mail_expire_days { get; set; }
    [XmlAttribute] public uint daily_pvp_mission_count { get; set; }
    [XmlAttribute] public uint daily_pve_mission_count { get; set; }
    [XmlAttribute] public uint daily_mission_reward_mail_expire_days { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MissionInfoDailyPvpMissionDto
  {
    [XmlElement("mission_condition")] public MissionInfoMissionConditionDto[] mission_condition { get; set; }
    [XmlElement("mission_reward")] public MissionInfoMissionRewardDto[] mission_reward { get; set; }

    [XmlAttribute] public uint id { get; set; }
    [XmlAttribute] public string name_key { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MissionInfoDailyPveMissionDto
  {
    [XmlElement("mission_condition")] public MissionInfoMissionConditionDto[] mission_condition { get; set; }
    [XmlElement("mission_reward")] public MissionInfoMissionRewardDto[] mission_reward { get; set; }

    [XmlAttribute] public uint id { get; set; }
    [XmlAttribute] public string name_key { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MissionInfoMissionConditionDto
  {
    [XmlAttribute] public uint condition_type { get; set; }
    [XmlAttribute] public uint condition_value { get; set; }
    [XmlAttribute] public uint map_id { get; set; }
    [XmlAttribute] public string game_type { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MissionInfoMissionRewardDto
  {
    [XmlAttribute] public uint reward_type { get; set; }
    [XmlAttribute] public uint reward_value { get; set; }
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public uint shop_id { get; set; }
    [XmlAttribute] public string period_type { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effect_id { get; set; }
  }
}
