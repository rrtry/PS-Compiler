namespace Execution;

using Ast;
using Ast.Declarations;
using Ast.Expressions;

public class AstEvaluator : IAstVisitor
{
    private readonly Context _context;

    private readonly Stack<decimal> _values = [];

    public AstEvaluator(Context context)
    {
        _context = context;
    }

    public decimal Evaluate(AstNode node)
    {
        if (_values.Count > 0)
        {
            throw new InvalidOperationException(
                $"Evaluation stack must be empty, but contains {_values.Count} values: {string.Join(", ", _values)}"
            );
        }

        node.Accept(this);

        return _values.Count switch
        {
            0 => throw new InvalidOperationException(
                "Evaluator logical error: the stack has no evaluation result"
            ),
            > 1 => throw new InvalidOperationException(
                $"Evaluator logical error: expected 1 value, got {_values.Count} values: {string.Join(", ", _values)}"
            ),
            _ => _values.Pop(),
        };
    }

    public void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);

        decimal right = _values.Pop();
        decimal left = _values.Pop();

        switch (e.Operation)
        {
            case BinaryOperation.Add:
                _values.Push(left + right);
                break;
            case BinaryOperation.Substract:
                _values.Push(left - right);
                break;
            case BinaryOperation.Multiply:
                _values.Push(left * right);
                break;
            case BinaryOperation.Divide:
                _values.Push(left / right);
                break;
            case BinaryOperation.Equal:
                _values.Push(Numbers.AreEqual(left, right) ? 1 : 0);
                break;
            case BinaryOperation.NotEqual:
                _values.Push(Numbers.AreEqual(left, right) ? 0 : 1);
                break;
            case BinaryOperation.GreaterThan:
                _values.Push(Numbers.IsGreaterThan(left, right) ? 1 : 0);
                break;
            case BinaryOperation.LessThan:
                _values.Push(Numbers.IsLessThan(left, right) ? 1 : 0);
                break;
            case BinaryOperation.GreaterThanOrEqual:
                _values.Push(Numbers.IsGreaterThanOrEqual(left, right) ? 1 : 0);
                break;
            case BinaryOperation.LessThanOrEqual:
                _values.Push(Numbers.IsLessOrEqual(left, right) ? 1 : 0);
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
                _values.Push(-_values.Pop());
                break;
            case UnaryOperation.Plus:
                break;
            default:
                throw new NotImplementedException($"Unknown unary operation {e.Operation}");
        }
    }

    public void Visit(LiteralExpression e)
    {
        _values.Push(e.Value);
    }

    public void Visit(VariableExpression e)
    {
        _values.Push(_context.GetValue(e.Name));
    }

    public void Visit(FunctionCallExpression e)
    {
        FunctionDeclaration function = _context.GetFunction(e.Name);

        // NOTE: вычисляем аргументы и временно сохраняем их в стеке.
        foreach (Expression argument in e.Arguments)
        {
            argument.Accept(this);
        }

        _context.PushScope(new Scope());
        try
        {
            // Определяем параметры, извлекая их из стека в обратном порядке.
            foreach (string name in Enumerable.Reverse(function.Parameters))
            {
                _context.DefineVariable(name, _values.Pop());
            }

            // Исполняем функцию
            function.Body.Accept(this);
        }
        finally
        {
            _context.PopScope();
        }
    }

    public void Visit(AssignmentExpression e)
    {
        // NOTE: Вычисляем выражение, и затем присваиваем его значение переменной,
        //  сохраняя результат в стеке.
        e.Value.Accept(this);
        decimal value = _values.Peek();
        _context.AssignVariable(e.Name, value);
    }

    public void Visit(SequenceExpression e)
    {
        // NOTE: Вычисляем все выражения последовательно, но сохраняем только последний результат.
        _values.Push(0);
        foreach (Expression nested in e.Sequence)
        {
            _values.Pop();
            nested.Accept(this);
        }
    }

    public void Visit(IfElseExpression e)
    {
        e.Condition.Accept(this);

        decimal conditionValue = _values.Pop();
        bool isTrueCondition = !Numbers.AreEqual(0.0m, conditionValue);

        if (isTrueCondition)
        {
            e.ThenBranch.Accept(this);
        }
        else
        {
            e.ElseBranch.Accept(this);
        }
    }

    public void Visit(ForLoopExpression e)
    {
        _context.PushScope(new Scope());
        try
        {
            // Вычисляем начальное значение переменной-итератора.
            e.StartValue.Accept(this);
            decimal iteratorValue = _values.Pop();

            // Вычисляем шаг итерации (по умолчанию 1, но может быть переопределён).
            decimal stepValue = 1;
            if (e.StepValue != null)
            {
                e.StepValue.Accept(this);
                stepValue = _values.Pop();
            }

            // Определяем переменную-итератор и добавляем в стек вероятное значение цикла
            _context.DefineVariable(e.IteratorName, iteratorValue);
            while (true)
            {
                // Вычисляем выражение-условие и сравниваем результат с 0.
                e.EndCondition.Accept(this);
                decimal endCondition = _values.Pop();

                if (Numbers.AreEqual(0.0m, endCondition))
                {
                    break;
                }

                // Выполняем тело цикла и отбрасываем результат.
                e.Body.Accept(this);
                _values.Pop();

                // Выполняем инкремент итератора.
                iteratorValue += stepValue;
                _context.AssignVariable(e.IteratorName, iteratorValue);
            }

            // Добавляем в стек значение 0.0, поскольку цикл хоть не возвращает осмысленного значения,
            //  но всё равно является выражением.
            _values.Push(0.0m);
        }
        finally
        {
            _context.PopScope();
        }
    }

    public void Visit(VariableDeclaration d)
    {
        // NOTE: Вычисляем инициализирующее выражение, и затем присваиваем его значение переменной,
        //  сохраняя результат в стеке.
        decimal value = 0;
        if (d.Value != null)
        {
            d.Value.Accept(this);
            value = _values.Peek();
        }

        _context.DefineVariable(d.Name, value);
    }

    public void Visit(FunctionDeclaration d)
    {
        _context.DefineFunction(d);

        // NOTE: Результат «вычисления» объявления функции — число 0.0.
        _values.Push(0m);
    }
}