using BlubLib.Serialization.Tests.Serializers;

namespace BlubLib.Serialization.Tests.Models
{
    [BlubContract]
    public class ModelWithCustomSerializerOnProperty
    {
        [BlubMember(0, SerializerType = typeof(SimpleModelSerializer))]
        public SimpleModel SimpleModel { get; set; }
    }
}
