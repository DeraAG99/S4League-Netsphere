using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class EquipLimitInfo
  {
    public IReadOnlyDictionary<int, EquipLimitEntry> Entries { get; set; }
  }

  internal class EquipLimitEntry
  {
    public int Id { get; set; }
    public string StringKey { get; set; }
    public HashSet<uint> AllowedItems { get; set; }
  }
}
