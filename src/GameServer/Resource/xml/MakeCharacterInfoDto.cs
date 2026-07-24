using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false, ElementName = "make_character_info")]
  public class MakeCharacterInfoDto
  {
    [XmlElement("background")] public MakeCharacterInfoBackgroundDto background { get; set; }
    [XmlElement("character")] public MakeCharacterInfoCharacterDto character { get; set; }
    [XmlElement("camera")] public MakeCharacterInfoCameraDto camera { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoBackgroundDto
  {
    [XmlElement("file")] public MakeCharacterInfoBackgroundFileDto[] file { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoBackgroundFileDto
  {
    [XmlAttribute] public string path { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoCharacterDto
  {
    [XmlElement("position")] public MakeCharacterInfoPositionDto position { get; set; }
    [XmlElement("rotate")] public MakeCharacterInfoRotateDto rotate { get; set; }
    [XmlElement("default")] public MakeCharacterInfoDefaultDto @default { get; set; }
    [XmlElement("weapons")] public MakeCharacterInfoWeaponsDto weapons { get; set; }
    [XmlElement("skills")] public MakeCharacterInfoSkillsDto skills { get; set; }
    [XmlElement("costumes")] public MakeCharacterInfoCostumesDto costumes { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoPositionDto
  {
    [XmlAttribute] public float x { get; set; }
    [XmlAttribute] public float y { get; set; }
    [XmlAttribute] public float z { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoRotateDto
  {
    [XmlAttribute] public float y { get; set; }
    [XmlAttribute] public float speed { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoDefaultDto
  {
    [XmlElement("gender")] public MakeCharacterInfoGenderDto gender { get; set; }
    [XmlElement("costume")] public MakeCharacterInfoCostumeDto costume { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoGenderDto
  {
    [XmlAttribute] public uint value { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoCostumeDto
  {
    [XmlElement("male")] public MakeCharacterInfoCostumeGenderDto male { get; set; }
    [XmlElement("female")] public MakeCharacterInfoCostumeGenderDto female { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoCostumeGenderDto
  {
    [XmlElement("hair")] public MakeCharacterInfoCostumePartDto hair { get; set; }
    [XmlElement("face")] public MakeCharacterInfoCostumePartDto face { get; set; }
    [XmlElement("coat")] public MakeCharacterInfoCostumePartDto coat { get; set; }
    [XmlElement("pants")] public MakeCharacterInfoCostumePartDto pants { get; set; }
    [XmlElement("gloves")] public MakeCharacterInfoCostumePartDto gloves { get; set; }
    [XmlElement("shoes")] public MakeCharacterInfoCostumePartDto shoes { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoCostumePartDto
  {
    [XmlAttribute] public uint itemid { get; set; }
    [XmlAttribute] public uint variation { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoWeaponsDto
  {
    [XmlElement("weapon")] public MakeCharacterInfoWeaponDto[] weapon { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoWeaponDto
  {
    [XmlAttribute] public uint itemid { get; set; }
    [XmlAttribute] public uint shopid { get; set; }
    [XmlAttribute] public string periodtype { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effectid { get; set; }
    [XmlAttribute] public uint slot { get; set; }
    [XmlAttribute] public bool equip { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoSkillsDto
  {
    [XmlElement("skill")] public MakeCharacterInfoSkillDto[] skill { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoSkillDto
  {
    [XmlAttribute] public uint itemid { get; set; }
    [XmlAttribute] public uint shopid { get; set; }
    [XmlAttribute] public string periodtype { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effectid { get; set; }
    [XmlAttribute] public bool equip { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoCostumesDto
  {
    [XmlElement("male")] public MakeCharacterInfoCostumeSetDto male { get; set; }
    [XmlElement("female")] public MakeCharacterInfoCostumeSetDto female { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoCostumeSetDto
  {
    [XmlElement("costume")] public MakeCharacterInfoCostumeSetItemDto[] costume { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoCostumeSetItemDto
  {
    [XmlElement("hair")] public MakeCharacterInfoCostumeSetPartDto hair { get; set; }
    [XmlElement("face")] public MakeCharacterInfoCostumeSetPartDto face { get; set; }
    [XmlElement("coat")] public MakeCharacterInfoCostumeSetPartDto coat { get; set; }
    [XmlElement("pants")] public MakeCharacterInfoCostumeSetPartDto pants { get; set; }
    [XmlElement("gloves")] public MakeCharacterInfoCostumeSetPartDto gloves { get; set; }
    [XmlElement("shoes")] public MakeCharacterInfoCostumeSetPartDto shoes { get; set; }

    [XmlAttribute] public string active_icon { get; set; }
    [XmlAttribute] public string unactive_icon { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoCostumeSetPartDto
  {
    [XmlAttribute] public uint itemid { get; set; }
    [XmlAttribute] public uint variation { get; set; }
    [XmlAttribute] public uint shopid { get; set; }
    [XmlAttribute] public string periodtype { get; set; }
    [XmlAttribute] public uint period { get; set; }
    [XmlAttribute] public uint color { get; set; }
    [XmlAttribute] public uint effectid { get; set; }
    [XmlAttribute] public bool provide { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoCameraDto
  {
    [XmlElement("lookat")] public MakeCharacterInfoCameraLookatDto lookat { get; set; }
    [XmlElement("rotate")] public MakeCharacterInfoCameraRotateDto rotate { get; set; }
    [XmlElement("dist")] public MakeCharacterInfoCameraDistDto dist { get; set; }
    [XmlElement("speed")] public MakeCharacterInfoCameraSpeedDto speed { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoCameraLookatDto
  {
    [XmlAttribute] public float x { get; set; }
    [XmlAttribute] public float y { get; set; }
    [XmlAttribute] public float z { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoCameraRotateDto
  {
    [XmlAttribute] public float x { get; set; }
    [XmlAttribute] public float y { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoCameraDistDto
  {
    [XmlAttribute] public float @default { get; set; }
    [XmlAttribute] public float min { get; set; }
    [XmlAttribute] public float max { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class MakeCharacterInfoCameraSpeedDto
  {
    [XmlAttribute] public float move { get; set; }
  }
}
