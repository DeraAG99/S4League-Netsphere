namespace BlubLib.Serialization.Tests.Models
{
    [BlubContract]
    public class ComplexModel
    {
        [BlubMember(0)]
        public SimpleModel SimpleModel { get; set; }
    }
}
