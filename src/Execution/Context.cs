namespace Execution;

using Ast.Declarations;

/// <summary>
/// Контекст выполнения программы (все переменные, константы и другие символы).
/// </summary>
public class Context
{
    private readonly IEnvironment environment;
    private readonly Stack<Scope> scopes = [];
    private readonly Dictionary<string, AbstractFunctionDeclaration> functions = [];

    public Context(IEnvironment environment)
    {
        this.environment = environment;
        functions["input"] = new NativeFunction("input", [], (args) => environment.ReadDecimal() ?? throw new ArgumentException("Couldn't read decimal from stdin"));
        functions["print"] = new NativeFunction("print", ["s"], (args) =>
        {
            this.environment.PrintDecimal(args[0]);
            return 0;
        });
        functions["abs"] = new NativeFunction("abs", ["x"], (args) => (decimal)Math.Abs((double)args[0]));
        functions["pow"] = new NativeFunction("pow", ["x", "y"], (args) => (decimal)Math.Pow((double)args[0], (double)args[1]));
        functions["max"] = new NativeFunction("max", ["x", "y"], (args) => (decimal)Math.Max((double)args[0], (double)args[1]));
        functions["min"] = new NativeFunction("min", ["x", "y"], (args) => (decimal)Math.Min((double)args[0], (double)args[1]));

        scopes.Push(new Scope());
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
    public decimal GetValue(string name)
    {
        foreach (Scope s in scopes)
        {
            if (s.TryGetVariable(name, out decimal variable))
            {
                return variable;
            }
        }

        throw new ArgumentException($"Variable '{name}' is not defined");
    }

    /// <summary>
    /// Присваивает (изменяет) значение переменной.
    /// </summary>
    public void AssignVariable(string name, decimal value)
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
    public void DefineVariable(string name, decimal value)
    {
        if (!scopes.Peek().TryDefineVariable(name, value))
        {
            throw new ArgumentException($"Variable '{name}' is already defined in this scope");
        }
    }
}