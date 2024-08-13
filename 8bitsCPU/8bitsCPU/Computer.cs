using System.Drawing;

namespace _8bitsCPU
{
    public class Computer
    {
        public static Memory RAM = new Memory(65535, 2);

        public static Memory HD = new Memory(65535, 2); 

        public static CPU cpu = new CPU();

        public static Bios bios = new Bios();

        public static bool IsQuit;

        public Computer() 
        {
            load();
            //RAM.write(1, Memory.Data, 0b10100101);

            HD.load("Files\\HD");
            RAM = HD;
        }

        void load()
        {
            Console.WriteLine("Load completed!");

            /*byte B1 = 0B00011001;
            byte B2 = 0B00110001;

            bool[] Bools1 = new bool[8];
            bool[] Bools2 = new bool[8];

            for (int i = 0; i < 8; i++)
            {
                Bools1[i] = (B1 & (1 >> i)) != 0;
                Bools2[i] = (B2 & (1 >> i)) != 0;
            }

            Console.WriteLine(B1);
            Console.WriteLine(B2);

            for (int i = 0; i < 8; i++)
            {
                Console.Write(Bools1[i] ? 1 : 0);
            }

            Console.WriteLine();

            for (int i = 0; i < 8; i++)
            {
                Console.Write(Bools2[i] ? 1 : 0);
            }

            for (int i = 0; i < 8; i++)
            {
                Bools1[i] = (B1 & (1 >> i)) != 0;
                Bools2[i] = (B2 & (1 >> i)) != 0;
            }
            for (int i = 0; i < 8; i++)
            {
                if (Bools1[i])
                {
                    B1 |= (byte)(1 >> (7 - i));
                }
                if (Bools2[i])
                {
                    B2 |= (byte)(1 >> (7 - i));
                }
            }

            Console.WriteLine();

            Console.WriteLine(B1);
            Console.WriteLine(B2);

            bool[] Result = new bool[8];
            byte result = 0;

            for (int i = 0; i < 8; i++)
            {
                Result[i] = Bools1[i] && Bools2[i];
                Console.Write(Result[i]);
            }

            Console.WriteLine();

            for (int i = 0; i < 8; i++)
            {
                if (Bools1[i])
                {
                    result |= (byte)(1 >> (7 - i));
                }
            }
            Console.WriteLine(result);*/
        }

        public void Update()
        {
            if (!CPU.FlagQuit) 
            { 
                cpu.Update();
            }
            else
            {
                if (!IsQuit)
                {               
                    Console.WriteLine("");

                    if (CPU.CodeQuit == 0) 
                    {
                        Console.WriteLine($"The program completed without errors after going through {CPU.Counter} iterations.");
                    }

                    if (CPU.CodeQuit == 1)
                    {
                        Console.WriteLine($"The program ended with errors after going through {CPU.Counter} iterations.");
                    }
    
                    Console.WriteLine($"The carriage has stopped at {CPU.carriagePos} memory cell.");
                    IsQuit = true;
                }
            }
        }
    }
}
