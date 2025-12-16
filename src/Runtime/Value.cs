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
    private readonly object _value;

    /// <summary>
    /// Создаёт строковое значение.
    /// </summary>
    public Value(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Создаёт целочисленное значение.
    /// </summary>
    public Value(long value)
    {
        _value = value;
    }

    public Value(double value)
    {
        _value = value;
    }

    private Value(object value)
    {
        _value = value;
    }

    public bool IsNumeric()
    {
        return IsDouble() || IsLong();
    }

    public bool IsDouble()
    {
        return _value switch
        {
            double => true,
            _ => false
        };
    }

    public double AsDouble()
    {
        return _value switch
        {
            double d => d,
            long l => l,
            _ => throw new InvalidOperationException($"Value {_value} is not a double"),
        };
    }

    /// <summary>
    /// Определяет, является ли значение строкой.
    /// </summary>
    public bool IsString()
    {
        return _value switch
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
        return _value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Value {_value} is not a string"),
        };
    }

    /// <summary>
    /// Определяет, является ли значение целым числом.
    /// </summary>
    public bool IsLong()
    {
        return _value switch
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
        return _value switch
        {
            long i => i,
            _ => throw new InvalidOperationException($"Value {_value} is not numeric"),
        };
    }

    /// <summary>
    /// Печатает значение для отладки.
    /// </summary>
    public override string ToString()
    {
        return _value switch
        {
            string s => ValueUtil.EscapeStringValue(s),
            long i => i.ToString(CultureInfo.InvariantCulture),
            double d => d.ToString(CultureInfo.InvariantCulture),

            VoidValue v => v.ToString(),
            NilValue v => v.ToString(),
            _ => throw new InvalidOperationException($"Unexpected value {_value} of type {_value.GetType()}"),
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

        return _value switch
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
            NilValue => other._value is NilValue,

            _ => throw new NotImplementedException(),
        };
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Value);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
}