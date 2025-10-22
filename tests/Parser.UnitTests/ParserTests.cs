namespace Parser.UnitTests;

public class ParserTests
{
    [Theory]
    [MemberData(nameof(GetNumberLiterals))]
    public void Can_parse_literals(string source, List<decimal> expected)
    {
        Parser p = new Parser(source);
        List<decimal> evaluated = p.ParseStatements();
        Assert.Equal(expected, evaluated);
    }

    [Theory]
    [MemberData(nameof(GetExpressions))]
    public void Can_parse_expressions(string source, List<decimal> expected)
    {
        Parser p = new Parser(source);
        List<decimal> evaluated = p.ParseStatements();
        Assert.Equal(expected, evaluated);
    }

    public static TheoryData<string, List<decimal>> GetExpressions()
    {
        return new TheoryData<string, List<decimal>>
        {
            { "1 + 2;",         new List<decimal> { 3 } },
            { "1 + 6 / 2;",     new List<decimal> { 4 } },
            { "12 / 4 / 3;",    new List<decimal> { 1 } },
            { "12 / 6 / 2;",    new List<decimal> { 1 } },
            { "12 / (6 / 2);",  new List<decimal> { 4 } },
            { "2 * 3 + 4;",     new List<decimal> { 10 } },
            { "2 + 3 * 4;",     new List<decimal> { 14 } },
            { "(2 + 3) * 5;",   new List<decimal> { 25 } },
            { "2 ^ 3 ^ 2;",     new List<decimal> { 512 } },
            { "2 ^ 3 + 4 * 5;", new List<decimal> { 28 } },
        };
    }

    public static TheoryData<string, List<decimal>> GetNumberLiterals()
    {
        return new TheoryData<string, List<decimal>>
        {
            { "42;", new List<decimal> { 42 } },
            { "0x2a;", new List<decimal> { 0x2a } },
            { "0b101010;", new List<decimal> { 0b101010 } },
            { "3.14;", new List<decimal> { 3.14m } },
        };
    }
}
