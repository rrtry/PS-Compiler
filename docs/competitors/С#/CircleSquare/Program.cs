namespace ProgramMain
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Введите радиус круга:");
            double radiusCircle = Convert.ToDouble(Console.ReadLine());
            if (radiusCircle > 0)
            {
                Console.WriteLine($"Площадь круга: {Math.PI * radiusCircle * radiusCircle:F2}");
            }
            else
            {
                Console.WriteLine("Радиус должен быть положительным числом");
            }
        }
    }
}
