using BlubLib.Serialization.Tests.Serializers;

namespace BlubLib.Serialization.Tests.Models
{
    [BlubContract]
    public class ModelWithCustomCompilerOnProperty
    {
        [BlubMember(0, SerializerType = typeof(SimpleModelCompiler))]
        public SimpleModel SimpleModel { get; set; }
    }
}
