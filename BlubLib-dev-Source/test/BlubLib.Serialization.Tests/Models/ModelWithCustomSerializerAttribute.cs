using BlubLib.Serialization.Tests.Serializers;

namespace BlubLib.Serialization.Tests.Models
{
    [BlubContract(typeof(ModelWithCustomSerializerAttributeSerializer))]
    public class ModelWithCustomSerializerAttribute
    {
        public byte A { get; set; }
    }
}