using Ast.Expressions;

namespace Ast.Declarations;

public sealed class FunctionDeclaration : Declaration
{
    public FunctionDeclaration(string name, List<string> parameters, Expression body)
    {
        Name = name;
        Parameters = parameters;
        Body = body;
    }

    public string Name { get; }

    public List<string> Parameters { get; }

    public Expression Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}