using System.IO.Compression;

namespace TradeForge.Core.Services.Interfaces;

public interface ITradeForgeSerializer<T>
{
    public byte[] Serialize(T source)
    {
        using var ms = new MemoryStream();
        using (var deflate = new DeflateStream(ms, CompressionLevel.Optimal, leaveOpen: true))
            ProtoBuf.Serializer.Serialize(deflate, source);   // protobuf-net
        return ms.ToArray();
    }

    public T Deserialize(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var deflate = new DeflateStream(ms, CompressionMode.Decompress);
        return  ProtoBuf.Serializer.Deserialize<T>(deflate);
    }
}