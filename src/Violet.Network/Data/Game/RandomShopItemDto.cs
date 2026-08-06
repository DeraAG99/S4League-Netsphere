using BlubLib.Serialization;

namespace NeoNetsphere.Network.Data.Game
{
  // RE (S4ClientLocal 0.8.32): the client reads each random shop item as a
  // native-aligned 28-byte (0x1C) struct:
  //   +0  uint  ItemNumber (name key)
  //   +4  int   Effect
  //   +8  uint  (unused / effect group)
  //   +12 byte  Color
  //   +13 3x pad
  //   +16 int   ItemNumber (image / period lookup)
  //   +20 byte  (unused)
  //   +21 3x pad
  //   +24 uint  Rate
  [BlubContract]
  public class RandomShopItemDto
  {
    [BlubMember(0)] public uint Unk1 { get; set; }

    [BlubMember(1)] public int Unk2 { get; set; }

    [BlubMember(2)] public uint Unk3 { get; set; }

    [BlubMember(3)] public byte Unk4 { get; set; }

    [BlubMember(4)] public byte Pad1 { get; set; }

    [BlubMember(5)] public byte Pad2 { get; set; }

    [BlubMember(6)] public byte Pad3 { get; set; }

    [BlubMember(7)] public int Unk5 { get; set; }

    [BlubMember(8)] public byte Unk6 { get; set; }

    [BlubMember(9)] public byte Pad4 { get; set; }

    [BlubMember(10)] public byte Pad5 { get; set; }

    [BlubMember(11)] public byte Pad6 { get; set; }

    [BlubMember(12)] public uint Unk7 { get; set; }
  }
}
