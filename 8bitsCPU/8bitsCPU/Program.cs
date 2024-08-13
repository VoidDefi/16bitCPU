namespace _8bitsCPU
{
    public class Program
    {
        public static bool flag = true;
        public static void Main() 
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            Directory.CreateDirectory(dir + "Files");

            Computer computer = new Computer();

            while (flag)
            {
                computer.Update();
            }
        }
    }
}