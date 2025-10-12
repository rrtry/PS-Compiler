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

    public Token ParseToken()
    {
        SkipWhiteSpacesAndComments();

        if (_scanner.IsEnd())
        {
            return new Token(TokenType.Eof);
        }

        // Разбор числовых литералов
        char c = _scanner.Peek();
        int octal = 10;

        if (char.IsAsciiDigit(c))
        {
            if ((c - '0') == 0)
            {
                if (_scanner.Peek(1) == 'b')
                {
                    octal = 2;
                }
                else if (_scanner.Peek(1) == 'x')
                {
                    octal = 16;
                }

                if (octal != 10)
                {
                    _scanner.Advance();
                    _scanner.Advance();
                }
            }

            return ParseNumericLiteral(octal);
        }

        if (c == '"')
        {
            return ParseStringLiteral();
        }

        // TODO: добавить побитовые операторы (& | << >> ~)
        switch (c)
        {
            case '{':
                _scanner.Advance();
                return new Token(TokenType.LeftBrace);
            case '}':
                _scanner.Advance();
                return new Token(TokenType.RightBrace);
            case '[':
                _scanner.Advance();
                return new Token(TokenType.LeftBracket);
            case ']':
                _scanner.Advance();
                return new Token(TokenType.RightBracket);
            case '|':
                _scanner.Advance();
                if (_scanner.Peek() == '|')
                {
                    _scanner.Advance();
                    return new Token(TokenType.OrOr);
                }

                // Пока возвращаем ошибку
                return new Token(TokenType.Unknown);
            case '&':
                _scanner.Advance();
                if (_scanner.Peek() == '&')
                {
                    _scanner.Advance();
                    return new Token(TokenType.AndAnd);
                }

                // Пока возвращаем ошибку
                return new Token(TokenType.Unknown);
            case '!':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    return new Token(TokenType.NotEqual);
                }

                return new Token(TokenType.Not);
            case '=':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    return new Token(TokenType.EqualEqual);
                }

                return new Token(TokenType.Assign);
            case ';':
                _scanner.Advance();
                return new Token(TokenType.Semicolon);
            case ',':
                _scanner.Advance();
                return new Token(TokenType.Comma);
            case '+':
                _scanner.Advance();
                if (_scanner.Peek() == '+')
                {
                    _scanner.Advance();
                    return new Token(TokenType.PlusPlus);
                }

                return new Token(TokenType.Plus);
            case '-':
                _scanner.Advance();
                if (_scanner.Peek() == '-')
                {
                    _scanner.Advance();
                    return new Token(TokenType.MinusMinus);
                }

                return new Token(TokenType.Minus);
            case '*':
                _scanner.Advance();
                return new Token(TokenType.Star);
            case '/':
                _scanner.Advance();
                return new Token(TokenType.Slash);
            case '%':
                _scanner.Advance();
                return new Token(TokenType.Percent);
            case '^':
                _scanner.Advance();
                return new Token(TokenType.Exp);
            case '<':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.LessEqual);
                }

                return new Token(TokenType.Less);
            case '>':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.GreaterEqual);
                }

                return new Token(TokenType.Greater);
            case '(':
                _scanner.Advance();
                return new Token(TokenType.LeftParen);
            case ')':
                _scanner.Advance();
                return new Token(TokenType.RightParen);
        }

        return ParseIdentifierOrKeyword();
    }

    /// <summary>
    ///  Распознаёт идентификаторы и ключевые слова.
    ///  Идентификаторы обрабатываются по правилам:
    ///     identifier = [letter | '_' ], { letter | digit | '_' } ;
    ///     letter = "a" | "b" | .. | "z" | unicode_letter ;
    ///     digit = "0" | "1" | .. | "9" ;
    ///     unicode_letter — любая буква Unicode.
    /// </summary>
    private Token ParseIdentifierOrKeyword()
    {
        string value = _scanner.Peek().ToString();
        _scanner.Advance();

        for (char c = _scanner.Peek(); char.IsLetter(c) || c == '_' || char.IsAsciiDigit(c); c = _scanner.Peek())
        {
            value += c;
            _scanner.Advance();
        }

        // Проверяем на совпадение с ключевым словом.
        if (Keywords.TryGetValue(value, out TokenType type))
        {
            return new Token(type);
        }

        // Возвращаем токен идентификатора.
        return new Token(TokenType.Identifier, new TokenValue(value));
    }

    /// <summary>
    ///  Распознаёт литерал числа по правилам:
    ///     number = digits_sequence, [ ".", digits_sequence ] ;
    ///     digits_sequence = digit { digit } ;
    ///     digit = "0" | "1" | ... | "9" ;.
    /// </summary>
    private Token ParseNumericLiteral(int octal)
    {
        decimal value = GetDigitValue(_scanner.Peek(), octal);
        _scanner.Advance();

        // Читаем целую часть числа.
        for (char c = _scanner.Peek(); IsDigitValue(c, octal); c = _scanner.Peek())
        {
            value = value * octal + GetDigitValue(c, octal);
            _scanner.Advance();
        }

        // Читаем дробную часть числа.
        if (_scanner.Peek() == '.')
        {
            if (octal != 10)
            {
                return new Token(TokenType.Unknown, new TokenValue(value));
            }

            _scanner.Advance();
            decimal factor = 0.1m;
            for (char c = _scanner.Peek(); char.IsAsciiDigit(c); c = _scanner.Peek())
            {
                _scanner.Advance();
                value += factor * GetDigitValue(c, 10);
                factor *= 0.1m;
            }
        }
        else
        {
            return new Token(TokenType.IntegerLiteral, new TokenValue(value));
        }

        return new Token(TokenType.FloatLiteral, new TokenValue(value));

        // Локальная функция, проверяющая является ли символ числом
        static bool IsDigitValue(char ch, int octal)
        {
            switch (octal)
            {
                case 2:
                    return ch == '1' || ch == '0';
                case 10:
                    return ch >= '0' && ch <= '9';
                case 16:
                    return (ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'F') || (ch >= 'a' && ch <= 'f');
                default:
                    throw new ArgumentException($"Invalid base: {octal}");
            }
        }

        // Локальная функция для получения числа из символа цифры.
        static int GetDigitValue(char ch, int octal)
        {
            if (octal != 2 && octal != 10 && octal != 16)
            {
                throw new ArgumentException($"Invalid base: {octal}");
            }

            if ((ch == '1' || ch == '0') && octal == 2)
            {
                return ch - '0';
            }

            if (ch >= '0' && ch <= '9' && (octal == 10 || octal == 16))
            {
                return ch - '0';
            }
            else if (ch >= 'A' && ch <= 'F' && octal == 16)
            {
                return ch - 'A' + 10;
            }
            else if (ch >= 'a' && ch <= 'f' && octal == 16)
            {
                return ch - 'a' + 10;
            }
            else
            {
                throw new ArgumentException($"Invalid digit {ch} for base: {octal}");
            }
        }
    }

    /// <summary>
    ///  Распознаёт литерал числа по правилам:
    ///     string = quote, { string_element }, quote ;
    ///     quote = "'" ;
    ///     string_element = char | escape_sequence ;
    ///     char = ^"'".
    /// </summary>
    private Token ParseStringLiteral()
    {
        _scanner.Advance();

        string contents = "";
        while (_scanner.Peek() != '\"')
        {
            if (_scanner.IsEnd())
            {
                // Ошибка: строка, не закрытая кавычкой.
                return new Token(TokenType.Unknown, new TokenValue(contents));
            }

            // Проверяем наличие escape-последовательности.
            if (TryParseStringLiteralEscapeSequence(out char unescaped))
            {
                contents += unescaped;
            }
            else
            {
                contents += _scanner.Peek();
                _scanner.Advance();
            }
        }

        _scanner.Advance();

        return new Token(TokenType.StringLiteral, new TokenValue(contents));
    }

    /// <summary>
    ///  Распознаёт escape-последовательности по правилам:
    ///     escape_sequence = "\", "\" | "\", "'" ;
    ///  Возвращает null при появлении неизвестных escape-последовательностей.
    /// </summary>
    private bool TryParseStringLiteralEscapeSequence(out char unescaped)
    {
        if (_scanner.Peek() == '\\')
        {
            _scanner.Advance();
            if (_scanner.Peek() == '\"')
            {
                _scanner.Advance();
                unescaped = '\"';
                return true;
            }

            if (_scanner.Peek() == '\\')
            {
                _scanner.Advance();
                unescaped = '\\';
                return true;
            }
        }

        unescaped = '\0';
        return false;
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
        if (_scanner.Peek() == '/' && _scanner.Peek(1) == '/')
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
