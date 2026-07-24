using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", ElementName = "equip_limit")]
  public class EquipLimitDto
  {
    [XmlElement("preset")] public EquipLimitPresetDto Preset { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class EquipLimitPresetDto
  {
    [XmlElement("limit")] public EquipLimitEntryDto[] Entries { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class EquipLimitEntryDto
  {
    [XmlAttribute] public int id { get; set; }
    [XmlAttribute] public string string_key { get; set; }
    [XmlElement("require_Item")] public EquipLimitItemDto[] Items { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class EquipLimitItemDto
  {
    [XmlAttribute] public uint Item_Id { get; set; }
  }
}
