namespace BlubLib.Serialization.Tests.Models
{
    [BlubContract]
    public struct ModelStruct
    {
        [BlubMember(0)]
        public byte A { get; set; }
    }
}