namespace ProgramMain
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Введите действительное число:");
            double number = Convert.ToDouble(Console.ReadLine());
            if (number >= 0)
            {
                Console.WriteLine($"Корень числа {number} равен {Math.Sqrt(number):F2}");
            }
            else
            {
                Console.WriteLine("ERROR");
            }
        }
    }
}