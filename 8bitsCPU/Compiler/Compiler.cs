using _8bitsCPU;
using System.Text.RegularExpressions;
using System;
using System.Runtime.CompilerServices;

namespace Compiler
{
    public class Compiler
    {
        string[] CodeLines = new string[0];

        string AllCode;

        int CodeMinLine = -1;

        int CodeMaxLine = -1;

        int DataMinLine = -1;

        int DataMaxLine = -1;

        int currentSaveAddress;

        int MemorySize = 65536;

        byte SegmentCount = 2;

        public static bool Stop;

        Instruction instruction = new Instruction();

        public Compiler(string assemblerName, string memoryName)
        {
            AllCode = File.ReadAllText("Files\\" + assemblerName) + "\r";  
            Console.WriteLine("\nAssembler is load.");

            Console.WriteLine("Периевод в строчные буквы и удаление табов/пробелов.");
            AllCode = AllCode.ToLower();
            AllCode = AllCode.Replace(" ", "").Replace("\t", "");
            Memory NewHD = new(MemorySize, SegmentCount);

            Console.WriteLine("Разделение кода на строки и удаление пустых строк.\n");
            CodeLines = AllCode.Split(new char[1]{'\n'});
            CodeLines = CodeLines.Where(str => str != "\r").ToArray();
            CodeLines = CodeLines.Where(str => !str.StartsWith(";")).ToArray();

            for (int i = 0; i < CodeLines.Length; i++)
            {
                int coment = CodeLines[i].IndexOf(';');

                if (coment != -1)
                {
                    Console.WriteLine($"строка: {CodeLines[i].Replace("\r", "")} удаление комментария");
                    CodeLines[i] = CodeLines[i].Substring(0, coment);
                    CodeLines[i] += "\r";
                }

                if (CodeLines[i] == "code:\r")
                {
                    CodeMinLine = i;
                }

                if ((CodeLines[i] == "data:\r" || i == CodeLines.Length - 1) && CodeMinLine > -1)
                {
                    CodeMaxLine = i;
                }
            }

            Console.WriteLine(" ");

            int instructionCount = 0;
            Dictionary<string, int> labels = new Dictionary<string, int>();

            for (int i = 0; i < CodeLines.Length; i++)
            {
                int label = CodeLines[i].IndexOf('.');
                if (label != -1)
                {
                    int address = instructionCount * 5;
                    Console.WriteLine($"метка: {CodeLines[i].Replace("\r", "")} ссылается на ячейку {address}, что равно {instructionCount} инструкции");
                    labels.Add(CodeLines[i], (instructionCount - 1) * 5);
                    CodeLines[i] = CodeLines[i].Substring(label, CodeLines[i].Length);
                    CodeLines[i] = "\r";
                }

                else
                {
                    instructionCount++;
                }
            }

            Console.WriteLine(" ");
            CodeLines = CodeLines.Where(str => str != "\r").ToArray();
            CodeMaxLine = CodeLines.Length - 1;

            for (int i = CodeMinLine + 1; i <= CodeMaxLine; i++)
            {
                instruction.Define(CodeLines, i, labels);
                Console.WriteLine($"строка: {i} {CodeLines[i].Replace("\r", "")} = код инструкции {instruction.instruction}, операнд 1 {instruction.operand1}, операнд 2 {instruction.operand2}");

                if (Stop)
                {break;}

                Memory buffer = new Memory(5, 1);

                buffer.writeByte(0, Memory.Data, instruction.instruction);
                buffer.write(1, Memory.Data, instruction.operand1);
                buffer.write(3, Memory.Data, instruction.operand2);

                for(int j = 0; j < 5; j++)
                {
                    NewHD.writeByte(currentSaveAddress + j, Memory.Code, buffer.MemoryData[j]);
                    //NewHD.readByte()
                }
       
                instruction.Default();
                currentSaveAddress += 5;
            }

            NewHD.save("Files\\" + memoryName);

            if (!Stop)
            {
                Console.WriteLine("\nThe file is compiled! and saved.");
            }
        }
    }
}
