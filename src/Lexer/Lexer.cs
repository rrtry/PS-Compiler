namespace Lexer;

public class Lexer
{
    private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
    {
        { "fn",       TokenType.Fn },
        { "let",      TokenType.Let },
        { "return",   TokenType.Return },
        { "if",       TokenType.If },
        { "else",     TokenType.Else },
        { "while",    TokenType.While },
        { "for",      TokenType.For },
        { "in",       TokenType.In },
        { "break",    TokenType.Break },
        { "continue", TokenType.Continue },
        { "true",     TokenType.True },
        { "false",    TokenType.False },
        { "null",     TokenType.Null },
        { "print",    TokenType.Print },
        { "import",   TokenType.Import },
    };

    private readonly TextScanner _scanner;

    public Lexer(string sql)
    {
        _scanner = new TextScanner(sql);
    }

    /// <summary>
    ///  Пропускает пробельные символы и комментарии, пока не встретит что-либо иное.
    /// </summary>
    private void SkipWhiteSpacesAndComments()
    {
        do
        {
            SkipWhiteSpaces();
        }
        while (TryParseMultilineComment() || TryParseSingleLineComment());
    }

    /// <summary>
    ///  Пропускает пробельные символы, пока не встретит иной символ.
    /// </summary>
    private void SkipWhiteSpaces()
    {
        while (char.IsWhiteSpace(_scanner.Peek()))
        {
            _scanner.Advance();
        }
    }

    /// <summary>
    ///  Пропускает многострочный комментарий в виде `/* ...текст */`,
    ///  пока не встретит `*/`.
    /// </summary>
    private bool TryParseMultilineComment()
    {
        if (_scanner.Peek() == '/' && _scanner.Peek(1) == '*')
        {
            do
            {
                _scanner.Advance();
            }
            while (!(_scanner.Peek() == '*' && _scanner.Peek(1) == '/'));

            _scanner.Advance();
            _scanner.Advance();
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Пропускает однострочный комментарий в виде `-- ...текст`,
    ///  пока не встретит конец строки (его оставляет).
    /// </summary>
    private bool TryParseSingleLineComment()
    {
        if (_scanner.Peek() == '-' && _scanner.Peek(1) == '-')
        {
            do
            {
                _scanner.Advance();
            }
            while (_scanner.Peek() != '\n' && _scanner.Peek() != '\r');

            return true;
        }

        return false;
    }
}
