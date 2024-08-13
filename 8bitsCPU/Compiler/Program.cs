namespace Compiler
{
    public class Program
    {
        public static void Main()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            Directory.CreateDirectory(dir + "Files");
            //File.Create(dir + "Code.asm");
            //File.Create(dir + "HD.rmy");

            Console.WriteLine("The compiler has started. For compilation, enter the name of the .asm file (without extension) and the name of the .rmy file (without extension).");
            Console.Write("\nAssembler file: ");
            string assemblerName = Console.ReadLine() + ".asm";

            Console.Write("Memory file: ");
            string memoryName = Console.ReadLine();

            if (assemblerName != null && memoryName != null)
            { 
                Compiler compiler = new(assemblerName, memoryName);
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}