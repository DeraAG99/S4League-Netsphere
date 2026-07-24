using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class DecompositionInfo
  {
    public uint PenPrice { get; set; }
    public uint MinHours { get; set; }
    public uint MinDays { get; set; }
    public IReadOnlyList<DecompositionMethod> Methods { get; set; }
    public IReadOnlyList<DecompositionBonus> Bonuses { get; set; }
    public IReadOnlyList<uint> ProhibitedItems { get; set; }
  }

  internal class DecompositionMethod
  {
    public string PeriodType { get; set; }
    public uint EffectMinCount { get; set; }
    public uint EffectMaxCount { get; set; }
    public bool Use { get; set; }
    public bool Bonus { get; set; }
    public IReadOnlyList<DecompositionComponent> Components { get; set; }
  }

  internal class DecompositionComponent
  {
    public uint Condition { get; set; }
    public uint ItemKey { get; set; }
    public uint ShopId { get; set; }
    public string PeriodType { get; set; }
    public uint Period { get; set; }
    public uint Color { get; set; }
    public uint EffectId { get; set; }
  }

  internal class DecompositionBonus
  {
    public uint PeriodMultipleValue { get; set; }
    public string ItemMainType { get; set; }
    public string ItemSubType { get; set; }
  }
}
