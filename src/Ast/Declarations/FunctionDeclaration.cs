namespace Ast.Declarations;

public sealed class FunctionDeclaration : AbstractFunctionDeclaration
{
    public FunctionDeclaration(string name, List<string> parameters, BlockStatement body)
    : base(name, parameters)
    {
        Body = body;
    }

    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}