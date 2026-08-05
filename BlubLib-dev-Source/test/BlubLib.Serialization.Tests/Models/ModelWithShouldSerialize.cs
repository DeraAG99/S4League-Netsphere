namespace BlubLib.Serialization.Tests.Models
{
    [BlubContract]
    public class ModelWithShouldSerialize
    {
        [BlubMember(0)]
        public bool A { get; set; }

        [BlubMember(1)]
        public byte B { get; set; }

        public bool ShouldSerializeB()
        {
            return A;
        }
    }
}
