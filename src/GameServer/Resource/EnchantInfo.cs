using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class EnchantInfo
  {
    public float MasteryPerMin { get; set; }
    public int BonusProb { get; set; }
    public int ProbUnit { get; set; }
    public int NoticeEnchantCount { get; set; }
    public IReadOnlyDictionary<string, IReadOnlyList<EnchantMasteryNeed>> MasteryNeeds { get; set; }
    public IReadOnlyDictionary<string, IReadOnlyList<EnchantPrice>> Prices { get; set; }
  }

  internal class EnchantMasteryNeed
  {
    public string ItemType { get; set; }
    public int EnchantCount { get; set; }
    public int Durability { get; set; }
    public int Period { get; set; }
  }

  internal class EnchantPrice
  {
    public string ItemType { get; set; }
    public int EnchantCount { get; set; }
    public int Price { get; set; }
  }

  internal class EnchantEffect
  {
    public int MainType { get; set; }
    public int Upper { get; set; }
    public int Lower { get; set; }
    public int EffectType { get; set; }
    public int EffectCondition { get; set; }
    public int EffectLevel { get; set; }
    public int SelectProbability { get; set; }
    public string EffectKey { get; set; }
    public float ValueMin { get; set; }
    public float ValueMax { get; set; }
    public float RateMin { get; set; }
    public float RateMax { get; set; }
    public float ValueTime { get; set; }
    public int Position { get; set; }
  }

  internal class EnchantExtractEntry
  {
    public uint Key { get; set; }
    public int ExtractingKey { get; set; }
  }

  internal class EsperEnchantPriceEntry
  {
    public int Index { get; set; }
    public int Price { get; set; }
    public int SuccessProbability { get; set; }
    public int FailKeep { get; set; }
    public int FailDown { get; set; }
    public int FailDestruction { get; set; }
  }
}
