namespace Interpreter;

using Parser;
using Execution;

public class Interpreter
{
    private readonly Context context;
    private readonly IEnvironment environment;

    public Interpreter()
    {
        environment = new ConsoleEnvironment();
        context = new Context(environment);
    }

    public Interpreter(Context context, IEnvironment environment)
    {
        this.context = context;
        this.environment = environment;
    }

    /// <summary>
    /// Выполнение программы.
    /// </summary>
    /// <param name="sourceCode">Исходный код программы.</param>
    public void Execute(string sourceCode)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            throw new ArgumentException("Source code cannot be null or empty", nameof(sourceCode));
        }

        Parser parser = new(context, environment, sourceCode);
        parser.Parse();
    }
}