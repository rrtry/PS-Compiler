namespace Parser;

using Lexer;

/// <summary>
/// Грамматика описана в файле `docs/specification/expressions-grammar.md`.
/// </summary>
public class Parser
{
    private readonly TokenStream tokens;
    private readonly List<decimal> evaluated = new List<decimal>();
    private readonly Dictionary<string, decimal> scope = new Dictionary<string, decimal>();

    public Parser(string source)
    {
        tokens = new TokenStream(source);
    }

    public List<decimal> ParseStatements()
    {
        while (tokens.Peek().Type != TokenType.Eof)
        {
            ParseStatement();
        }

        return evaluated;
    }

    private void ParseStatement()
    {
        Token t = tokens.Peek();
        switch (t.Type)
        {
            case TokenType.Let:
                ParseVariableDefinition();
                break;

            case TokenType.Print:
                ParsePrintStatement();
                break;

            default:
                if (t.Type == TokenType.Identifier && tokens.Peek(1).Type == TokenType.Assign)
                {
                    ParseVariableAssignment();
                    break;
                }
                else
                {
                    decimal value = ParseExpression();
                    Match(TokenType.Semicolon);
                    evaluated.Add(value);
                    break;
                }
        }
    }

    /// <summary>
    /// variable_declaration =
    /// "let", identifier, ["=", expression] ;.
    /// </summary>
    private void ParseVariableAssignment()
    {
        Token identifier = tokens.Advance();
        string name = identifier.Value!.ToString();

        if (!scope.ContainsKey(name))
        {
            throw new Exception($"SyntaxError: {name} is not declared in the scope");
        }

        Match(TokenType.Assign);
        decimal value = ParseExpression();
        Match(TokenType.Semicolon);

        scope[name] = value;
    }

    /// <summary>
    /// assignment_statement =
    /// identifier, "=", expression ;
    /// </summary>
    private void ParseVariableDefinition()
    {
        tokens.Advance();

        Token identifier = Match(TokenType.Identifier);
        string name = identifier.Value!.ToString();

        if (scope.ContainsKey(name))
        {
            throw new Exception($"SyntaxError: {name} is already declared in the scope");
        }

        Match(TokenType.Assign);
        decimal value = ParseExpression();
        Match(TokenType.Semicolon);

        scope[name] = value;
    }

    private void ParsePrintStatement()
    {
        tokens.Advance();
        decimal value = ParseExpression();
        Match(TokenType.Semicolon);
        evaluated.Add(value);
    }

    private decimal ParseExpression() => ParseAdditive();

    /// <summary>
    /// additive = multiplicative, { ("+" | "-"), multiplicative } ;.
    /// </summary>
    private decimal ParseAdditive()
    {
        decimal left = ParseMultiplicative();

        while (tokens.Peek().Type == TokenType.Plus ||
               tokens.Peek().Type == TokenType.Minus)
        {
            Token op = tokens.Advance();
            decimal right = ParseMultiplicative();
            left = op.Type == TokenType.Plus ? left + right : left - right;
        }

        return left;
    }

    /// <summary>
    /// multiplicative  = power, { ("*" | "/" | "%"), power } ;.
    /// </summary>
    private decimal ParseMultiplicative()
    {
        decimal left = ParsePower();

        while (tokens.Peek().Type == TokenType.Star ||
               tokens.Peek().Type == TokenType.Slash ||
               tokens.Peek().Type == TokenType.Percent)
        {
            Token op = tokens.Advance();
            decimal right = ParsePower();

            left = op.Type switch
            {
                TokenType.Star => left * right,
                TokenType.Slash => left / right,
                TokenType.Percent => left % right,
                _ => throw new Exception("Invalid operator")
            };
        }

        return left;
    }

    /// <summary>
    /// power = unary, [ ("^" | "**"), power ] ;.
    /// </summary>
    private decimal ParsePower()
    {
        decimal left = ParseUnary();

        // TODO: заменить ^ на **
        if (tokens.Peek().Type == TokenType.Exp)
        {
            tokens.Advance();
            decimal right = ParsePower(); // правоассоциативно
            left = (decimal)Math.Pow((double)left, (double)right);
        }

        return left;
    }

    /// <summary>
    /// unary = [ ("+" | "-") ], primary ;.
    /// </summary>
    private decimal ParseUnary()
    {
        if (tokens.Peek().Type == TokenType.Plus)
        {
            tokens.Advance();
            return ParseUnary();
        }
        else if (tokens.Peek().Type == TokenType.Minus)
        {
            tokens.Advance();
            return -ParseUnary();
        }
        else
        {
            return ParsePrimary();
        }
    }

    /// <summary>
    /// primary = number
    ///           | constant
    ///           | identifier
    ///           | function_call
    ///           | "(", expression, ")" ;.
    /// </summary>
    private decimal ParsePrimary()
    {
        Token token = tokens.Peek();
        switch (token.Type)
        {
            case TokenType.IntegerLiteral:
            case TokenType.FloatLiteral:
                tokens.Advance();
                return token.Value!.ToDecimal();

            case TokenType.Identifier:
                string name = tokens.Advance().Value!.ToString();
                if (tokens.Peek().Type == TokenType.LeftParen)
                {
                    return ParseFunctionCall(name);
                }

                return scope.TryGetValue(token.Value!.ToString()!, out decimal val) ? val : throw new Exception(
                    $"Trying to access undeclared variable ${token.Value!}"
                );

            case TokenType.LeftParen:
                tokens.Advance();
                decimal expr = ParseExpression();
                Match(TokenType.RightParen);
                return expr;

            default:
                throw new Exception($"Unexpected token {token.Type}");
        }
    }

    /// <summary>
    /// function_call   = identifier, "(", [ argument_list ], ")" ;
    /// argument_list   = expression, { ",", expression } ;.
    /// </summary>
    private decimal ParseFunctionCall(string name)
    {
        Match(TokenType.LeftParen);
        List<decimal> args = new();

        if (tokens.Peek().Type != TokenType.RightParen)
        {
            do
            {
                args.Add(ParseExpression());
            }
            while (MatchOptional(TokenType.Comma));
        }

        Match(TokenType.RightParen);

        return name switch
        {
            "input" => decimal.Parse(Console.ReadLine() ?? throw new Exception("Could not parse decimal from stdin")),
            "abs" => (decimal)Math.Abs((double)args[0]),
            "pow" => (decimal)Math.Pow((double)args[0], (double)args[1]),
            "max" => (decimal)Math.Max((double)args[0], (double)args[1]),
            "min" => (decimal)Math.Min((double)args[0], (double)args[1]),
            _ => throw new Exception($"Unknown function {name}")
        };
    }

    private bool MatchOptional(TokenType type)
    {
        if (tokens.Peek().Type == type)
        {
            tokens.Advance();
            return true;
        }

        return false;
    }

    private Token Match(TokenType expected)
    {
        Token t = tokens.Peek();
        if (t.Type != expected)
        {
            throw new UnexpectedLexemeException(expected, t);
        }

        return tokens.Advance();
    }
}