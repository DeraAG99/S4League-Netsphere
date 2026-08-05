using System;
using BlubLib.Serialization;
using NeoNetsphere.Network.Serializers;

namespace NeoNetsphere.Network.Data.Game
{
  [BlubContract]
  public class RandomShopDto
  {
    public RandomShopDto()
    {
      Items = Array.Empty<RandomShopItemDto>();
    }

    [BlubMember(0, typeof(ArrayWithIntPrefixSerializer))]
    public RandomShopItemDto[] Items { get; set; }
  }
}
