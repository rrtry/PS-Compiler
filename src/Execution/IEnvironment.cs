namespace Execution;

/// <summary>
/// Представляет окружение для выполнения программы.
/// Прежде всего это функции ввода/вывода.
/// </summary>
public interface IEnvironment
{
    /// <summary>
    /// Список результатов выражений.
    /// </summary>
    public List<decimal> GetEvaluated();

    /// <summary>
    /// Функция чтения числа из stdin.
    /// </summary>
    public decimal? ReadDecimal();

    /// <summary>
    /// Функция записи в числа stdout.
    /// </summary>
    public void PrintDecimal(decimal result);
}