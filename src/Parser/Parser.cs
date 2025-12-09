namespace Parser;

using Lexer;
using Execution;
using Ast;
using Ast.Expressions;
using Ast.Declarations;
using Ast.Statements;

/// <summary>
/// Грамматика описана в файле `docs/specification/expressions-grammar.md`.
/// </summary>
public class Parser
{
    private readonly TokenStream tokens;

    private readonly Context context;
    private readonly IEnvironment environment;
    private readonly AstEvaluator evaluator;

    public Parser(string source)
    {
        tokens = new TokenStream(source);
        environment = new FakeEnvironment();
        context = new Context(environment);
        evaluator = new AstEvaluator(context);
    }

    public Parser(Context context, IEnvironment environment, string source)
    {
        tokens = new TokenStream(source);
        this.context = context;
        this.environment = environment;
        this.evaluator = new AstEvaluator(context);
    }

    public List<decimal> Parse()
    {
        while (tokens.Peek().Type != TokenType.Eof)
        {
            AstNode node = ParseStatement();
            evaluator.Evaluate(node);
        }

        return environment.GetEvaluated();
    }

    private AstNode ParseStatement()
    {
        Token token = tokens.Peek();
        AstNode evaluated;

        switch (token.Type)
        {
            case TokenType.Let:
                evaluated = ParseVariableDefinition();
                Match(TokenType.Semicolon);
                break;

            case TokenType.Print:
                evaluated = ParsePrintStatement();
                Match(TokenType.Semicolon);
                break;

            case TokenType.If:
                evaluated = ParseIfStatement();
                break;

            case TokenType.While:
                evaluated = ParseWhileLoopStatement();
                break;

            case TokenType.For:
                evaluated = ParseForLoopStatement();
                break;

            default:
                evaluated = ParseExpression();
                Match(TokenType.Semicolon);
                break;
        }

        return evaluated;
    }

    private IfElseStatement ParseIfStatement()
    {
        Match(TokenType.If);

        Match(TokenType.LeftParen);
        Expression condition = ParseExpression();
        Match(TokenType.RightParen);

        BlockStatement thenBlock = ParseBlockStatement();
        BlockStatement? elseBlock = null;

        if (MatchOptional(TokenType.Else))
        {
            elseBlock = ParseBlockStatement();
        }

        return new IfElseStatement(condition, thenBlock, elseBlock);
    }

    private BlockStatement ParseBlockStatement()
    {
        Match(TokenType.LeftBrace);
        List<AstNode> statements = [];

        while (tokens.Peek().Type != TokenType.RightBrace &&
               tokens.Peek().Type != TokenType.Eof)
        {
            AstNode stmt = ParseStatement();
            statements.Add(stmt);
        }

        Match(TokenType.RightBrace);
        return new BlockStatement(statements);
    }

    private WhileLoopStatement ParseWhileLoopStatement()
    {
        Match(TokenType.While);

        Match(TokenType.LeftParen);
        Expression condition = ParseExpression();
        Match(TokenType.RightParen);

        BlockStatement body = ParseBlockStatement();
        return new WhileLoopStatement(condition, body);
    }

    private ForLoopStatement ParseForLoopStatement()
    {
        Match(TokenType.For);
        Match(TokenType.LeftParen);
        AstNode initialization = ParseStatement();

        if (initialization is not VariableDeclaration and not AssignmentExpression)
        {
            throw new Exception("Invalid for loop initialization");
        }

        string name = initialization switch
        {
            VariableDeclaration varDecl => varDecl.Name,
            AssignmentExpression assignExpr => assignExpr.Name,
            _ => throw new Exception("Unreachable code")
        };

        Expression condition = ParseExpression();
        Match(TokenType.Semicolon);

        Expression increment = ParseExpression();
        Match(TokenType.RightParen);

        BlockStatement body = ParseBlockStatement();
        return new ForLoopStatement(name, initialization, condition, increment, body);
    }

    /// <summary>
    /// expression = logical_or ;.
    /// </summary>
    private Expression ParseExpression() => ParseLogicalOr();

    /// <summary>
    /// (* Логическое ИЛИ *)
    /// logical_or = logical_and, { "||", logical_and } ;.
    /// </summary>
    private Expression ParseLogicalOr()
    {
        Expression left = ParseLogicalAnd();
        while (tokens.Peek().Type == TokenType.OrOr)
        {
            tokens.Advance();
            Expression right = ParseLogicalAnd();
            left = new BinaryOperationExpression(left, BinaryOperation.Or, right);
        }

        return left;
    }

    /// <summary>
    /// (* Логическое И *)
    /// logical_and = equality, { "&&", equality } ;.
    /// </summary>
    private Expression ParseLogicalAnd()
    {
        Expression left = ParseEquality();
        while (tokens.Peek().Type == TokenType.AndAnd)
        {
            tokens.Advance();
            Expression right = ParseEquality();
            left = new BinaryOperationExpression(left, BinaryOperation.And, right);
        }

        return left;
    }

    /// <summary>
    /// (* Равенство *)
    /// equality = relational, { ("==" | "!="), relational } ;.
    /// </summary>
    private Expression ParseEquality()
    {
        Expression left = ParseRelational();
        if (tokens.Peek().Type == TokenType.EqualEqual ||
            tokens.Peek().Type == TokenType.NotEqual)
        {
            Token op = tokens.Advance();
            Expression right = ParseRelational();
            left = new BinaryOperationExpression(
                left,
                op.Type == TokenType.EqualEqual ? BinaryOperation.Equal : BinaryOperation.NotEqual,
                right
            );
        }

        return left;
    }

    /// <summary>
    /// (* Сравнение *)
    /// additive, { ("<" | ">" | "<=" | ">="), additive }.
    /// </summary>
    private Expression ParseRelational()
    {
        Expression left = ParseAdditive();
        if (tokens.Peek().Type == TokenType.Less ||
            tokens.Peek().Type == TokenType.Greater ||
            tokens.Peek().Type == TokenType.LessEqual ||
            tokens.Peek().Type == TokenType.GreaterEqual)
        {
            Token op = tokens.Advance();
            Expression right = ParseAdditive();
            switch (op.Type)
            {
                case TokenType.Less:
                    left = new BinaryOperationExpression(left, BinaryOperation.LessThan, right);
                    break;
                case TokenType.Greater:
                    left = new BinaryOperationExpression(left, BinaryOperation.GreaterThan, right);
                    break;
                case TokenType.LessEqual:
                    left = new BinaryOperationExpression(left, BinaryOperation.LessThanOrEqual, right);
                    break;
                case TokenType.GreaterEqual:
                    left = new BinaryOperationExpression(left, BinaryOperation.GreaterThanOrEqual, right);
                    break;
            }
        }

        return left;
    }

    /// <summary>
    /// variable_declaration =
    /// "let", identifier, ["=", expression] ;.
    /// </summary>
    private AssignmentExpression ParseVariableAssignment()
    {
        Token identifier = tokens.Advance();
        string name = identifier.Value!.ToString();

        Match(TokenType.Assign);
        Expression value = ParseExpression();
        return new AssignmentExpression(name, value);
    }

    /// <summary>
    /// assignment_statement =
    /// identifier, "=", expression ;.
    /// </summary>
    private VariableDeclaration ParseVariableDefinition()
    {
        tokens.Advance();

        Token identifier = Match(TokenType.Identifier);
        string name = identifier.Value!.ToString();

        Match(TokenType.Assign);
        Expression value = ParseExpression();
        return new VariableDeclaration(name, value);
    }

    private Expression ParsePrintStatement()
    {
        tokens.Advance();
        return ParseFunctionCall("print");
    }

    /// <summary>
    /// additive = multiplicative, { ("+" | "-"), multiplicative } ;.
    /// </summary>
    private Expression ParseAdditive()
    {
        Expression left = ParseMultiplicative();

        while (tokens.Peek().Type == TokenType.Plus ||
               tokens.Peek().Type == TokenType.Minus)
        {
            Token op = tokens.Advance();
            Expression right = ParseMultiplicative();
            left = new BinaryOperationExpression(
                left,
                op.Type == TokenType.Plus ? BinaryOperation.Add : BinaryOperation.Substract,
                right
            );
        }

        return left;
    }

    /// <summary>
    /// multiplicative  = power, { ("*" | "/" | "%"), power } ;.
    /// </summary>
    private Expression ParseMultiplicative()
    {
        Expression left = ParsePower();

        while (tokens.Peek().Type == TokenType.Star ||
               tokens.Peek().Type == TokenType.Slash ||
               tokens.Peek().Type == TokenType.Percent)
        {
            Token op = tokens.Advance();
            Expression right = ParsePower();

            left = op.Type switch
            {
                TokenType.Star => new BinaryOperationExpression(left, BinaryOperation.Multiply, right),
                TokenType.Slash => new BinaryOperationExpression(left, BinaryOperation.Divide, right),
                TokenType.Percent => new BinaryOperationExpression(left, BinaryOperation.Modulo, right),
                _ => throw new Exception("Invalid operator")
            };
        }

        return left;
    }

    /// <summary>
    /// power = unary, [ ("^" | "**"), power ] ;.
    /// </summary>
    private Expression ParsePower()
    {
        Expression left = ParseUnary();

        // TODO: заменить ^ на **
        if (tokens.Peek().Type == TokenType.Exp)
        {
            tokens.Advance();
            Expression right = ParsePower(); // правоассоциативно
            left = new BinaryOperationExpression(left, BinaryOperation.Power, right);
        }

        return left;
    }

    /// <summary>
    /// unary = [ ("+" | "-") ], primary ;.
    /// </summary>
    private Expression ParseUnary()
    {
        if (tokens.Peek().Type == TokenType.Plus)
        {
            tokens.Advance();
            return new UnaryOperationExpression(UnaryOperation.Plus, ParseUnary());
        }
        else if (tokens.Peek().Type == TokenType.Minus)
        {
            tokens.Advance();
            return new UnaryOperationExpression(UnaryOperation.Minus, ParseUnary());
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
    private Expression ParsePrimary()
    {
        Token token = tokens.Peek();
        switch (token.Type)
        {
            case TokenType.IntegerLiteral:
            case TokenType.FloatLiteral:
                tokens.Advance();
                return new LiteralExpression(token.Value!.ToDecimal());

            case TokenType.Input:
                tokens.Advance();
                return ParseFunctionCall("input");

            case TokenType.Identifier:

                if (tokens.Peek(1).Type == TokenType.Assign)
                {
                    return ParseVariableAssignment();
                }

                string name = tokens.Advance().Value!.ToString();
                if (tokens.Peek().Type == TokenType.LeftParen)
                {
                    return ParseFunctionCall(name);
                }

                return new VariableExpression(name);

            case TokenType.LeftParen:
            case TokenType.AndAnd:
            case TokenType.OrOr:
                tokens.Advance();
                Expression expr = ParseExpression();
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
    private FunctionCallExpression ParseFunctionCall(string name)
    {
        Match(TokenType.LeftParen);
        List<Expression> args = new();

        if (tokens.Peek().Type != TokenType.RightParen)
        {
            do
            {
                args.Add(ParseExpression());
            }
            while (MatchOptional(TokenType.Comma));
        }

        Match(TokenType.RightParen);
        return new FunctionCallExpression(name, args);
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