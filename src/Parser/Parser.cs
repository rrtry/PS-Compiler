namespace Parser;

using Lexer;
using Execution;

/// <summary>
/// Грамматика описана в файле `docs/specification/expressions-grammar.md`.
/// </summary>
public class Parser
{
    private readonly TokenStream tokens;

    private readonly Context context = new Context();
    private readonly IEnvironment environment = new FakeEnvironment();

    public Parser(string source)
    {
        tokens = new TokenStream(source);
    }

    public Parser(Context context, IEnvironment environment, string source)
    {
        tokens = new TokenStream(source);
        this.context = context;
        this.environment = environment;
    }

    public List<decimal> Parse()
    {
        context.PushScope(new Scope());
        while (tokens.Peek().Type != TokenType.Eof)
        {
            ParseStatement();
        }

        return environment.GetEvaluated();
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
                    environment.PrintDecimal(value);
                    break;
                }
        }
    }

    /// <summary>
    /// expression = logical_or ;.
    /// </summary>
    private decimal ParseExpression() => ParseLogicalOr();

    /// <summary>
    /// (* Логическое ИЛИ *)
    /// logical_or = logical_and, { "||", logical_and } ;.
    /// </summary>
    private decimal ParseLogicalOr()
    {
        decimal left = ParseLogicalAnd();
        while (tokens.Peek().Type == TokenType.OrOr)
        {
            Token op = tokens.Advance();
            decimal right = ParseLogicalAnd();
            left = (long)left != 0L || (long)right != 0L ? 1 : 0;
        }

        return left;
    }

    /// <summary>
    /// (* Логическое И *)
    /// logical_and = equality, { "&&", equality } ;.
    /// </summary>
    private decimal ParseLogicalAnd()
    {
        decimal left = ParseEquality();
        while (tokens.Peek().Type == TokenType.AndAnd)
        {
            decimal right = ParseEquality();
            left = (long)left != 0L && (long)right != 0L ? 1 : 0;
        }

        return left;
    }

    /// <summary>
    /// (* Равенство *)
    /// equality = relational, { ("==" | "!="), relational } ;.
    /// </summary>
    private decimal ParseEquality()
    {
        decimal left = ParseRelational();
        if (tokens.Peek().Type == TokenType.EqualEqual ||
            tokens.Peek().Type == TokenType.NotEqual)
        {
            Token op = tokens.Advance();
            decimal right = ParseRelational();
            left = op.Type == TokenType.EqualEqual ?
                   ((long)left == (long)right ? 1 : 0) :
                   ((long)left != (long)right ? 1 : 0);
        }

        return left;
    }

    /// <summary>
    /// (* Сравнение *)
    /// additive, { ("<" | ">" | "<=" | ">="), additive }.
    /// </summary>
    private decimal ParseRelational()
    {
        decimal left = ParseAdditive();
        if (tokens.Peek().Type == TokenType.Less ||
            tokens.Peek().Type == TokenType.Greater ||
            tokens.Peek().Type == TokenType.LessEqual ||
            tokens.Peek().Type == TokenType.GreaterEqual)
        {
            Token op = tokens.Advance();
            decimal right = ParseAdditive();
            switch (op.Type)
            {
                case TokenType.Less:
                    left = left < right ? 1 : 0;
                    break;
                case TokenType.Greater:
                    left = left > right ? 1 : 0;
                    break;
                case TokenType.LessEqual:
                    left = left <= right ? 1 : 0;
                    break;
                case TokenType.GreaterEqual:
                    left = left >= right ? 1 : 0;
                    break;
            }
        }

        return left;
    }

    /// <summary>
    /// variable_declaration =
    /// "let", identifier, ["=", expression] ;.
    /// </summary>
    private void ParseVariableAssignment()
    {
        Token identifier = tokens.Advance();
        string name = identifier.Value!.ToString();

        Match(TokenType.Assign);
        decimal value = ParseExpression();
        Match(TokenType.Semicolon);

        context.AssignVariable(name, value);
    }

    /// <summary>
    /// assignment_statement =
    /// identifier, "=", expression ;.
    /// </summary>
    private void ParseVariableDefinition()
    {
        tokens.Advance();

        Token identifier = Match(TokenType.Identifier);
        string name = identifier.Value!.ToString();

        Match(TokenType.Assign);
        decimal value = ParseExpression();
        Match(TokenType.Semicolon);

        context.DefineVariable(name, value);
    }

    private void ParsePrintStatement()
    {
        tokens.Advance();
        decimal value = ParseExpression();
        Match(TokenType.Semicolon);

        environment.PrintDecimal(value);
    }

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

            case TokenType.Input:
                tokens.Advance();
                return ParseFunctionCall("input");

            case TokenType.Identifier:
                string name = tokens.Advance().Value!.ToString();
                if (tokens.Peek().Type == TokenType.LeftParen)
                {
                    return ParseFunctionCall(name);
                }

                return context.GetValue(token.Value!.ToString());

            case TokenType.LeftParen:
            case TokenType.AndAnd:
            case TokenType.OrOr:
                tokens.Advance();
                decimal expr = ParseExpression();
                if (token.Type == TokenType.LeftParen)
                {
                    Match(TokenType.RightParen);
                }

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
            "input" => environment.ReadDecimal() ?? throw new ArgumentException("Couldn't read decimal from stdin"),
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