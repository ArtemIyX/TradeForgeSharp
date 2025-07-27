namespace TradeForge.Core.Generic;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Generic converter for any enum that uses [EnumMember] attributes
/// or falls back to camelCase names.
/// </summary>
public sealed class EnumMemberJsonConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    private static readonly ConcurrentDictionary<Type, MapHolder> Cache = new();

    private sealed class MapHolder
    {
        public readonly Dictionary<string, TEnum> Read;
        public readonly Dictionary<TEnum, string> Write;

        public MapHolder()
        {
            Read  = new Dictionary<string, TEnum>(StringComparer.OrdinalIgnoreCase);
            Write = new Dictionary<TEnum, string>();

            foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attr = field.GetCustomAttribute<EnumMemberAttribute>();
                var name = attr?.Value ?? JsonNamingPolicy.CamelCase.ConvertName(field.Name);
                var value = (TEnum)field.GetValue(null)!;

                Read[name] = value;
                Write[value] = name;
            }
        }
    }

    private static MapHolder GetMaps() => Cache.GetOrAdd(typeof(TEnum), _ => new MapHolder());

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? text = reader.GetString();
        MapHolder maps = GetMaps();
        if (text is null || !maps.Read.TryGetValue(text, out var value))
            throw new JsonException($"Unknown {typeof(TEnum).Name} value '{text}'");
        return value;
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(GetMaps().Write[value]);
    }
}