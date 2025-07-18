using System.IO.Compression;

namespace TradeForge.Core.Generic;

public static class TradeForgeSerializer<T>
{
    public static byte[] Serialize(T source)
    {
        using var ms = new MemoryStream();
        using (var deflate = new DeflateStream(ms, CompressionLevel.Optimal, leaveOpen: true))
            ProtoBuf.Serializer.Serialize(deflate, source);   // protobuf-net
        return ms.ToArray();
    }

    public static T Deserialize(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var deflate = new DeflateStream(ms, CompressionMode.Decompress);
        return  ProtoBuf.Serializer.Deserialize<T>(deflate);
    }
}