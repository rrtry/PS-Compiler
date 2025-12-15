namespace Ast.Declarations;

using Runtime;
using Ast.Attributes;

/// <summary>
/// Абстрактный класс всех объявлений (declarations).
/// </summary>
public abstract class Declaration : AstNode
{
    private AstAttribute<ValueType> _resultType;

    /// <summary>
    /// Тип результата объявления.
    /// </summary>
    public ValueType ResultType
    {
        get => _resultType.Get();

        set => _resultType.Set(value);
    }
}