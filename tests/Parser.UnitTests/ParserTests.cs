using Xunit.Sdk;

namespace Parser.UnitTests;

public class ParserTests
{
    [Theory]
    [MemberData(nameof(GetNumberLiterals))]
    [MemberData(nameof(GetExpressions))]
    [MemberData(nameof(GetFunctionCalls))]
    public void Can_parse_statements(string source, List<string> expected)
    {
        Parser p = new Parser(source);
        List<string> evaluated = p.Eval();
        Assert.Equal(expected, evaluated);
    }

    [Theory]
    [MemberData(nameof(GetExamplePrograms))]
    public void Can_interpret_simple_programs(string source, List<string> expected)
    {
        Parser p = new Parser(source);
        List<string> evaluated = p.Eval();
        Assert.Equal(expected.Count, evaluated.Count);

        for (int i = 0; i < evaluated.Count; i++)
        {
            if (expected[i] != evaluated[i])
            {
                throw new XunitException($"Expected {expected[i]} but got {evaluated[i]}");
            }
        }
    }

    public static TheoryData<string, List<string>> GetExamplePrograms()
    {
        return new TheoryData<string, List<string>>
        {
            {
                "let x = 1; " +
                "let y = 2; " +
                "print(x + y);",
                new List<string> { "3" }
            },
            {
                "let x = 1; " +
                "let y = 2; " +
                "y = 3; " +
                "print(x + y);",
                new List<string> { "4" }
            },
            {
                "let x = 2; " +
                "let y = 2; " +
                "let z = x + y;" +
                "let result = sqrt(z); " +
                "print(result);",
                new List<string> { "2.00" }
            },
            {
                "let PI = 3.14159265358979323846; " +
                "let radius = 10; " +
                "let area = (radius ^ 2) * PI; " +
                "print(area);",
                new List<string> { "314.16" }
            },
        };
    }

    public static TheoryData<string, List<string>> GetFunctionCalls()
    {
        return new TheoryData<string, List<string>>
        {
            { "print(abs(-42));",      new List<string> { "42" } },
            { "print(max(1, 3));",     new List<string> { "3" } },
            { "print(min(1, 3));",     new List<string> { "1" } },
            { "print(min(1 - 2, 3));", new List<string> { "-1" } },
            {
                "let x = 1; let y = 2; print(abs(min(x - y, 3)));",
                new List<string> { "1" }
            },
            {
                "let x = 1; let y = 2; print(max(x - y, 3));",
                new List<string> { "3" }
            },
            {
                "let x = 1; let y = 2; print(min(x - y, 3));",
                new List<string> { "-1" }
            },
            {
                "let x = 2; let y = 3; print(pow(x, y));",
                new List<string> { "8" }
            },
            {
                "let x = 2; let y = 3; let p = pow(x, y); print(p);",
                new List<string> { "8" }
            },
        };
    }

    public static TheoryData<string, List<string>> GetExpressions()
    {
        return new TheoryData<string, List<string>>
        {
            { "print(4 % 2);",           new List<string> { "0" } },
            { "print(1 + 2);",           new List<string> { "3" } },
            { "print(1 + 6 / 2);",       new List<string> { "4" } },
            { "print(-12 / -4 / -3);",   new List<string> { "-1" } },
            { "print(-12 / 4 / 3);",     new List<string> { "-1" } },
            { "print(12 / 4 / 3);",      new List<string> { "1" } },
            { "print(12 / 6 / 2);",      new List<string> { "1" } },
            { "print(12 / (6 / 2));",    new List<string> { "4" } },
            { "print(2 * 3 + 4);",       new List<string> { "10" } },
            { "print(2 + 3 * 4);",       new List<string> { "14" } },
            { "print((2 + 3) * 5);",     new List<string> { "25" } },
            { "print(2 ^ 3 ^ 2);",       new List<string> { "512" } },
            { "print(2 ^ 3 + 4 * 5);",   new List<string> { "28" } },
            { "print(2 ^ (2 + 2) * 5);", new List<string> { "80" } },
            { "print((-5) ^ 2);",        new List<string> { "25" } },
            { "print(-5 + 10);",         new List<string> { "5" } },
        };
    }

    public static TheoryData<string, List<string>> GetNumberLiterals()
    {
        return new TheoryData<string, List<string>>
        {
            { "print(42);",       new List<string> { "42" } },
            { "print(0x2a);",     new List<string> { "42" } },
            { "print(0b101010);", new List<string> { "42" } },
            { "print(3.14);",     new List<string> { "3.14" } },
            { "print(-(-42));",   new List<string> { "42" } },
            { "print(+(-42));",   new List<string> { "-42" } },
        };
    }
}
