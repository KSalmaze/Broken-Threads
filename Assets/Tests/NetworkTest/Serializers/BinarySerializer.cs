using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tests.NetworkTest.Serializers
{
    public class BinarySerializer: Serializer
    {
        readonly BinaryFormatter _formatter = new BinaryFormatter();
        
        public override byte[] Serialize<T>(T obj)
        {
            MemoryStream stream = new MemoryStream();
        
#pragma warning disable SYSLIB0011
            _formatter.Serialize(stream,obj);
#pragma warning restore SYSLIB0011
        
            return stream.ToArray();
        }

        public override T Deserialize<T>(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            
#pragma warning disable SYSLIB0011
            return (T)_formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
        }
    }
}