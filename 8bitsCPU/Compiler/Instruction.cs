using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Compiler
{
    public class Instruction
    {
        public byte instruction;

        public ushort operand1;

        public ushort operand2;

        public string instructionText = "";

        public string operand1Text = "";

        public string operand2Text = "";

        public const byte DataAddress = 0;

        public const byte CodeAddress = 1;

        public const byte Register = 2;

        public const byte Number = 3;

        public void Default() 
        { 
            instruction = 0; 
            operand1 = 0;
            operand2 = 0;
            instructionText = "";
            operand1Text = "";
            operand2Text = "";
        }

        public void Define(string[] line, int i, Dictionary<string, int> labels)
        {
            Match match = Regex.Match(line[i], "~(.*?)~");

            if (match.Success)
            {
                int labelAddress = -1;
                if (!labels.TryGetValue("." + match.Groups[1].Value + "\r", out labelAddress))
                {
                    Console.WriteLine($"This label \"{match.Groups[1].Value}\" does not exist");
                    Compiler.Stop = true;
                    return;
                }

                line[i] = line[i].Remove(match.Groups[1].Index - 1, match.Groups[1].Value.Length + 2);
                line[i] = line[i].Insert(match.Groups[1].Index - 1, "0a0c" + labelAddress);
            }

            if (line[i].Length >= 3)
            {
                instructionText = line[i].Substring(0, 3);
            }

            switch (instructionText)
            {
                case "mov":                   
                    FindOperand1(line[i]);                  
                    FindOperand2(line[i]);

                    if(DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == DataAddress)
                    {
                        instruction = 1;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, DataAddress);
                    }

                    else if (DefineTypeOperand(operand1Text) == DataAddress && DefineTypeOperand(operand2Text) == Register)
                    {
                        instruction = 2;
                        operand1 = DefineOperand(operand1Text, DataAddress);
                        operand2 = DefineOperand(operand2Text, Register);
                    }

                    else if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == Number)
                    {
                        instruction = 14;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, Number);
                    }

                    else if (DefineTypeOperand(operand1Text) == DataAddress && DefineTypeOperand(operand2Text) == Number)
                    {
                        instruction = 15;
                        operand1 = DefineOperand(operand1Text, DataAddress);
                        operand2 = DefineOperand(operand2Text, Number);
                    }

                    else if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == Register)
                    {
                        instruction = 34;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, Register);
                    }

                    break;

                case "add":
                    FindOperand1(line[i]);
                    FindOperand2(line[i]);

                    if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == Register)
                    {
                        instruction = 3;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, Register);
                    }

                    else if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == Number)
                    {
                        instruction = 4;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, Number);
                    }

                    break;

                case "sub":
                    FindOperand1(line[i]);
                    FindOperand2(line[i]);

                    if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == Register)
                    {
                        instruction = 5;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, Register);
                    }

                    else if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == Number)
                    {
                        instruction = 6;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, Number);
                    }

                    break;

                case "nln":
                    FindOperand1(line[i]);
                    FindOperand2(line[i]);

                    if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == Number)
                    {
                        instruction = 8;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, Number);
                    }

                    else if (DefineTypeOperand(operand1Text) == Number && DefineTypeOperand(operand2Text) == Number)
                    {
                        instruction = 9;
                        operand1 = DefineOperand(operand1Text, Number);
                        operand2 = DefineOperand(operand2Text, Number);
                    }

                    else if (DefineTypeOperand(operand1Text) == DataAddress && DefineTypeOperand(operand2Text) == Number)
                    {
                        instruction = 10;
                        operand1 = DefineOperand(operand1Text, DataAddress);
                        operand2 = DefineOperand(operand2Text, Number);
                    }

                    break;

                case "mvc":
                    instruction = 11;
                    FindOperand1(line[i]);
                    operand1 = DefineOperand(operand1Text, CodeAddress);
                    break;

                case "mbc":
                    instruction = 12;
                    break;

                case "end":
                    instruction = 13;
                    break;

                case "and":
                    FindOperand1(line[i]);
                    FindOperand2(line[i]);

                    instruction = 16;
                    operand1 = DefineOperand(operand1Text, Register);
                    operand2 = DefineOperand(operand2Text, Register);
                    break;

                case "orl":
                    FindOperand1(line[i]);
                    FindOperand2(line[i]);

                    instruction = 17;
                    operand1 = DefineOperand(operand1Text, Register);
                    operand2 = DefineOperand(operand2Text, Register);
                    break;

                case "xor":
                    FindOperand1(line[i]);
                    FindOperand2(line[i]);

                    instruction = 18;
                    operand1 = DefineOperand(operand1Text, Register);
                    operand2 = DefineOperand(operand2Text, Register);
                    break;

                case "not":
                    FindOperand1(line[i]);
                    FindOperand2(line[i]);

                    instruction = 19;
                    operand1 = DefineOperand(operand1Text, Register);
                    break;

                case "shr":
                    FindOperand1(line[i]);
                    FindOperand2(line[i]);

                    if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == Register)
                    {
                        instruction = 20;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, Register);
                    }

                    else if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == Number)
                    {
                        instruction = 21;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, Number);
                    }
                    break;

                case "shl":
                    FindOperand1(line[i]);
                    FindOperand2(line[i]);

                    if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == Register)
                    {
                        instruction = 22;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, Register);
                    }

                    else if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == Number)
                    {
                        instruction = 23;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, Number);
                    }
                    break;

                case "ifl":
                    FindOperand1(line[i]);
                    FindOperand2(line[i]);

                    if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == Register)
                    {
                        instruction = 24;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, Register);
                    }

                    else if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == DataAddress)
                    {
                        instruction = 25;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, DataAddress);
                    }

                    else if (DefineTypeOperand(operand1Text) == DataAddress && DefineTypeOperand(operand2Text) == DataAddress)
                    {
                        instruction = 26;
                        operand1 = DefineOperand(operand1Text, DataAddress);
                        operand2 = DefineOperand(operand2Text, DataAddress);
                    }

                    else if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == Number)
                    {
                        instruction = 35;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, Number);
                    }

                    break;

                case "fls":
                    instruction = 27;
                    break;

                case "tre":
                    instruction = 28;
                    break;

                case "mor":
                    instruction = 29;
                    break;

                case "les":
                    instruction = 30;
                    break;

                case "bir":
                    FindOperand1(line[i]);
                    FindOperand2(line[i]);

                    instruction = 31;
                    operand1 = DefineOperand(operand1Text, Number);
                    operand2 = DefineOperand(operand2Text, Number);
                    break;

                case "dmd":
                    FindOperand1(line[i]);
                    FindOperand2(line[i]);

                    if (DefineTypeOperand(operand1Text) == Register && DefineTypeOperand(operand2Text) == Register)
                    {
                        instruction = 32;
                        operand1 = DefineOperand(operand1Text, Register);
                        operand2 = DefineOperand(operand2Text, Register);
                    }

                    else if (DefineTypeOperand(operand1Text) == DataAddress && DefineTypeOperand(operand2Text) == Register)
                    {
                        instruction = 33;
                        operand1 = DefineOperand(operand1Text, DataAddress);
                        operand2 = DefineOperand(operand2Text, Register);
                    }
                    break;

                default:
                    Console.WriteLine($"line {i + 1}, instruction '{instructionText}' does not exist.");
                    Compiler.Stop = true;
                    break;
            }         
        }

        void FindOperand1(string line)
        {
            Match match = Regex.Match(line, "'(.*?),");
            Match match1 = Regex.Match(line, "'(.*?)\\r");

            if (match.Success)
            {
                operand1Text = match.Groups[1].Value;
            }

            else if (match1.Success)
            {
                operand1Text = match1.Groups[1].Value;
            }

            else
            {
                Console.WriteLine($"line {line}, operand1 does not exist");
                Compiler.Stop = true;
            }
        }

        void FindOperand2(string line)
        {
            Match match = Regex.Match(line, ",(.*?)\\r");

            if (match.Success)
            {
                operand2Text = match.Groups[1].Value;
            }

            else
            {
                Console.WriteLine($"line {line}, operand2 does not exist");
                Compiler.Stop = true;
            }
        }

        ushort DefineOperand(string operand, byte type)
        {
            ushort outOperand = 0;
            switch (type)
            {
                case DataAddress:
                    Match match = Regex.Match(operand, "0a0d(.*?)\\Z");
                    if (match.Success) {outOperand = ushort.Parse(match.Groups[1].Value);}
                    break;

                case CodeAddress:
                    Match match1 = Regex.Match(operand, "0a0c(.*?)\\Z");
                    if (match1.Success) {outOperand = ushort.Parse(match1.Groups[1].Value);}
                    break;

                case Register:
                    switch (operand)
                    {
                        case "dt":
                            outOperand = 0;
                            break;

                        case "ir":
                            outOperand = 1;
                            break;

                        case "af":
                            outOperand = 2;
                            break;

                        case "ct":
                            outOperand = 3;
                            break;

                        case "gr":
                            outOperand = 4;
                            break;

                        case "cm":
                            outOperand = 5;
                            break;

                        case "jn":
                            outOperand = 6;
                            break;

                        case "mt":
                            outOperand = 7;
                            break;

                        case "bx":
                            outOperand = 8;
                            break;

                        case "by":
                            outOperand = 9;
                            break;
                    }
                    break;

                case Number:
                    Match match2 = Regex.Match(operand, "\\?char:(.*?)\\Z");
                    Match match3 = Regex.Match(operand, "\\?char/u:(.*?)\\Z");
                    Match match4 = Regex.Match(operand, "\\?char/d:(.*?)\\Z");
                    Match match5 = Regex.Match(operand, "\\?char/esc:(.*?)\\Z");

                    if (match2.Success) 
                    {
                        outOperand = (ushort)match2.Groups[1].Value[0];
                    }

                    else if (match3.Success)
                    {
                        outOperand = (ushort)match3.Groups[1].Value.ToUpper()[0];
                    }

                    else if (match4.Success)
                    {
                        outOperand = (ushort)match4.Groups[1].Value.ToLower()[0];
                    }

                    else if (match5.Success)
                    {
                        char ESC = match5.Groups[1].Value[0];
                        char ResultESC = '\0';

                        switch (ESC)
                        {
                            case '0':
                                ResultESC = '\0';
                                break;

                            case 'a':
                                ResultESC = '\a';
                                break;

                            case 'b':
                                ResultESC = '\b';
                                break;

                            case 't':
                                ResultESC = '\t';
                                break;

                            case 'n':
                                ResultESC = '\n';
                                break;

                            case 'v':
                                ResultESC = '\v';
                                break;

                            case 'f':
                                ResultESC = '\f';
                                break;

                            case 'r':
                                ResultESC = '\r';
                                break;

                            default:
                                Console.WriteLine($"Escape '{((char)92)}{ESC}' does not exist");
                                break;
                        }

                        outOperand = (ushort)ResultESC;
                    }

                    else 
                    {
                        try
                        {
                            outOperand = ushort.Parse(operand);
                        }
                        catch
                        {
                            Console.WriteLine($"{operand} - This is not а number");
                        }
                    }
                    
                    break;
            }
            return outOperand;
        }

        byte DefineTypeOperand(string operand)
        {
            byte type = 255;

            Match match = Regex.Match(operand, "0a0d(.*?)\\Z");

            Match match1 = Regex.Match(operand, "0a0c(.*?)\\Z");

            if (match.Success)
            {
                type = DataAddress;
            }
          
            else if (match1.Success)
            {
                type = CodeAddress;
            }

            else if (operand == "dt" || operand == "ir" || operand == "af" || operand == "ct" || operand == "gr" || operand == "cm" || operand == "jn" || operand == "mt" || operand == "bx" || operand == "by") 
            { 
                type = Register;
            }

            else
            {
                type = Number;
            }

            return type;
        }
    }
}
