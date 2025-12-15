namespace Execution;

using Ast;
using Runtime;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Execution.Exceptions;

public class AstEvaluator : IAstVisitor
{
    private readonly Context context;

    private readonly Stack<Value> values = [];

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

        switch (values.Count)
        {
            case 0:
                throw new InvalidOperationException("Evaluator logical error: the stack has no evaluation result");

            case > 1:
                throw new InvalidOperationException($"Evaluator logical error: expected 1 value, got {values.Count} values: {string.Join(", ", values)}");

            default:
                Value v = values.Pop();
                if (v == Value.Void)
                {
                    return 0m;
                }

                return v.AsLong();
        }

        /*
        return values.Count switch
        {
            0 => throw new InvalidOperationException(
                "Evaluator logical error: the stack has no evaluation result"
            ),
            > 1 => throw new InvalidOperationException(
                $"Evaluator logical error: expected 1 value, got {values.Count} values: {string.Join(", ", values)}"
            ),
            _ => values.Pop(),
        }; */
    }

    public void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);

        long right = values.Pop().AsLong();
        long left = values.Pop().AsLong();

        switch (e.Operation)
        {
            case BinaryOperation.And:
                values.Push(new Value((!Numbers.AreEqual(0, left) && !Numbers.AreEqual(0, right)) ? 1L : 0L));
                break;
            case BinaryOperation.Or:
                values.Push(new Value((!Numbers.AreEqual(0, left) || !Numbers.AreEqual(0, right)) ? 1L : 0L));
                break;
            case BinaryOperation.Modulo:
                values.Push(new Value(left % right));
                break;
            case BinaryOperation.Add:
                values.Push(new Value(left + right));
                break;
            case BinaryOperation.Substract:
                values.Push(new Value(left - right));
                break;
            case BinaryOperation.Multiply:
                values.Push(new Value(left * right));
                break;
            case BinaryOperation.Divide:
                values.Push(new Value(left / right));
                break;
            case BinaryOperation.Equal:
                values.Push(new Value(Numbers.AreEqual(left, right) ? 1L : 0L));
                break;
            case BinaryOperation.NotEqual:
                values.Push(new Value(Numbers.AreEqual(left, right) ? 0L : 1L));
                break;
            case BinaryOperation.GreaterThan:
                values.Push(new Value(Numbers.IsGreaterThan(left, right) ? 1L : 0L));
                break;
            case BinaryOperation.LessThan:
                values.Push(new Value(Numbers.IsLessThan(left, right) ? 1L : 0L));
                break;
            case BinaryOperation.GreaterThanOrEqual:
                values.Push(new Value(Numbers.IsGreaterThanOrEqual(left, right) ? 1L : 0L));
                break;
            case BinaryOperation.LessThanOrEqual:
                values.Push(new Value(Numbers.IsLessOrEqual(left, right) ? 1L : 0L));
                break;
            case BinaryOperation.Power:
                values.Push(new Value((long)Math.Pow(left, right)));
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
            case UnaryOperation.Not:
                values.Push(new Value(Numbers.AreEqual(0, values.Pop().AsLong()) ? 1L : 0L));
                break;
            case UnaryOperation.Minus:
                values.Push(new Value(-values.Pop().AsLong()));
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
        e.Function = context.TryGetFunction(e.Name);
        switch (e.Function)
        {
            case NativeFunction nativeFunction:
                InvokeNativeFunction(e, nativeFunction);
                break;
            case FunctionDeclaration function:
                InvokeFunction(e, function);
                break;
            default:
                throw new InvalidOperationException($"Unknown function subclass {e.Function.GetType()}");
        }
    }

    public void Visit(AssignmentExpression e)
    {
        e.Value.Accept(this);
        Value value = values.Peek();
        context.AssignVariable(e.Name, value);
    }

    public void Visit(BlockStatement s)
    {
        values.Push(Value.Void);
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

        long conditionValue = values.Pop().AsLong();
        bool isTrueCondition = !Numbers.AreEqual(0, conditionValue);

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
                values.Push(Value.Void);
            }
        }
    }

    public void Visit(ForLoopStatement e)
    {
        context.PushScope(new Scope());
        try
        {
            e.StartValue.Accept(this);
            long iteratorValue = values.Pop().AsLong();

            context.AssignVariable(e.IteratorName, new Value(iteratorValue)); // Changed: DefineVariable -> AssignVariable
            values.Push(Value.Void);

            while (true)
            {
                e.EndCondition.Accept(this);
                long endCondition = values.Pop().AsLong();

                if (Numbers.AreEqual(0, endCondition))
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
        values.Push(Value.Void);
        context.PushScope(new Scope());

        try
        {
            while (true)
            {
                e.Condition.Accept(this);
                long condition = values.Pop().AsLong();

                if (Numbers.AreEqual(0, condition))
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
        d.InitialValue.Accept(this);
        Value value = values.Peek();
        context.DefineVariable(d.Name, value);
    }

    public void Visit(AbstractFunctionDeclaration d)
    {
        values.Push(Value.Void);
        context.DefineFunction(d);
    }

    public void Visit(BreakLoopStatement s)
    {
        values.Push(Value.Void);
        throw new BreakLoopException();
    }

    public void Visit(ContinueLoopStatement s)
    {
        values.Push(Value.Void);
        throw new ContinueLoopException();
    }

    public void Visit(ReturnStatement s)
    {
        s.ReturnValue.Accept(this);
        values.Push(Value.Void);
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

    public void Visit(ParameterDeclaration d)
    {
    }

    private void InvokeNativeFunction(FunctionCallExpression e, NativeFunction function)
    {
        List<Value> arguments = [];
        foreach (Expression argument in e.Arguments)
        {
            argument.Accept(this);
            arguments.Add(values.Pop());
        }

        Value result = function.Invoke(arguments);
        values.Push(result);
    }

    private void InvokeFunction(FunctionCallExpression e, FunctionDeclaration function)
    {
        bool hasReturn = false;
        Value returnValue = Value.Void;
        context.PushScope(new Scope());

        try
        {
            for (int i = 0, iMax = function.Parameters.Count; i < iMax; ++i)
            {
                e.Arguments[i].Accept(this);
                Value argument = values.Pop();

                string name = function.Parameters[i].Name;
                context.DefineVariable(name, argument);
            }

            function.Body.Accept(this);
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
                returnValue = values.Pop(); // Then the expression we return;
            }

            context.PopScope();
        }

        values.Push(returnValue);
    }
}