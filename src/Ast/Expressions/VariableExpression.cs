using Ast.Attributes;
using Ast.Declarations;

namespace Ast.Expressions;

/// <summary>
/// Выражение доступа к переменной по имени.
/// </summary>
public sealed class VariableExpression : Expression
{
    private AstAttribute<AbstractVariableDeclaration> _variable;

    public VariableExpression(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public AbstractVariableDeclaration Variable
    {
        get => _variable.Get();
        set => _variable.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}