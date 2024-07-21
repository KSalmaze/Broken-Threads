namespace Tests.NetworkTest.Serializers
{
    public abstract class Serializer
    {
        public abstract byte[] Serialize<T>(T obj);
        
        public abstract T Deserialize<T>(byte[] bytes);
    }
}