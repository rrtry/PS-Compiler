namespace Parser.UnitTests;

public class ParserTests
{
    [Theory]
    [MemberData(nameof(GetNumberLiterals))]
    public void Can_tokenize_literals(string source, List<decimal> expected)
    {
        Parser p = new Parser(source);
        List<decimal> evaluated = p.ParseStatements();
        Assert.Equal(expected, evaluated);
    }

    public static TheoryData<string, List<decimal>> GetNumberLiterals()
    {
        return new TheoryData<string, List<decimal>>
        {
            {
                "42;", new List<decimal> { 42 }
            },
            {
                "0x2a;", new List<decimal> { 0x2a }
            },
            {
                "0b101010;", new List<decimal> { 0b101010 }
            },
            {
                "3.14;", new List<decimal> { 3.14m }
            },
        };
    }
}
