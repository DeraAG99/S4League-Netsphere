using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false, ElementName = "task")]
  public class TaskListInfoDto
  {
    [XmlElement("ex_pensetup")] public TaskListInfoExPensetupDto ex_pensetup { get; set; }
    [XmlElement("compulsory_task")] public TaskListInfoCompulsoryTaskDto compulsory_task { get; set; }
    [XmlElement("weekly_task")] public TaskListInfoWeeklyTaskDto weekly_task { get; set; }
    [XmlElement("optional_task")] public TaskListInfoOptionalTaskDto optional_task { get; set; }

    [XmlAttribute] public string string_table { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoExPensetupDto
  {
    [XmlElement("set")] public TaskListInfoExPensetupSetDto[] set { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoExPensetupSetDto
  {
    [XmlAttribute] public string period { get; set; }
    [XmlAttribute] public uint level { get; set; }
    [XmlAttribute] public uint count { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoCompulsoryTaskDto
  {
    [XmlElement("base_setting")] public TaskListInfoBaseSettingDto[] base_setting { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoWeeklyTaskDto
  {
    [XmlElement("base_setting")] public TaskListInfoBaseSettingDto[] base_setting { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoOptionalTaskDto
  {
    [XmlElement("base_setting")] public TaskListInfoBaseSettingDto[] base_setting { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoBaseSettingDto
  {
    [XmlElement("lang")] public TaskListInfoLangDto lang { get; set; }
    [XmlElement("level_setting")] public TaskListInfoLevelSettingDto[] level_setting { get; set; }

    [XmlAttribute] public string name_key { get; set; }
    [XmlAttribute] public string mode_type { get; set; }
    [XmlAttribute] public string category { get; set; }
    [XmlAttribute] public string name { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoLangDto
  {
    [XmlElement("nation")] public TaskListInfoNationDto[] nation { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoNationDto
  {
    [XmlAttribute] public uint id { get; set; }
    [XmlAttribute] public string name_code { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoLevelSettingDto
  {
    [XmlElement("select_condition")] public TaskListInfoSelectConditionDto select_condition { get; set; }
    [XmlElement("complet_condition")] public TaskListInfoCompletConditionDto complet_condition { get; set; }
    [XmlElement("reward")] public TaskListInfoRewardDto reward { get; set; }
    [XmlElement("help_massage")] public TaskListInfoHelpMassageDto help_massage { get; set; }

    [XmlAttribute] public uint id { get; set; }
    [XmlAttribute] public uint level { get; set; }
    [XmlAttribute] public uint chance_value { get; set; }
    [XmlAttribute] public uint add_chance_value { get; set; }
    [XmlAttribute] public uint add_chan_limit_lv { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoSelectConditionDto
  {
    [XmlElement("kill_per_death")] public TaskListInfoKillPerDeathDto kill_per_death { get; set; }
    [XmlElement("touch_down_score")] public TaskListInfoTouchDownScoreDto touch_down_score { get; set; }
    [XmlElement("have_license")] public TaskListInfoHaveLicenseDto have_license { get; set; }
    [XmlElement("have_weapon")] public TaskListInfoHaveWeaponDto have_weapon { get; set; }
    [XmlElement("min_level")] public TaskListInfoMinLevelDto min_level { get; set; }
    [XmlElement("max_level")] public TaskListInfoMaxLevelDto max_level { get; set; }
    [XmlElement("exp_rate")] public TaskListInfoExpRateDto exp_rate { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoKillPerDeathDto
  {
    [XmlAttribute] public string value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoTouchDownScoreDto
  {
    [XmlAttribute] public string value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoHaveLicenseDto
  {
    [XmlAttribute] public string value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoHaveWeaponDto
  {
    [XmlAttribute] public string value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoMinLevelDto
  {
    [XmlAttribute] public string value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoMaxLevelDto
  {
    [XmlAttribute] public string value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoExpRateDto
  {
    [XmlAttribute] public string value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoCompletConditionDto
  {
    [XmlElement("game_play_ts")] public TaskListInfoGamePlayTsDto game_play_ts { get; set; }
    [XmlElement("number_of_team_person")] public TaskListInfoNumberOfTeamPersonDto number_of_team_person { get; set; }
    [XmlElement("goal_of_match")] public TaskListInfoGoalOfMatchDto goal_of_match { get; set; }
    [XmlElement("repetetion")] public TaskListInfoRepetetionDto repetetion { get; set; }
    [XmlElement("checker_type")] public TaskListInfoCheckerTypeDto checker_type { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoGamePlayTsDto
  {
    [XmlAttribute] public uint value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoNumberOfTeamPersonDto
  {
    [XmlAttribute] public uint value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoGoalOfMatchDto
  {
    [XmlAttribute] public uint value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoRepetetionDto
  {
    [XmlAttribute] public uint value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoCheckerTypeDto
  {
    [XmlAttribute] public string value { get; set; }
    [XmlAttribute] public string data { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoRewardDto
  {
    [XmlElement("pen")] public TaskListInfoPenDto pen { get; set; }
    [XmlElement("ex_pen")] public TaskListInfoExPenDto ex_pen { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoPenDto
  {
    [XmlAttribute] public uint value { get; set; }
    [XmlAttribute] public uint chance_value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoExPenDto
  {
    [XmlAttribute] public uint value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoHelpMassageDto
  {
    [XmlElement("massage01")] public TaskListInfoMassage01Dto massage01 { get; set; }
    [XmlElement("massage02")] public TaskListInfoMassage02Dto massage02 { get; set; }
    [XmlElement("lang")] public TaskListInfoHelpMassageLangDto lang { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoMassage01Dto
  {
    [XmlAttribute] public string string_key { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoMassage02Dto
  {
    [XmlAttribute] public string string_key { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoHelpMassageLangDto
  {
    [XmlElement("nation")] public TaskListInfoHelpMassageNationDto[] nation { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class TaskListInfoHelpMassageNationDto
  {
    [XmlAttribute] public uint id { get; set; }
    [XmlAttribute] public string massage01_code { get; set; }
    [XmlAttribute] public string massage02_code { get; set; }
  }
}
