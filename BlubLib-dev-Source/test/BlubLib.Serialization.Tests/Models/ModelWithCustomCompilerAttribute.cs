using BlubLib.Serialization.Tests.Serializers;

namespace BlubLib.Serialization.Tests.Models
{
    [BlubContract(typeof(ModelWithCustomCompilerAttributeCompiler))]
    public class ModelWithCustomCompilerAttribute
    {
        public byte A { get; set; }
    }
}