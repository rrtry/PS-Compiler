namespace Ast.Expressions;

public sealed class LiteralExpression : Expression
{
    public LiteralExpression(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public override void Accept(
        IAstVisitor visitor
    )
    {
        visitor.Visit(this);
    }
}