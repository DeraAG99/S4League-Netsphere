namespace BlubLib.Serialization.Tests.Models
{
    [BlubContract]
    public class SimpleModel
    {
        [BlubMember(0)]
        public byte A { get; set; }
    }
}