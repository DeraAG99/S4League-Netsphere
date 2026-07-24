using System.Collections.Generic;

namespace NeoNetsphere.Resource
{
  internal class CombinationInfo
  {
    public uint PenPrice { get; set; }
    public uint MinHours { get; set; }
    public uint MinDays { get; set; }
    public CombinationItem ArgonComponent { get; set; }
    public CombinationItem KryptonComponent { get; set; }
    public CombinationEnchantOption EnchantOption { get; set; }
    public uint OvercountMaxLevel { get; set; }
    public uint OvercountWeightMax { get; set; }
    public IReadOnlyList<CombinationComponent> Components { get; set; }
  }

  internal class CombinationItem
  {
    public uint ItemKey { get; set; }
    public uint ShopId { get; set; }
    public string PeriodType { get; set; }
    public uint Period { get; set; }
    public uint Color { get; set; }
    public uint EffectId { get; set; }
  }

  internal class CombinationEnchantOption
  {
    public uint EnchantItemKey { get; set; }
    public uint EnchantShopId { get; set; }
    public string EnchantPeriodType { get; set; }
    public uint EnchantPeriod { get; set; }
    public uint ProtectItemKey { get; set; }
    public uint ProtectShopId { get; set; }
    public string ProtectPeriodType { get; set; }
    public uint ProtectPeriod { get; set; }
  }

  internal class CombinationComponent
  {
    public uint ItemKey { get; set; }
    public uint ShopId { get; set; }
    public string PeriodType { get; set; }
    public uint Period { get; set; }
    public uint Color { get; set; }
    public uint EffectId { get; set; }
    public uint MinUseCount { get; set; }
    public uint MaxUseCount { get; set; }
  }
}
