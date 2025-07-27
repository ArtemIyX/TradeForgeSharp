using TradeForge.BacktestEngine.Enums;

namespace TradeForge.BacktestEngine.Models;

public abstract class BacktestStrategy
{
    public abstract Task<bool> OnInit(Services.BacktestEngine engine, Account account);
    public abstract Task OnBar(Services.BacktestEngine engine, Account account, 
        int index, DateTime[] dates,
        double[] open, double[] high, double[] low, double[] close);
    protected abstract IEnumerable<StrategyParameter> DefineParameters();
    protected List<StrategyParameter>? CachedParams { get; set; } = null;

    public abstract string DisplayName { get; }
    public sealed override string ToString() => DisplayName;
    
    // Public getter (cached) 
    public IReadOnlyList<StrategyParameter> Parameters =>
        (CachedParams ??= DefineParameters().ToList()).AsReadOnly();
    
    /// <summary>
    /// Set a parameter value from the UI.
    /// </summary>
    /// <param name="name">Parameter name (case-sensitive).</param>
    /// <param name="value">New value.</param>
    /// <exception cref="ArgumentException">Unknown or incompatible value.</exception>
    public void SetParameter(string name, object? value)
    {
        if (CachedParams == null) _ = Parameters;   // force cache population

        StrategyParameter p = CachedParams!.FirstOrDefault(p => p.Name == name)
                              ?? throw new ArgumentException($"Unknown parameter '{name}'");

        Type runtimeType = p.EnumType ?? GetRuntimeType(p.Type);

        if (value is not null &&
            !runtimeType.IsInstanceOfType(value) &&
            !CanConvert(value, runtimeType))
        {
            throw new ArgumentException($"Value '{value}' is not compatible with {runtimeType}");
        }

        p.Value = Convert.ChangeType(value,
            Nullable.GetUnderlyingType(runtimeType) ?? runtimeType);
    }
    
    private static Type GetRuntimeType(ParamType paramType) => paramType switch
    {
        ParamType.Int    => typeof(int),
        ParamType.Double => typeof(double),
        ParamType.String => typeof(string),
        ParamType.Bool   => typeof(bool),
        ParamType.Enum   => throw new InvalidOperationException(
            "Enum parameters must use EnumType property."),
        _                => throw new NotSupportedException()
    };

    private static bool CanConvert(object value, Type target)
    {
        try
        {
            Convert.ChangeType(value, target);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public object this[string name]
    {
        get => GetParam<object>(name);
        set => SetParameter(name, value);
    }
    
    protected static ParamType GetParamType(Type type)
    {
        if (type.IsEnum) return ParamType.Enum;
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Int32 => ParamType.Int,
            TypeCode.Double => ParamType.Double,
            TypeCode.String => ParamType.String,
            TypeCode.Boolean => ParamType.Bool,
            _ => throw new NotSupportedException($"Type {type} not supported")
        };
    }

    public T GetParam<T>(string name) =>
        (T)CachedParams?.First(p => p.Name == name).Value!;
}