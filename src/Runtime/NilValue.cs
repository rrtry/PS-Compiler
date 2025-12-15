namespace Runtime;

/// <summary>
/// Специальный тип, обозначающий неинициализированную переменную.
/// </summary>
public record struct NilValue
{
    public static readonly NilValue Value = default;

    public override string ToString()
    {
        return "nil";
    }
}