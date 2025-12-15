using Runtime;
using ValueType = Runtime.ValueType;

namespace Ast.Expressions;

public sealed class LiteralExpression : Expression
{
    public LiteralExpression(ValueType type, Value value)
    {
        Type = type;
        Value = value;
    }

    public ValueType Type { get; }

    public Value Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}