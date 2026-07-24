using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false, ElementName = "promotion_info")]
  public class PromotionInfoDto
  {
    [XmlElement("eventbaseInfo")] public PromotionInfoEventbaseInfoDto eventbaseInfo { get; set; }
    [XmlElement("event_notice")] public PromotionInfoEventNoticeDto event_notice { get; set; }
    [XmlElement("event_system")] public PromotionInfoEventSystemDto event_system { get; set; }
    [XmlElement("roulette_machine")] public PromotionInfoRouletteMachineDto roulette_machine { get; set; }
    [XmlElement("x_mas_card_event")] public PromotionInfoXMasCardEventDto x_mas_card_event { get; set; }
    [XmlElement("new_year_event")] public PromotionInfoNewYearEventDto[] new_year_event { get; set; }
    [XmlElement("recommend_event")] public PromotionInfoRecommendEventDto recommend_event { get; set; }
    [XmlElement("daily_attendance")] public PromotionInfoDailyAttendanceDto daily_attendance { get; set; }
    [XmlElement("daily_gift")] public PromotionInfoDailyGiftDto daily_gift { get; set; }
    [XmlElement("daily_play_time")] public PromotionInfoDailyPlayTimeDto daily_play_time { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoEventbaseInfoDto
  {
    [XmlElement("guide")] public PromotionInfoGuideDto guide { get; set; }
    [XmlElement("sound")] public PromotionInfoSoundDto sound { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoGuideDto
  {
    [XmlElement("event")] public PromotionInfoGuideEventDto[] @event { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoGuideEventDto
  {
    [XmlAttribute] public string @string { get; set; }
    [XmlAttribute] public string image { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoSoundDto
  {
    [XmlAttribute] public string ingame_coin_drop { get; set; }
    [XmlAttribute] public string ingame_coin_get { get; set; }
    [XmlAttribute] public string rolling_coin { get; set; }
    [XmlAttribute] public string rolling_coin_success { get; set; }
    [XmlAttribute] public string rolling_coin_faild { get; set; }
    [XmlAttribute] public string attend_item_get { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoEventNoticeDto
  {
    [XmlElement("notice")] public PromotionInfoNoticeDto[] notice { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoNoticeDto
  {
    [XmlAttribute] public string key { get; set; }
    [XmlAttribute] public uint repeat_period { get; set; }
    [XmlAttribute] public bool active { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoEventSystemDto
  {
    [XmlElement("event_info")] public PromotionInfoEventInfoDto[] event_info { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoEventInfoDto
  {
    [XmlAttribute] public uint event_type { get; set; }
    [XmlAttribute] public bool active { get; set; }
    [XmlAttribute] public string event_title { get; set; }
    [XmlAttribute] public uint reward_type { get; set; }
    [XmlAttribute] public uint event_state { get; set; }
    [XmlAttribute] public uint event_attribute { get; set; }
    [XmlAttribute] public uint min_player { get; set; }
    [XmlAttribute] public uint min_time { get; set; }
    [XmlAttribute] public uint channel_id { get; set; }
    [XmlAttribute] public uint map_id { get; set; }
    [XmlAttribute] public uint game_mode { get; set; }
    [XmlAttribute] public uint min_score { get; set; }
    [XmlAttribute] public string gift_type { get; set; }
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public uint shop_id { get; set; }
    [XmlAttribute] public string period_type { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effect_id { get; set; }
    [XmlAttribute] public uint prob { get; set; }
    [XmlAttribute] public uint reward_item_limit_cnt { get; set; }
    [XmlAttribute] public string Item_string_key { get; set; }
    [XmlAttribute] public string lose_gift_type { get; set; }
    [XmlAttribute] public uint lose_item_key { get; set; }
    [XmlAttribute] public uint lose_shop_id { get; set; }
    [XmlAttribute] public string lose_period_type { get; set; }
    [XmlAttribute] public uint lose_period { get; set; }
    [XmlAttribute] public uint lose_color { get; set; }
    [XmlAttribute] public uint lose_effect_id { get; set; }
    [XmlAttribute] public uint lose_prob { get; set; }
    [XmlAttribute] public uint lose_reward_item_limit_cnt { get; set; }
    [XmlAttribute] public string lose_Item_string_key { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoRouletteMachineDto
  {
    [XmlElement("event_info")] public PromotionInfoRouletteEventInfoDto[] event_info { get; set; }

    [XmlAttribute] public bool active { get; set; }
    [XmlAttribute] public bool active_clientGuide { get; set; }
    [XmlAttribute] public uint use_item_key { get; set; }
    [XmlAttribute] public uint use_item_cnt { get; set; }
    [XmlAttribute] public string event_title { get; set; }
    [XmlAttribute] public uint reward_type { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoRouletteEventInfoDto
  {
    [XmlAttribute] public uint event_type { get; set; }
    [XmlAttribute] public string gift_type { get; set; }
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public uint shop_id { get; set; }
    [XmlAttribute] public string period_type { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effect_id { get; set; }
    [XmlAttribute] public uint prob { get; set; }
    [XmlAttribute] public string Item_string_key { get; set; }
    [XmlAttribute] public string result_image { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoXMasCardEventDto
  {
    [XmlElement("event_info")] public PromotionInfoXMasCardEventInfoDto[] event_info { get; set; }

    [XmlAttribute] public bool active { get; set; }
    [XmlAttribute] public bool active_clientGuide { get; set; }
    [XmlAttribute] public uint use_item_key { get; set; }
    [XmlAttribute] public uint use_item_cnt { get; set; }
    [XmlAttribute] public string event_title { get; set; }
    [XmlAttribute] public uint reward_type { get; set; }
    [XmlAttribute] public uint unique1 { get; set; }
    [XmlAttribute] public uint unique2 { get; set; }
    [XmlAttribute] public uint unique3 { get; set; }
    [XmlAttribute] public uint reward_delay { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoXMasCardEventInfoDto
  {
    [XmlAttribute] public uint event_type { get; set; }
    [XmlAttribute] public string gift_type { get; set; }
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public uint shop_id { get; set; }
    [XmlAttribute] public string period_type { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effect_id { get; set; }
    [XmlAttribute] public uint prob { get; set; }
    [XmlAttribute] public uint reward_item_limit_cnt { get; set; }
    [XmlAttribute] public string Item_string_key { get; set; }
    [XmlAttribute] public string event_title { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoNewYearEventDto
  {
    [XmlElement("event_info")] public PromotionInfoNewYearEventInfoDto[] event_info { get; set; }

    [XmlAttribute] public bool active { get; set; }
    [XmlAttribute] public bool active_clientGuide { get; set; }
    [XmlAttribute] public uint use_item_key { get; set; }
    [XmlAttribute] public string use_item_cnt { get; set; }
    [XmlAttribute] public string event_title { get; set; }
    [XmlAttribute] public uint reward_type { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoNewYearEventInfoDto
  {
    [XmlAttribute] public uint event_type { get; set; }
    [XmlAttribute] public string gift_type { get; set; }
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public uint shop_id { get; set; }
    [XmlAttribute] public string period_type { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effect_id { get; set; }
    [XmlAttribute] public uint prob { get; set; }
    [XmlAttribute] public string Item_string_key { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoRecommendEventDto
  {
    [XmlElement("requital")] public PromotionInfoRequitalDto[] requital { get; set; }

    [XmlAttribute] public bool active { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoRequitalDto
  {
    [XmlAttribute] public uint key { get; set; }
    [XmlAttribute] public string gift_type { get; set; }
    [XmlAttribute] public uint gift_value { get; set; }
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public uint shop_id { get; set; }
    [XmlAttribute] public string period_type { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effect_id { get; set; }
    [XmlAttribute] public uint prob { get; set; }
    [XmlAttribute] public string Item_string_key { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoDailyAttendanceDto
  {
    [XmlElement("daily_item_info")] public PromotionInfoDailyItemInfoDto[] daily_item_info { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoDailyItemInfoDto
  {
    [XmlAttribute] public uint item_index { get; set; }
    [XmlAttribute] public uint user_type { get; set; }
    [XmlAttribute] public uint year { get; set; }
    [XmlAttribute] public uint week { get; set; }
    [XmlAttribute] public uint day_of_week { get; set; }
    [XmlAttribute] public uint item_key { get; set; }
    [XmlAttribute] public uint shop_id { get; set; }
    [XmlAttribute] public string period_type { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effect_id { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoDailyGiftDto
  {
    [XmlElement("requital")] public PromotionInfoRequitalDto[] requital { get; set; }

    [XmlAttribute] public bool active { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class PromotionInfoDailyPlayTimeDto
  {
    [XmlElement("requital")] public PromotionInfoRequitalDto[] requital { get; set; }

    [XmlAttribute] public bool active { get; set; }
  }
}
