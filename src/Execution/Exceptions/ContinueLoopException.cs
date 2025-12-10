namespace Execution.Exceptions;

#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.
/// <summary>
/// Внутреннее исключение библиотеки, используется для выхода из текущего цикла.
/// </summary>
internal class ContinueLoopException : Exception
{
    public ContinueLoopException()
        : base("Loop continue")
    {
    }
}
#pragma warning restore RCS1194