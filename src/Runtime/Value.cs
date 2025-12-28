using System.Globalization;

using ValueType = Runtime.ValueType;

namespace Runtime;

/// <summary>
/// Абстракция над типами, которые доступны в языке.
/// int    - целое число (int64_t).
/// float  - вещественное число (double).
/// string - UTF-8 строка.
/// void   - отсуствие возвращаемого типа у функции.
/// nul    - значение по умолчанию при отсутствии инициализации.
/// </summary>
public class Value : IEquatable<Value>
{
    public const double Tolerance = 0.001d;
    public static readonly Value Void = new(VoidValue.Value);
    public static readonly Value Nil = new(NilValue.Value);
    private readonly object value;

    /// <summary>
    /// Создаёт строковое значение.
    /// </summary>
    public Value(string v)
    {
        value = v;
    }

    /// <summary>
    /// Создаёт целочисленное значение.
    /// </summary>
    public Value(long v)
    {
        value = v;
    }

    public Value(double v)
    {
        value = v;
    }

    private Value(object v)
    {
        value = v;
    }

    public bool IsDouble()
    {
        return value switch
        {
            double => true,
            _ => false
        };
    }

    public double AsDouble()
    {
        return value switch
        {
            double d => d,
            long l => l,
            _ => throw new InvalidOperationException($"Value {value} is not a double"),
        };
    }

    /// <summary>
    /// Определяет, является ли значение строкой.
    /// </summary>
    public bool IsString()
    {
        return value switch
        {
            string => true,
            _ => false,
        };
    }

    /// <summary>
    /// Возвращает значение как строку либо бросает исключение.
    /// </summary>
    public string AsString()
    {
        return value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Value {value} is not a string"),
        };
    }

    /// <summary>
    /// Определяет, является ли значение целым числом.
    /// </summary>
    public bool IsLong()
    {
        return value switch
        {
            long => true,
            _ => false,
        };
    }

    /// <summary>
    /// Возвращает значение как целое число либо бросает исключение.
    /// </summary>
    public long AsLong()
    {
        return value switch
        {
            long i => i,
            _ => throw new InvalidOperationException($"Value {value} is not numeric"),
        };
    }

    /// <summary>
    /// Печатает значение для отладки.
    /// </summary>
    public override string ToString()
    {
        return value switch
        {
            string s => ValueUtil.EscapeStringValue(s),
            long i => i.ToString(CultureInfo.InvariantCulture),
            double d => d.ToString(CultureInfo.InvariantCulture),

            VoidValue v => v.ToString(),
            NilValue v => v.ToString(),
            _ => throw new InvalidOperationException($"Unexpected value {value} of type {value.GetType()}"),
        };
    }

    /// <summary>
    /// Сравнивает на равенство два значения.
    /// </summary>
    public bool Equals(Value? other)
    {
        if (other is null)
        {
            return false;
        }

        return value switch
        {
            // Строки сравниваются посимвольно.
            string s => other.AsString() == s,

            // Целые сравниваются по значению.
            long i => other.IsDouble() ? Math.Abs(other.AsDouble() - i) < Tolerance : other.AsLong() == i,

            // Вещественные числа сравниваются с погрешностью.
            double d => Math.Abs(other.AsDouble() - d) < Tolerance,

            // Пустые значения всегда равны.
            VoidValue => true,

            // Несуществующая структура равна сама себе и не равна никаким другим.
            NilValue => other.value is NilValue,

            _ => throw new NotImplementedException(),
        };
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Value);
    }

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }
}