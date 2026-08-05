namespace BlubLib.Serialization.Tests.Models
{
    [BlubContract]
    public class InheritedModel : SimpleModel
    {
        [BlubMember(0)]
        public byte B { get; set; }
    }
}