using TradeForge.BacktestEngine.Enums;

namespace TradeForge.BacktestEngine.Models;

public abstract class BacktestStrategy
{
    public abstract Task<bool> OnInit(Services.BacktestEngine engine, Account account);
    public abstract Task OnBar(Services.BacktestEngine engine, Account account, 
        int index, DateTime[] dates,
        double[] open, double[] high, double[] low, double[] close);
    
    public List<StrategyParameter> Parameters { get; } = new();
    
    protected void AddParameter<T>(string name, string displayName, T defaultValue,
        T? min = default, T? max = default)
    {
        Parameters.Add(new StrategyParameter
        {
            Name = name,
            DisplayName = displayName,
            Type = GetParamType(typeof(T)),
            Value = defaultValue,
            Min = min,
            Max = max,
            EnumType = typeof(T).IsEnum ? typeof(T) : null
        });
    }

    private static ParamType GetParamType(Type type)
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
        (T)Parameters.First(p => p.Name == name).Value!;
}