namespace Lexer;

public static class LexicalStats
{
    public record class Stats(
        int keywords,
        int identifiers,
        int intLiterals,
        int floatLiterals,
        int strLiterals,
        int operators,
        int otherLexems
    )
    {
        public override string ToString()
        {
            return $@"keywords: {keywords}
                    identifiers: {identifiers}
                    int literals: {intLiterals}
                    float literals: {floatLiterals}
                    string literals: {strLiterals}
                    operators: {operators}
                    other lexemes: {otherLexems}";
        }
    }

    public static Stats CollectFromFile(string path)
    {
        string source = File.ReadAllText(path);
        List<Token> tokens = Tokenize(source);

        int keywords = tokens.Count(token => Lexer.Keywords.ContainsValue(token.Type));
        int operators = tokens.Count(token => Lexer.Operators.Contains(token.Type));
        int otherLexems = tokens.Count(token => Lexer.OtherLexems.Contains(token.Type));

        int identifiers = tokens.Count(token => token.Type.Equals(TokenType.Identifier));
        int intLiterals = tokens.Count(token => token.Type.Equals(TokenType.IntegerLiteral));
        int strLiterals = tokens.Count(token => token.Type.Equals(TokenType.StringLiteral));
        int floatLiterals = tokens.Count(token => token.Type.Equals(TokenType.FloatLiteral));

        return new Stats(
            keywords, identifiers, intLiterals, floatLiterals, strLiterals, operators, otherLexems
        );
    }

    public static List<Token> Tokenize(string source)
    {
        List<Token> results = [];
        Lexer lexer = new(source);

        for (Token t = lexer.ParseToken(); t.Type != TokenType.Eof; t = lexer.ParseToken())
        {
            results.Add(t);
        }

        return results;
    }
}