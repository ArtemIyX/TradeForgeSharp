namespace TradeForge.Core.Extensions;

public static class DeepCloneExtensions
{
    public static List<T> DeepCloneList<T>(this IEnumerable<T> source) where T : ICloneable
    {
        return source.Select(item => (T)item.Clone()).ToList();
    }
}