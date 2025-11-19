namespace Execution;

public class ConsoleEnvironment : IEnvironment
{
    private readonly List<decimal> evaluated = new List<decimal>();

    public List<decimal> GetEvaluated()
    {
        return evaluated;
    }

    public decimal? ReadDecimal()
    {
        string? line = Console.ReadLine();
        if (line == null)
        {
            return null;
        }

        return decimal.Parse(line);
    }

    public void PrintDecimal(decimal result)
    {
        evaluated.Add(result);
        Console.WriteLine(result);
    }
}