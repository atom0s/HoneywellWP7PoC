
namespace TotalConnect.Classes
{
    using System.IO;
    using ProtoBuf;

    public class ProtobufHandler
    {
        /// <summary>
        /// Serializes a ProtoContract object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T obj)
        {
            using (var memStream = new MemoryStream())
            {
                // Attempt to serialize..
                Serializer.Serialize<T>(memStream, obj);

                // Read and return the result..
                using (var reader = new BinaryReader(memStream))
                {
                    memStream.Position = 0;
                    return reader.ReadBytes((int)memStream.Length);
                }
            }
        }

        /// <summary>
        /// Deserializes a ProtoContract object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] data)
        {
            using (var memStream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(memStream);
            }
        }
    }
}
