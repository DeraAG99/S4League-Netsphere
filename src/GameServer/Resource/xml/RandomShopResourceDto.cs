using System.Xml.Serialization;

namespace NeoNetsphere.Resource.xml
{
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = true, ElementName = "RandomShop")]
  public class RandomShopResourceDto
  {
    [XmlElement("pool")] public RandomShopPoolDto[] Pools { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class RandomShopPoolDto
  {
    [XmlAttribute] public int Id { get; set; }

    [XmlAttribute] public uint PriceType { get; set; }

    [XmlAttribute] public uint Price { get; set; }

    [XmlElement("periods")]
    public RandomShopPeriodsDto Periods { get; set; }

    [XmlElement("item")] public RandomShopItemPoolDto[] Items { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class RandomShopPeriodsDto
  {
    [XmlElement("period")] public RandomShopPeriodDto[] Periods { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class RandomShopPeriodDto
  {
    [XmlAttribute] public uint PeriodType { get; set; }

    [XmlAttribute] public uint Period { get; set; }

    [XmlAttribute] public uint Rate { get; set; }
  }

  [XmlType(AnonymousType = true)]
  public class RandomShopItemPoolDto
  {
    [XmlAttribute] public uint ItemNumber { get; set; }

    [XmlAttribute] public uint RewardNumber { get; set; }

    [XmlAttribute] public uint PeriodType { get; set; }

    [XmlAttribute] public uint Period { get; set; }

    [XmlAttribute] public uint Effect { get; set; }

    [XmlAttribute] public byte Color { get; set; }

    [XmlAttribute] public uint Rate { get; set; }
  }
}
