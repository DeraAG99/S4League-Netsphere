using System.Collections.Generic;
using NeoNetsphere.Network;

namespace NeoNetsphere.Resource
{
  internal class RandomShopPool
  {
    public int Id { get; set; }

    public ItemPriceType PriceType { get; set; }

    public uint Price { get; set; }

    public List<RandomShopPeriodTier> Periods { get; set; } = new List<RandomShopPeriodTier>();

    public List<RandomShopItem> Items { get; set; } = new List<RandomShopItem>();

    public RandomShopItem Roll()
    {
      var totalRate = 0u;
      foreach (var item in Items)
        totalRate += item.Rate;

      var roll = (uint)(new System.Random().NextDouble() * totalRate);
      foreach (var item in Items)
      {
        if (roll < item.Rate)
          return item;
        roll -= item.Rate;
      }

      return Items[Items.Count - 1];
    }

    public RandomShopPeriodTier RollPeriod()
    {
      if (Periods.Count == 0)
        return new RandomShopPeriodTier { PeriodType = ItemPeriodType.None, Period = 0, Rate = 1 };

      var totalRate = 0u;
      foreach (var period in Periods)
        totalRate += period.Rate;

      if (totalRate == 0)
        return Periods[Periods.Count - 1];

      var roll = (uint)(new System.Random().NextDouble() * totalRate);
      foreach (var period in Periods)
      {
        if (roll < period.Rate)
          return period;
        roll -= period.Rate;
      }

      return Periods[Periods.Count - 1];
    }
  }

  internal class RandomShopPeriodTier
  {
    public ItemPeriodType PeriodType { get; set; }

    public ushort Period { get; set; }

    public uint Rate { get; set; }
  }

  internal class RandomShopItem
  {
    public ItemNumber ItemNumber { get; set; }

    public ItemNumber RewardNumber { get; set; }

    public ItemPeriodType PeriodType { get; set; }

    public ushort Period { get; set; }

    public uint Effect { get; set; }

    public byte Color { get; set; }

    public uint Rate { get; set; }
  }
}
