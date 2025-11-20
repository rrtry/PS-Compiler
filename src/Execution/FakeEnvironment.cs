namespace Execution;

public class FakeEnvironment : IEnvironment
{
    private readonly List<decimal> evaluated = new List<decimal>();

    private int inputIndex = 0;

    private List<decimal> programInput = new List<decimal>();

    public decimal? ReadDecimal()
    {
        if (inputIndex >= programInput.Count)
        {
            return null;
        }

        return programInput[inputIndex++];
    }

    public void PrintDecimal(decimal result)
    {
        evaluated.Add(result);
    }

    public List<decimal> GetEvaluated()
    {
        return evaluated;
    }

    public void SetProgramInput(List<decimal> input)
    {
        programInput = input;
    }
}