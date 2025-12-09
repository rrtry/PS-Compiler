namespace Lexer.UnitTests;

public class LexerTests
{
    [Theory]
    [MemberData(nameof(GetTokenizeSimpleExpressions))]
    [MemberData(nameof(GetTokenizeLanguageConstructiions))]
    public void Can_tokenize_source(string source, List<Token> expected)
    {
        List<Token> actual = Tokenize(source);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetLexicalStats))]
    public void Can_collect_stats(string filename, LexicalStats.Stats expected)
    {
        string root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"../../../"));
        string file = Path.Combine(root, "sources", filename);

        LexicalStats.Stats actual = LexicalStats.CollectFromFile(file);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, LexicalStats.Stats> GetLexicalStats()
    {
        return new TheoryData<string, LexicalStats.Stats>
        {
            {
                "square_root.lu",
                new LexicalStats.Stats(8, 8, 2, 0, 2, 4, 19)
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizeLanguageConstructiions()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "let b = ~255 | 1", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("b")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.BinNot),
                    new Token(TokenType.IntegerLiteral, new TokenValue(255)),
                    new Token(TokenType.Or),
                    new Token(TokenType.IntegerLiteral, new TokenValue(1))
                ]
            },
            {
                "let b = ~255 & 1", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("b")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.BinNot),
                    new Token(TokenType.IntegerLiteral, new TokenValue(255)),
                    new Token(TokenType.And),
                    new Token(TokenType.IntegerLiteral, new TokenValue(1))
                ]
            },
            {
                "let i = input();", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("i")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.Input),
                    new Token(TokenType.LeftParen),
                    new Token(TokenType.RightParen),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "fn main() { return 42; }", [
                    new Token(TokenType.Fn),
                    new Token(TokenType.Identifier, new TokenValue("main")),
                    new Token(TokenType.LeftParen),
                    new Token(TokenType.RightParen),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.Return),
                    new Token(TokenType.IntegerLiteral, new TokenValue(42)),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.RightBrace)
                ]
            },
            {
                "if x > 0 { print(\"positive\"); } else { print(\"non-positive\"); }", [
                    new Token(TokenType.If),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Greater),
                    new Token(TokenType.IntegerLiteral, new TokenValue(0)),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.Print),
                    new Token(TokenType.LeftParen),
                    new Token(TokenType.StringLiteral, new TokenValue("positive")),
                    new Token(TokenType.RightParen),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.RightBrace),
                    new Token(TokenType.Else),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.Print),
                    new Token(TokenType.LeftParen),
                    new Token(TokenType.StringLiteral, new TokenValue("non-positive")),
                    new Token(TokenType.RightParen),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.RightBrace)
                ]
            },
            {
                "while i < 10 { i++; }", [
                    new Token(TokenType.While),
                    new Token(TokenType.Identifier, new TokenValue("i")),
                    new Token(TokenType.Less),
                    new Token(TokenType.IntegerLiteral, new TokenValue(10)),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.Identifier, new TokenValue("i")),
                    new Token(TokenType.PlusPlus),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.RightBrace)
                ]
            },
            {
                "while i < 10 { i++; continue; }", [
                    new Token(TokenType.While),
                    new Token(TokenType.Identifier, new TokenValue("i")),
                    new Token(TokenType.Less),
                    new Token(TokenType.IntegerLiteral, new TokenValue(10)),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.Identifier, new TokenValue("i")),
                    new Token(TokenType.PlusPlus),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.Continue),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.RightBrace)
                ]
            },
            {
                "while i < 10 { i++; break; }", [
                    new Token(TokenType.While),
                    new Token(TokenType.Identifier, new TokenValue("i")),
                    new Token(TokenType.Less),
                    new Token(TokenType.IntegerLiteral, new TokenValue(10)),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.Identifier, new TokenValue("i")),
                    new Token(TokenType.PlusPlus),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.Break),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.RightBrace)
                ]
            },
            {
                "for x in numbers { print(x); }", [
                    new Token(TokenType.For),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.In),
                    new Token(TokenType.Identifier, new TokenValue("numbers")),
                    new Token(TokenType.LeftBrace),
                    new Token(TokenType.Print),
                    new Token(TokenType.LeftParen),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.RightParen),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.RightBrace)
                ]
            },
            {
                "let flag = true && !false || false;", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("flag")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.True),
                    new Token(TokenType.AndAnd),
                    new Token(TokenType.Not),
                    new Token(TokenType.False),
                    new Token(TokenType.OrOr),
                    new Token(TokenType.False),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "import math;", [
                    new Token(TokenType.Import),
                    new Token(TokenType.Identifier, new TokenValue("math")),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "let result = (a + b) * c - d / e % f ^ g;", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("result")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.LeftParen),
                    new Token(TokenType.Identifier, new TokenValue("a")),
                    new Token(TokenType.Plus),
                    new Token(TokenType.Identifier, new TokenValue("b")),
                    new Token(TokenType.RightParen),
                    new Token(TokenType.Star),
                    new Token(TokenType.Identifier, new TokenValue("c")),
                    new Token(TokenType.Minus),
                    new Token(TokenType.Identifier, new TokenValue("d")),
                    new Token(TokenType.Slash),
                    new Token(TokenType.Identifier, new TokenValue("e")),
                    new Token(TokenType.Percent),
                    new Token(TokenType.Identifier, new TokenValue("f")),
                    new Token(TokenType.Exp),
                    new Token(TokenType.Identifier, new TokenValue("g")),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "let compare = 1 != 2", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("compare")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntegerLiteral, new TokenValue(1)),
                    new Token(TokenType.NotEqual),
                    new Token(TokenType.IntegerLiteral, new TokenValue(2)),
                ]
            },
            {
                "let compare = 1 != 2 && 2 > 1 && 2 >= 1 && 1 < 2 && 1 <= 2", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("compare")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntegerLiteral, new TokenValue(1)),
                    new Token(TokenType.NotEqual),
                    new Token(TokenType.IntegerLiteral, new TokenValue(2)),
                    new Token(TokenType.AndAnd),
                    new Token(TokenType.IntegerLiteral, new TokenValue(2)),
                    new Token(TokenType.Greater),
                    new Token(TokenType.IntegerLiteral, new TokenValue(1)),
                    new Token(TokenType.AndAnd),
                    new Token(TokenType.IntegerLiteral, new TokenValue(2)),
                    new Token(TokenType.GreaterEqual),
                    new Token(TokenType.IntegerLiteral, new TokenValue(1)),
                    new Token(TokenType.AndAnd),
                    new Token(TokenType.IntegerLiteral, new TokenValue(1)),
                    new Token(TokenType.Less),
                    new Token(TokenType.IntegerLiteral, new TokenValue(2)),
                    new Token(TokenType.AndAnd),
                    new Token(TokenType.IntegerLiteral, new TokenValue(1)),
                    new Token(TokenType.LessEqual),
                    new Token(TokenType.IntegerLiteral, new TokenValue(2)),
                ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizeSimpleExpressions()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "let hello = \"Hello, World!\";", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("hello")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.StringLiteral, new TokenValue("Hello, World!")),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "let escapedQuotes = \"I said, \\\"Hello, World!\\\"\";", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("escapedQuotes")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.StringLiteral, new TokenValue("I said, \"Hello, World!\"")),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "let escapedNewLineAndQuotes = \"I said, \\\"Hello, \\\nWorld!\\\"\";", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("escapedNewLineAndQuotes")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.StringLiteral, new TokenValue("I said, \"Hello, \nWorld!\"")),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "let escapedTabAndQuotes = \"I said, \\\"Hello, \\\tWorld!\\\"\";", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("escapedTabAndQuotes")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.StringLiteral, new TokenValue("I said, \"Hello, \tWorld!\"")),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "let val = null;", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("val")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.Null),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "let number = 42;", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("number")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntegerLiteral, new TokenValue(42)),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "let number = -42;", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("number")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.Minus),
                    new Token(TokenType.IntegerLiteral, new TokenValue(42)),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "let number = 3.14;", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("number")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.FloatLiteral, new TokenValue(3.14m)),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "let number = -3.14;", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("number")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.Minus),
                    new Token(TokenType.FloatLiteral, new TokenValue(3.14m)),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "let numbers = [1, 2, 3, 4];", [

                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("numbers")),
                    new Token(TokenType.Assign),

                    new Token(TokenType.LeftBracket),
                    new Token(TokenType.IntegerLiteral, new TokenValue(1)),
                    new Token(TokenType.Comma),

                    new Token(TokenType.IntegerLiteral, new TokenValue(2)),
                    new Token(TokenType.Comma),

                    new Token(TokenType.IntegerLiteral, new TokenValue(3)),
                    new Token(TokenType.Comma),

                    new Token(TokenType.IntegerLiteral, new TokenValue(4)),
                    new Token(TokenType.RightBracket),

                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "let empty = [];", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("empty")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.LeftBracket),
                    new Token(TokenType.RightBracket),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "let number = 0xDEADC0DE;", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("number")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntegerLiteral, new TokenValue(0xDEADC0DE)),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "let number = 0b11011110101011011100000011011110;", [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, new TokenValue("number")),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntegerLiteral, new TokenValue(0b11011110101011011100000011011110)),
                    new Token(TokenType.Semicolon)
                ]
            },
        };
    }

    private List<Token> Tokenize(string sql)
    {
        List<Token> results = [];
        Lexer lexer = new(sql);

        for (Token t = lexer.ParseToken(); t.Type != TokenType.Eof; t = lexer.ParseToken())
        {
            results.Add(t);
        }

        return results;
    }
}
