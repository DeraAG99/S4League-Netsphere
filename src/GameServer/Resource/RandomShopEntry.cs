using System.Collections.Generic;
using NeoNetsphere.Network;

namespace NeoNetsphere.Resource
{
  internal class RandomShopPool
  {
    public int Id { get; set; }

    public ItemPriceType PriceType { get; set; }

    public uint Price { get; set; }

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
  }

  internal class RandomShopItem
  {
    public ItemNumber ItemNumber { get; set; }

    public ItemPeriodType PeriodType { get; set; }

    public ushort Period { get; set; }

    public uint Effect { get; set; }

    public byte Color { get; set; }

    public uint Rate { get; set; }
  }
}
