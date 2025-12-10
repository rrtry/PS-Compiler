namespace Execution;

using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Execution.Exceptions;

public class AstEvaluator : IAstVisitor
{
    private readonly Context context;

    private readonly Stack<decimal> values = [];

    public AstEvaluator(Context context)
    {
        this.context = context;
    }

    public decimal Evaluate(AstNode node)
    {
        if (values.Count > 0)
        {
            throw new InvalidOperationException(
                $"Evaluation stack must be empty, but contains {values.Count} values: {string.Join(", ", values)}"
            );
        }

        node.Accept(this);

        return values.Count switch
        {
            0 => throw new InvalidOperationException(
                "Evaluator logical error: the stack has no evaluation result"
            ),
            > 1 => throw new InvalidOperationException(
                $"Evaluator logical error: expected 1 value, got {values.Count} values: {string.Join(", ", values)}"
            ),
            _ => values.Pop(),
        };
    }

    public void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);

        decimal right = values.Pop();
        decimal left = values.Pop();

        switch (e.Operation)
        {
            case BinaryOperation.And:
                values.Push((!Numbers.AreEqual(0m, left) && !Numbers.AreEqual(0m, right)) ? 1 : 0);
                break;
            case BinaryOperation.Or:
                values.Push((!Numbers.AreEqual(0m, left) || !Numbers.AreEqual(0m, right)) ? 1 : 0);
                break;
            case BinaryOperation.Modulo:
                values.Push(left % right);
                break;
            case BinaryOperation.Add:
                values.Push(left + right);
                break;
            case BinaryOperation.Substract:
                values.Push(left - right);
                break;
            case BinaryOperation.Multiply:
                values.Push(left * right);
                break;
            case BinaryOperation.Divide:
                values.Push(left / right);
                break;
            case BinaryOperation.Equal:
                values.Push(Numbers.AreEqual(left, right) ? 1 : 0);
                break;
            case BinaryOperation.NotEqual:
                values.Push(Numbers.AreEqual(left, right) ? 0 : 1);
                break;
            case BinaryOperation.GreaterThan:
                values.Push(Numbers.IsGreaterThan(left, right) ? 1 : 0);
                break;
            case BinaryOperation.LessThan:
                values.Push(Numbers.IsLessThan(left, right) ? 1 : 0);
                break;
            case BinaryOperation.GreaterThanOrEqual:
                values.Push(Numbers.IsGreaterThanOrEqual(left, right) ? 1 : 0);
                break;
            case BinaryOperation.LessThanOrEqual:
                values.Push(Numbers.IsLessOrEqual(left, right) ? 1 : 0);
                break;
            case BinaryOperation.Power:
                values.Push((decimal)Math.Pow((double)left, (double)right));
                break;
            default:
                throw new NotImplementedException($"Unknown binary operation {e.Operation}");
        }
    }

    public void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
        switch (e.Operation)
        {
            case UnaryOperation.Minus:
                values.Push(-values.Pop());
                break;
            case UnaryOperation.Plus:
                break;
            default:
                throw new NotImplementedException($"Unknown unary operation {e.Operation}");
        }
    }

    public void Visit(LiteralExpression e)
    {
        values.Push(e.Value);
    }

    public void Visit(VariableExpression e)
    {
        values.Push(context.GetValue(e.Name));
    }

    public void Visit(FunctionCallExpression e)
    {
        AbstractFunctionDeclaration function = context.GetFunction(e.Name);
        foreach (Expression argument in e.Arguments)
        {
            argument.Accept(this);
        }

        decimal result;
        if (function is NativeFunction)
        {
            List<decimal> args = [];
            for (int i = 0; i < function.Parameters.Count; i++)
            {
                args.Insert(0, values.Pop());
            }

            result = ((NativeFunction)function).Invoke(args);
        }
        else
        {
            context.PushScope(new Scope());
            bool hasReturn = false;

            try
            {
                foreach (string name in Enumerable.Reverse(function.Parameters))
                {
                    context.DefineVariable(name, values.Pop());
                }

                ((FunctionDeclaration)function).Body.Accept(this);
            }
            catch (ReturnException)
            {
                hasReturn = true;
            }
            finally
            {
                if (!hasReturn)
                {
                    throw new InvalidOperationException("Function has to have a return statement in the end");
                }
                else
                {
                    values.Pop(); // First pop return statement value
                    result = values.Pop(); // Then the expression we return;
                }

                context.PopScope();
            }
        }

        values.Push(result); // Again, push the value return by the function
    }

    public void Visit(AssignmentExpression e)
    {
        e.Value.Accept(this);
        decimal value = values.Peek();
        context.AssignVariable(e.Name, value);
    }

    public void Visit(BlockStatement s)
    {
        values.Push(0m);
        context.PushScope(new Scope());

        try
        {
            foreach (AstNode node in s.Statements)
            {
                values.Pop();
                node.Accept(this);
            }
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(IfElseStatement s)
    {
        s.Condition.Accept(this);

        decimal conditionValue = values.Pop();
        bool isTrueCondition = !Numbers.AreEqual(0.0m, conditionValue);

        if (isTrueCondition)
        {
            s.ThenBranch.Accept(this);
        }
        else
        {
            if (s.ElseBranch != null)
            {
                s.ElseBranch.Accept(this);
            }
            else
            {
                values.Push(0m);
            }
        }
    }

    public void Visit(ForLoopStatement e)
    {
        context.PushScope(new Scope());
        try
        {
            e.StartValue.Accept(this);
            decimal iteratorValue = values.Pop();

            context.AssignVariable(e.IteratorName, iteratorValue); // Changed: DefineVariable -> AssignVariable
            values.Push(0.0m);

            while (true)
            {
                e.EndCondition.Accept(this);
                decimal endCondition = values.Pop();

                if (Numbers.AreEqual(0m, endCondition))
                {
                    break;
                }

                try
                {
                    values.Pop();
                    e.Body.Accept(this);
                }
                catch (ContinueLoopException)
                {
                }

                e.StepValue!.Accept(this);
                values.Pop();
                /*
                iteratorValue += stepValue;
                context.AssignVariable(e.IteratorName, iteratorValue); */
            }
        }
        catch (BreakLoopException)
        {
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(WhileLoopStatement e)
    {
        values.Push(0m);
        context.PushScope(new Scope());

        try
        {
            while (true)
            {
                e.Condition.Accept(this);
                decimal condition = values.Pop();

                if (Numbers.AreEqual(0m, condition))
                {
                    break;
                }

                try
                {
                    values.Pop();
                    e.LoopBody.Accept(this);
                }
                catch (ContinueLoopException)
                {
                }
            }
        }
        catch (BreakLoopException)
        {
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(VariableDeclaration d)
    {
        decimal value = 0;
        if (d.Value != null)
        {
            d.Value.Accept(this);
            value = values.Peek();
        }

        context.DefineVariable(d.Name, value);
    }

    public void Visit(AbstractFunctionDeclaration d)
    {
        values.Push(0m);
        context.DefineFunction(d);
    }

    public void Visit(BreakLoopStatement s)
    {
        values.Push(0m);
        throw new BreakLoopException();
    }

    public void Visit(ContinueLoopStatement s)
    {
        values.Push(0m);
        throw new ContinueLoopException();
    }

    public void Visit(ReturnStatement s)
    {
        s.ReturnValue.Accept(this);
        values.Push(0m);
        throw new ReturnException();
    }

    public void Visit(ForLoopExpression e)
    {
    }

    public void Visit(SequenceExpression e)
    {
    }

    public void Visit(IfElseExpression e)
    {
    }
}