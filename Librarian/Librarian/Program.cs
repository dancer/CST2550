using Librarian.Services;

namespace Librarian
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var consoleInterface = new MinimalConsoleInterface();
                consoleInterface.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"application error: {ex.Message}");
                Console.WriteLine("press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
