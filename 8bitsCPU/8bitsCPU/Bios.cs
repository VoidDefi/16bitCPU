using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace _8bitsCPU
{
    public class Bios
    {
        public static int DisplayDraw = 0;

        public static int DisplayClear = 1;

        public const int PixelSize = 4;

        const string DisplayPath = "Files\\Display.dim";

        const string UpdatePath = "Files\\DisplayUpdate.duf";

        public static CPU.Register BX = new(0);  //BX Регистр абсциссы (стондартный, 16бит), (Bios X) 

        public static CPU.Register BY = new(0);  //BY Регистр орденаты (стондартный, 16бит), (Bios Y) 

        ConsoleGraphics graphics = new ConsoleGraphics();

        public Bios() 
        {

        }

        public void unload()
        {

        }
        Thread thread;

        public object Update(int type, object[] args = null)
        {
            Console.CursorVisible = false;
            object obj = null;

            if (type == DisplayDraw)
            {
                graphics.DrawRectangle((BX.data + 2) * PixelSize, BY.data * PixelSize, PixelSize, PixelSize, 0x00FF00);
            }

            else if (type == DisplayClear)
            {
                graphics.DrawRectangle(0, 0, Console.WindowWidth * 10, Console.WindowHeight * 20, 0x000000);
            }

            else
            {
                //Console.WriteLine("");
                Console.WriteLine($"BIOS Error: {type} - this command is not exist");
                CPU.CodeQuit = 1;
            }

            return obj;
        }
    }
}
