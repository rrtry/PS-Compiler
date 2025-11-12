namespace Parser.UnitTests;

public class ParserTests
{
    [Theory]
    [MemberData(nameof(GetNumberLiterals))]
    [MemberData(nameof(GetExpressions))]
    [MemberData(nameof(GetFunctionCalls))]
    public void Can_parse_statements(string source, List<decimal> expected)
    {
        Parser p = new Parser(source);
        List<decimal> evaluated = p.ParseStatements();
        Assert.Equal(expected, evaluated);
    }

    [Theory]
    [MemberData(nameof(GetExamplePrograms))]
    public void Can_interpret_simple_programs(string source, List<decimal> expected)
    {
        Parser p = new Parser(source);
        List<decimal> evaluated = p.ParseStatements();
        Assert.Equal(expected.Count, evaluated.Count);

        for (int i = 0; i < evaluated.Count; i++)
        {
            if (!decimal.Equals(decimal.Round(expected[i], 2), decimal.Round(evaluated[i], 2)))
            {
                throw new Xunit.Sdk.XunitException($"Expected {expected[i]} but got {evaluated[i]}");
            }
        }
    }

    public static TheoryData<string, List<decimal>> GetExamplePrograms()
    {
        return new TheoryData<string, List<decimal>>
        {
            {
                "let x = 1; " +
                "let y = 2; " +
                "print(x + y);",
                new List<decimal> { 3 }
            },
            {
                "let x = 1; " +
                "let y = 2; " +
                "y = 3; " +
                "print(x + y);",
                new List<decimal> { 4 }
            },
            {
                "let x = 2; " +
                "let y = 2; " +
                "let result = pow(x + y, 0.5); " +
                "print(result);",
                new List<decimal> { 2 }
            },
            {
                "let x = 2; " +
                "let y = 2; " +
                "let result = (x + y) ^ 0.5; " +
                "print(result);",
                new List<decimal> { 2 }
            },
            {
                "let PI = 3.14159265358979323846; " +
                "let radius = 10; " +
                "let area = (radius ^ 2) * PI; " +
                "print(area);",
                new List<decimal> { new decimal(100.0 * 3.14159265358979323846) }
            },
        };
    }

    public static TheoryData<string, List<decimal>> GetFunctionCalls()
    {
        return new TheoryData<string, List<decimal>>
        {
            { "abs(-42);",      new List<decimal> { 42 } },
            { "max(1, 3);",     new List<decimal> { 3 } },
            { "min(1, 3);",     new List<decimal> { 1 } },
            { "min(1 - 2, 3);", new List<decimal> { -1 } },
            {
                "let x = 1; let y = 2; abs(min(x - y, 3));",
                new List<decimal> { 1 }
            },
            {
                "let x = 1; let y = 2; max(x - y, 3);",
                new List<decimal> { 3 }
            },
            {
                "let x = 1; let y = 2; min(x - y, 3);",
                new List<decimal> { -1 }
            },
            {
                "let x = 2; let y = 3; pow(x, y);",
                new List<decimal> { 8 }
            },
            {
                "let x = 2; let y = 3; let p = pow(x, y); print(p);",
                new List<decimal> { 8 }
            },
        };
    }

    public static TheoryData<string, List<decimal>> GetExpressions()
    {
        return new TheoryData<string, List<decimal>>
        {
            { "4 % 2;",           new List<decimal> { 0 } },
            { "1 + 2;",           new List<decimal> { 3 } },
            { "1 + 6 / 2;",       new List<decimal> { 4 } },
            { "-12 / -4 / -3;",   new List<decimal> { -1 } },
            { "-12 / 4 / 3;",     new List<decimal> { -1 } },
            { "12 / 4 / 3;",      new List<decimal> { 1 } },
            { "12 / 6 / 2;",      new List<decimal> { 1 } },
            { "12 / (6 / 2);",    new List<decimal> { 4 } },
            { "2 * 3 + 4;",       new List<decimal> { 10 } },
            { "2 + 3 * 4;",       new List<decimal> { 14 } },
            { "(2 + 3) * 5;",     new List<decimal> { 25 } },
            { "2 ^ 3 ^ 2;",       new List<decimal> { 512 } },
            { "2 ^ 3 + 4 * 5;",   new List<decimal> { 28 } },
            { "2 ^ (2 + 2) * 5;", new List<decimal> { 80 } },
            { "(-5) ^ 2;",        new List<decimal> { 25 } },
            { "-5 + 10;",         new List<decimal> { 5 } },
        };
    }

    public static TheoryData<string, List<decimal>> GetNumberLiterals()
    {
        return new TheoryData<string, List<decimal>>
        {
            { "42;",       new List<decimal> { 42 } },
            { "0x2a;",     new List<decimal> { 0x2a } },
            { "0b101010;", new List<decimal> { 0b101010 } },
            { "3.14;",     new List<decimal> { 3.14m } },
            { "-(-42);",   new List<decimal> { 42 } },
            { "+(-42);",   new List<decimal> { -42 } },
        };
    }
}
