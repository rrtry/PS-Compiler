namespace Lexer;

/// <summary>
///  Сканирует текст выражения, предоставляя три операции: Peek(N), Advance() и IsEnd().
/// </summary>
public class TextScanner(string expr)
{
    private readonly string _expr = expr;
    private int _position;

    /// <summary>
    ///  Читает на N символов вперёд текущей позиции (по умолчанию N=0).
    /// </summary>
    public char Peek(int n = 0)
    {
        int position = _position + n;
        return position >= _expr.Length ? '\0' : _expr[position];
    }

    /// <summary>
    ///  Сдвигает текущую позицию на один символ.
    /// </summary>
    public void Advance()
    {
        _position++;
    }

    public bool IsEnd()
    {
        return _position >= _expr.Length;
    }
}