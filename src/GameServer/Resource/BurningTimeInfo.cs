using System.Collections.Generic;
using System.Linq;

namespace NeoNetsphere.Resource
{
  internal class BurningTimeInfo
  {
    public IReadOnlyDictionary<uint, IReadOnlyList<BurningTimeEntry>> Entries { get; set; }

    public BurningTimeEntry GetEntry(uint mode, int level)
    {
      if (!Entries.TryGetValue(mode, out var list))
        return null;
      return list.FirstOrDefault(e => level >= e.LevelMin && level <= e.LevelMax);
    }
  }

  internal class BurningTimeEntry
  {
    public uint Mode { get; set; }
    public int LevelMin { get; set; }
    public int LevelMax { get; set; }
    public int Point { get; set; }
    public int BurningTime { get; set; }
    public float MultiAp { get; set; }
    public int PlusAs { get; set; }
    public int AvHp { get; set; }
    public float MultiDp { get; set; }
    public int AvSp { get; set; }
  }
}
