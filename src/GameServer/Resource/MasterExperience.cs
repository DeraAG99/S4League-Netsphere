namespace NeoNetsphere.Resource
{
  internal class MasterExperience
  {
    public int MaxLevel { get; set; }
    public MasterExperienceEntry[] Entries { get; set; }
  }

  internal class MasterExperienceEntry
  {
    public int Level { get; set; }
    public uint Require { get; set; }
    public uint Accumulate { get; set; }
  }
}
