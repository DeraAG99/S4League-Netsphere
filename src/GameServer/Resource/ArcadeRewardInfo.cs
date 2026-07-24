using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class ArcadeRewardInfo
  {
    public IReadOnlyList<ArcadeRewardGrade> Grades { get; set; }
    public IReadOnlyList<ArcadeRewardItem> Items { get; set; }
  }

  internal class ArcadeRewardGrade
  {
    public uint Grade { get; set; }
    public string NameKey { get; set; }
    public uint MinScore { get; set; }
    public uint MaxScore { get; set; }
  }

  internal class ArcadeRewardItem
  {
    public uint MapId { get; set; }
    public uint Difficulty { get; set; }
    public uint Grade { get; set; }
    public uint Category { get; set; }
    public uint SubCategory { get; set; }
    public uint ItemNumber { get; set; }
    public uint ProductNumber { get; set; }
    public uint Probability { get; set; }
    public uint MinScore { get; set; }
    public uint MaxScore { get; set; }
  }
}
