using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class ItemGradeInfo
  {
    public IReadOnlyDictionary<uint, string> EffectToGrade { get; set; }
  }

  internal class CombineElementInfo
  {
    public IReadOnlyList<CombineElementEntry> Entries { get; set; }
  }

  internal class CombineElementEntry
  {
    public uint ItemKey { get; set; }
    public int UiSlot { get; set; }
    public bool Use { get; set; }
  }

  internal class DecompositionElementInfo
  {
    public IReadOnlyList<DecompositionElementEntry> Entries { get; set; }
  }

  internal class DecompositionElementEntry
  {
    public uint ItemKey { get; set; }
    public int UiSlot { get; set; }
  }
}
