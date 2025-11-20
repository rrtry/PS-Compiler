using Execution;

namespace Interpreter.Specs;

public class InterpreterTests
{
    [Theory]
    [MemberData(nameof(GetExamplePrograms))]
    public void Can_interpret_simple_programs(string source, Tuple<List<decimal>, List<decimal>> tuple)
    {
        List<decimal> programInput = tuple.Item1;
        List<decimal> expectedOutput = tuple.Item2;

        Context context = new Context();
        FakeEnvironment environment = new FakeEnvironment();
        environment.SetProgramInput(programInput);

        List<decimal> evaluated = environment.GetEvaluated();
        Interpreter interpreter = new Interpreter(context, environment);
        interpreter.Execute(source);

        Assert.Equal(expectedOutput.Count, evaluated.Count);

        for (int i = 0; i < evaluated.Count; i++)
        {
            if (!decimal.Equals(decimal.Round(expectedOutput[i], 2), decimal.Round(evaluated[i], 2)))
            {
                throw new Xunit.Sdk.XunitException($"Expected {expectedOutput[i]} but got {evaluated[i]}");
            }
        }
    }

    public static TheoryData<string, Tuple<List<decimal>, List<decimal>>> GetExamplePrograms()
    {
        return new TheoryData<string, Tuple<List<decimal>, List<decimal>>>
        {
            {
                "let x = input(); " +
                "let y = input(); " +
                "print(x + y);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 1, 2 },
                    new List<decimal> { 3 }
                )
            },
            {
                "let x = input(); " +
                "let y = input(); " +
                "y = 3; " +
                "print(x + y);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 1, 2 },
                    new List<decimal> { 4 }
                )
            },
            {
                "let x = input(); " +
                "let y = input(); " +
                "let result = pow(x + y, 0.5); " +
                "print(result);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 2, 2 },
                    new List<decimal> { 2 }
                )
            },
            {
                "let x = 2; " +
                "let y = 2; " +
                "let result = (x + y) ^ 0.5; " +
                "print(result);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 2, 2 },
                    new List<decimal> { 2 }
                )
            },
            {
                "let PI = input(); " +
                "let radius = input(); " +
                "let area = (radius ^ 2) * PI; " +
                "print(area);",
                new Tuple<List<decimal>, List<decimal>>(
                    new List<decimal> { 3.14159265358979323846m, 10m },
                    new List<decimal> { 100.0m * 3.14159265358979323846m }
                )
            },
        };
    }
}
