namespace Execution;

using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

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

        context.PushScope(new Scope());
        try
        {
            List<decimal> args = [];
            foreach (string name in Enumerable.Reverse(function.Parameters))
            {
                context.DefineVariable(name, values.Pop());
                args.Add(context.GetValue(name));
            }

            decimal result = ((NativeFunction)function).Invoke(Enumerable.Reverse(args).ToList());
            values.Push(result);
        }
        finally
        {
            context.PopScope();
        }

        /*
        // NOTE: вычисляем аргументы и временно сохраняем их в стеке.
        foreach (Expression argument in e.Arguments)
        {
            argument.Accept(this);
        }

        context.PushScope(new Scope());
        try
        {
            // Определяем параметры, извлекая их из стека в обратном порядке.
            foreach (string name in Enumerable.Reverse(function.Parameters))
            {
                context.DefineVariable(name, values.Pop());
            }

            // Исполняем функцию
            function.Body.Accept(this);
        }
        finally
        {
            context.PopScope();
        } */
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

    public void Visit(ForLoopExpression e)
    {
        context.PushScope(new Scope());
        try
        {
            // Вычисляем начальное значение переменной-итератора.
            e.StartValue.Accept(this);
            decimal iteratorValue = values.Pop();

            // Вычисляем шаг итерации (по умолчанию 1, но может быть переопределён).
            decimal stepValue = 1;
            if (e.StepValue != null)
            {
                e.StepValue.Accept(this);
                stepValue = values.Pop();
            }

            // Определяем переменную-итератор и добавляем в стек вероятное значение цикла
            context.DefineVariable(e.IteratorName, iteratorValue);
            while (true)
            {
                // Вычисляем выражение-условие и сравниваем результат с 0.
                e.EndCondition.Accept(this);
                decimal endCondition = values.Pop();

                if (Numbers.AreEqual(0.0m, endCondition))
                {
                    break;
                }

                // Выполняем тело цикла и отбрасываем результат.
                e.Body.Accept(this);
                values.Pop();

                // Выполняем инкремент итератора.
                iteratorValue += stepValue;
                context.AssignVariable(e.IteratorName, iteratorValue);
            }

            // Добавляем в стек значение 0.0, поскольку цикл хоть не возвращает осмысленного значения,
            //  но всё равно является выражением.
            values.Push(0.0m);
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
        context.DefineFunction(d);
        values.Push(0m);
    }

    public void Visit(SequenceExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(IfElseExpression e)
    {
        throw new NotImplementedException();
    }
}