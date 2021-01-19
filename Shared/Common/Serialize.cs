using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using ProtoBuf;

namespace Shared.Common
{
    public class Serialize
    {
        public static byte[] ProtoBufSerialize(object item)
        {
            if (item != null)
                try
                {
                    var ms = new MemoryStream();
                    Serializer.Serialize(ms, item);
                    var rt = ms.ToArray();
                    return rt;
                }
                catch (ProtoException ex)
                {
                    throw new Exception("Unable to serialize object", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to serialize object", ex);
                }

            throw new Exception("Object serialize is null");
        }

        public static byte[] ProtoBufSerialize(object item, bool isCompress)
        {
            if (item != null)
                try
                {
                    var ms = new MemoryStream();
                    Serializer.Serialize(ms, item);
                    var rt = ms.ToArray();
                    if (isCompress) rt = Compress(rt);
                    return rt;
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to serialize object", ex);
                }

            throw new Exception("Object serialize is null");
        }

        public static Stream ProtoBufSerializeToStream(object item, bool isCompress)
        {
            if (item != null)
                try
                {
                    var ms = new MemoryStream();
                    Serializer.Serialize(ms, item);

                    if (isCompress)
                    {
                        var rt = ms.ToArray();
                        return CompressToStream(rt);
                    }

                    return ms;
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to serialize object", ex);
                }

            throw new Exception("Object serialize is null");
        }

        public static T ProtoBufDeserialize<T>(byte[] byteArray)
        {
            if (byteArray != null && byteArray.Length > 0)
                try
                {
                    var ms = new MemoryStream(byteArray);
                    return Serializer.Deserialize<T>(ms);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to deserialize object", ex);
                    //return default(T);
                }

            throw new Exception("Object Deserialize is null or empty");
        }

        public static T ProtoBufDeserialize<T>(byte[] byteArray, bool isDecompress)
        {
            if (byteArray != null && byteArray.Length > 0)
                try
                {
                    if (isDecompress) byteArray = Decompress(byteArray);
                    return ProtoBufDeserialize<T>(byteArray);
                }
                catch (Exception ex)
                {
                    //throw new Exception("Unable to deserialize object", ex);
                    return default;
                }

            throw new Exception("Object Deserialize is null or empty");
        }

        public static object ProtoBufDeserialize(byte[] byteArray, Type type)
        {
            if (byteArray != null && byteArray.Length > 0)
                try
                {
                    var ms = new MemoryStream(byteArray);
                    return Serializer.Deserialize(type, ms);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to deserialize object", ex);
                    //return default(T);
                }

            throw new Exception("Object Deserialize is null or empty");
        }

        public static object ProtoBufDeserialize(byte[] byteArray, Type type, bool isDecompress)
        {
            if (byteArray != null && byteArray.Length > 0)
                try
                {
                    if (isDecompress) byteArray = Decompress(byteArray);
                    return ProtoBufDeserialize(byteArray, type);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to deserialize object", ex);
                    //return default(T);
                }

            throw new Exception("Object Deserialize is null or empty");
        }


        public static byte[] Compress(byte[] raw)
        {
            using (var memory = new MemoryStream())
            {
                using (var gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }

                return memory.ToArray();
            }
        }

        public static Stream CompressToStream(byte[] raw)
        {
            using (var memory = new MemoryStream())
            {
                using (var gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }

                return memory;
            }
        }

        public static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (var stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                var buffer = new byte[size];
                using (var memory = new MemoryStream())
                {
                    var count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0) memory.Write(buffer, 0, count);
                    } while (count > 0);

                    return memory.ToArray();
                }
            }
        }

        public static string JsonSerializeObject<T>(T obj)
        {
            if (obj == null) return string.Empty;

            return JsonConvert.SerializeObject(obj, Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public static T JsonDeserializeObject<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) return default;
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object JsonDeserializeObject(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public static byte[] BinarySerializer(object _object)
        {
            if (_object == null) return null;
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                IFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, _object);
                bytes = memoryStream.ToArray();
            }

            bytes = Compress(bytes);
            return bytes;
        }

        public static T BinaryDeserializer<T>(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return default;
            byteArray = Decompress(byteArray);
            T returnValue;
            using (var memoryStream = new MemoryStream(byteArray))
            {
                IFormatter binaryFormatter = new BinaryFormatter();
                returnValue = (T) binaryFormatter.Deserialize(memoryStream);
            }

            return returnValue;
        }
    }
}