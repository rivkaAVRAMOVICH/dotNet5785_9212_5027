partial class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        Welcome9212();
        Welcome5027();
        Console.ReadKey();
    }
    static partial void Welcome5027();
    private static void Welcome9212()
    {
        Console.WriteLine("Enter your name: ");
        string userName = Console.ReadLine();
        Console.WriteLine("{0}, welcome to my first console application", userName);
    }
}
