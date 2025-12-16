namespace Execution;

using Runtime;
using Ast.Declarations;

using ValueType = Runtime.ValueType;

/// <summary>
/// Контекст выполнения программы (все переменные, константы и другие символы).
/// </summary>
public class Context
{
    private readonly IEnvironment environment;
    private readonly Stack<Scope> scopes = [];
    private readonly Dictionary<string, AbstractFunctionDeclaration> functions = [];
    private readonly Dictionary<string, NativeFunction> nativeFunctions;

    public Context(IEnvironment environment)
    {
        scopes.Push(new Scope());
        this.environment = environment;
        this.nativeFunctions = new Dictionary<string, NativeFunction>
        {
            {
                "int",
                new(
                    "int",
                    [new NativeFunctionParameter("x", ValueType.Float)],
                    ValueType.Float,
                    args => new Value((long)args[0].AsDouble())
                )
            },
            {
                "float",
                new(
                    "float",
                    [new NativeFunctionParameter("x", ValueType.Int)],
                    ValueType.Float,
                    args => new Value((double)args[0].AsLong())
                )
            },
            {
                "input",
                new(
                    "input",
                    [],
                    ValueType.Int,
                    _ =>
                    {
                        decimal d = environment.ReadDecimal() ?? throw new ArgumentException("Couldn't read decimal from stdin");
                        return new Value((long)d);
                    }
                )
            },
            {
                "print",
                new(
                    "print",
                    [ new NativeFunctionParameter("n", ValueType.Int),],
                    ValueType.Void,
                    arguments =>
                    {
                        environment.PrintDecimal(arguments[0].AsLong());
                        return new Value(0L);
                    }
                )
            },
            {
                "abs",
                new(
                    "abs",
                    [new NativeFunctionParameter("x", ValueType.Int)],
                    ValueType.Int,
                    (args) =>
                    {
                        long l = Math.Abs(args[0].AsLong());
                        return new Value(l);
                    }
                )
            },
            {
                "sqrt",
                new(
                    "sqrt",
                    [new NativeFunctionParameter("x", ValueType.Float)],
                    ValueType.Float,
                    (args) =>
                    {
                        double r = Math.Sqrt(args[0].AsDouble());
                        return new Value(r);
                    }
                )
            },
            {
                "pow",
                new(
                    "pow",
                    [new NativeFunctionParameter("x", ValueType.Int), new NativeFunctionParameter("y", ValueType.Int)],
                    ValueType.Int,
                    (args) =>
                    {
                        long l = (long)Math.Pow(args[0].AsLong(), args[1].AsLong());
                        return new Value(l);
                    }
                )
            },
            {
                "min",
                new(
                    "min",
                    [new NativeFunctionParameter("x", ValueType.Int), new NativeFunctionParameter("y", ValueType.Int)],
                    ValueType.Int,
                    (args) =>
                    {
                        long l = Math.Min(args[0].AsLong(), args[1].AsLong());
                        return new Value(l);
                    }
                )
            },
            {
                "max",
                new(
                    "max",
                    [new NativeFunctionParameter("x", ValueType.Int), new NativeFunctionParameter("y", ValueType.Int)],
                    ValueType.Int,
                    (args) =>
                    {
                        long l = Math.Max(args[0].AsLong(), args[1].AsLong());
                        return new Value(l);
                    }
                )
            },
        };
    }

    public AbstractFunctionDeclaration TryGetFunction(string name)
    {
        if (nativeFunctions.TryGetValue(name, out NativeFunction? nativeFunction))
        {
            return nativeFunction;
        }

        if (functions.TryGetValue(name, out AbstractFunctionDeclaration? function))
        {
            return function;
        }

        throw new ArgumentException($"Function '{name}' is not defined");
    }

    public AbstractFunctionDeclaration GetFunction(string name)
    {
        if (functions.TryGetValue(name, out AbstractFunctionDeclaration? function))
        {
            return function;
        }

        throw new ArgumentException($"Function '{name}' is not defined");
    }

    public void DefineFunction(AbstractFunctionDeclaration function)
    {
        if (!functions.TryAdd(function.Name, function))
        {
            throw new ArgumentException($"Function '{function.Name}' is already defined");
        }
    }

    public void PushScope(Scope scope)
    {
        scopes.Push(scope);
    }

    public void PopScope()
    {
        scopes.Pop();
    }

    /// <summary>
    /// Возвращает значение переменной или константы.
    /// </summary>
    public Value GetValue(string name)
    {
        foreach (Scope s in scopes)
        {
            if (s.TryGetVariable(name, out Value variable))
            {
                return variable;
            }
        }

        throw new ArgumentException($"Variable '{name}' is not defined");
    }

    /// <summary>
    /// Присваивает (изменяет) значение переменной.
    /// </summary>
    public void AssignVariable(string name, Value value)
    {
        foreach (Scope s in scopes.Reverse())
        {
            if (s.TryAssignVariable(name, value))
            {
                return;
            }
        }

        throw new ArgumentException($"Variable '{name}' is not defined");
    }

    /// <summary>
    /// Определяет переменную в текущей области видимости.
    /// </summary>
    public void DefineVariable(string name, Value value)
    {
        if (!scopes.Peek().TryDefineVariable(name, value))
        {
            throw new ArgumentException($"Variable '{name}' is already defined in this scope");
        }
    }
}