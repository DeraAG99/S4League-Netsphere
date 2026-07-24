using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class ArcadeItemInfo
  {
    public IReadOnlyDictionary<uint, ArcadeItemEffect> Effects { get; set; }
  }

  internal class ArcadeItemEffect
  {
    public uint ItemKey { get; set; }
    public uint EffectType { get; set; }
    public uint EffectValue { get; set; }
    public uint EffectRate { get; set; }
    public uint EffectTime { get; set; }
    public uint CooldownTime { get; set; }
    public uint MaxStack { get; set; }
  }
}
