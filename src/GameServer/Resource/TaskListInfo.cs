using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class TaskListInfo
  {
    public IReadOnlyList<TaskEntry> CompulsoryTasks { get; set; }
    public IReadOnlyList<TaskEntry> WeeklyTasks { get; set; }
    public IReadOnlyList<TaskEntry> OptionalTasks { get; set; }
  }

  internal class TaskEntry
  {
    public string NameKey { get; set; }
    public string ModeType { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
    public IReadOnlyList<TaskLevelSetting> LevelSettings { get; set; }
  }

  internal class TaskLevelSetting
  {
    public uint Id { get; set; }
    public uint Level { get; set; }
    public uint ChanceValue { get; set; }
    public uint AddChanceValue { get; set; }
    public uint AddChanceLimitLevel { get; set; }
    public uint GamePlayTimeSeconds { get; set; }
    public uint GoalOfMatch { get; set; }
    public uint Repetition { get; set; }
    public string CheckerType { get; set; }
    public string CheckerData { get; set; }
    public uint PenReward { get; set; }
    public uint ExPenReward { get; set; }
  }
}
