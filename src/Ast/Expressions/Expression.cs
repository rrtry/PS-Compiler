using Ast.Attributes;

using ValueType = Runtime.ValueType;

namespace Ast.Expressions;

public abstract class Expression : AstNode
{
    private AstAttribute<ValueType> _resultType;

    /// <summary>
    /// Тип результата выражения.
    /// </summary>
    public ValueType ResultType
    {
        get => _resultType.Get();

        set => _resultType.Set(value);
    }
}