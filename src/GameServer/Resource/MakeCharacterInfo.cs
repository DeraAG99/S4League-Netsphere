using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class MakeCharacterInfo
  {
    public IReadOnlyList<MakeCharacterItem> MaleCostumes { get; set; }
    public IReadOnlyList<MakeCharacterItem> FemaleCostumes { get; set; }
    public IReadOnlyList<MakeCharacterWeapon> Weapons { get; set; }
    public IReadOnlyList<MakeCharacterSkill> Skills { get; set; }
    public MakeCharacterGender DefaultGender { get; set; }
  }

  internal class MakeCharacterGender
  {
    public MakeCharacterCostume MaleCostume { get; set; }
    public MakeCharacterCostume FemaleCostume { get; set; }
  }

  internal class MakeCharacterCostume
  {
    public uint HairItemId { get; set; }
    public uint HairVariation { get; set; }
    public uint FaceItemId { get; set; }
    public uint FaceVariation { get; set; }
    public uint CoatItemId { get; set; }
    public uint CoatVariation { get; set; }
    public uint PantsItemId { get; set; }
    public uint PantsVariation { get; set; }
    public uint GlovesItemId { get; set; }
    public uint GlovesVariation { get; set; }
    public uint ShoesItemId { get; set; }
    public uint ShoesVariation { get; set; }
  }

  internal class MakeCharacterItem
  {
    public string Icon { get; set; }
    public MakeCharacterCostume Costume { get; set; }
  }

  internal class MakeCharacterWeapon
  {
    public uint ItemId { get; set; }
    public uint ShopId { get; set; }
    public string PeriodType { get; set; }
    public uint Period { get; set; }
    public uint Color { get; set; }
    public uint EffectId { get; set; }
    public uint Slot { get; set; }
    public bool Equip { get; set; }
  }

  internal class MakeCharacterSkill
  {
    public uint ItemId { get; set; }
    public uint ShopId { get; set; }
    public string PeriodType { get; set; }
    public uint Period { get; set; }
    public uint Color { get; set; }
    public uint EffectId { get; set; }
    public bool Equip { get; set; }
  }
}
