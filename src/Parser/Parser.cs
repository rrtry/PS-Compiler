namespace Parser;

using Lexer;

/// <summary>
/// Выполняет синтаксический разбор SQL-запросов.
/// Грамматика описана в файле `docs/specification/expressions-grammar.md`.
/// </summary>
public class Parser
{
    private readonly TokenStream tokens;
    private List<decimal> evaluated = new List<decimal>();

    public Parser(string source)
    {
        tokens = new TokenStream(source);
    }

    public List<decimal> ParseStatements()
    {
        while (tokens.Peek().Type != TokenType.Semicolon)
        {
            evaluated.Add(ParsePrimary());
        }

        return evaluated;
    }

    private decimal ParsePrimary()
    {
        Token t = tokens.Peek();
        switch (t.Type)
        {
            case TokenType.IntegerLiteral:
            case TokenType.FloatLiteral:
                tokens.Advance();
                return t.Value!.ToDecimal();

            default:
                throw new Exception($"Unexpected token {t.Type}");
        }
    }

    private bool DoesMatch(TokenType expected)
    {
        try
        {
            Match(expected);
            return true;
        }
        catch (UnexpectedLexemeException)
        {
            return false;
        }
    }

    private void Match(TokenType expected)
    {
        Token t = tokens.Peek();
        if (t.Type != expected)
        {
            throw new UnexpectedLexemeException(expected, t);
        }

        tokens.Advance();
    }
}