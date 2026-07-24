using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class SupportItemInfo
  {
    public IReadOnlyList<SupportItemEntry> Items { get; set; }
  }

  internal class SupportItemEntry
  {
    public uint Category { get; set; }
    public uint SubCategory { get; set; }
    public uint Number { get; set; }
    public uint Product { get; set; }
    public uint Slot { get; set; }
  }
}
