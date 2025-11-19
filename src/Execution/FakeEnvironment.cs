namespace Execution;

public class FakeEnvironment : IEnvironment
{
    private readonly List<decimal> evaluated = new List<decimal>();

    public decimal InputDecimal { get; set; }

    public decimal OutputDecimal { get; private set; }

    public decimal? ReadDecimal()
    {
        return InputDecimal;
    }

    public void PrintDecimal(decimal result)
    {
        evaluated.Add(result);
        OutputDecimal = result;
    }

    public List<decimal> GetEvaluated()
    {
        return evaluated;
    }
}