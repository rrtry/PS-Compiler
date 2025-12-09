using Ast.Declarations;
using Ast.Expressions;

namespace Ast;

public interface IAstVisitor
{
    public void Visit(BinaryOperationExpression e);

    public void Visit(UnaryOperationExpression e);

    public void Visit(LiteralExpression e);

    public void Visit(VariableExpression e);

    public void Visit(FunctionCallExpression e);

    public void Visit(AssignmentExpression e);

    public void Visit(SequenceExpression e);

    public void Visit(IfElseExpression e);

    public void Visit(ForLoopExpression e);

    public void Visit(VariableDeclaration d);

    public void Visit(AbstractFunctionDeclaration d);
}