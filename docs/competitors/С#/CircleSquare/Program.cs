namespace ProgramMain
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Введите радиус круга:");
            double radiusCircle = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine($"Площадь круга: {Math.PI * radiusCircle * radiusCircle:F2}");
        }
    }
}
