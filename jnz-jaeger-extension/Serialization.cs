using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Jnz.JaegerExtensions
{
    public static class Serialization
    {
        public static byte[] ToByteArray(this object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        public static T FromByteArray<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
                return default;

            BinaryFormatter bf = new BinaryFormatter();
            using var ms = new MemoryStream(byteArray);
            return (T)bf.Deserialize(ms);
        }
    }
}
