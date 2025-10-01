
namespace ProgramName
{
    public static class Programm
    {
        private static void Main(string[] args)
        {
            string? str = Console.ReadLine();
            if (!string.IsNullOrEmpty(str))
            {
                char[] chars = str.ToCharArray();
                Array.Reverse(chars);
                string reversed = new string(chars);
                Console.WriteLine($"Реверсированная строка: {reversed}");
            }
            else
            {
                Console.WriteLine("Строка пуста или не введена.");
            }
        }
    }
}