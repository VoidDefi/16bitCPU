using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace _8bitsCPU
{
    public class CPU
    {
        public class Register
        {
            public ushort data;

            public byte instruction;
            public ushort[] operands = new ushort[0];

            //public byte address;

            public Register(ushort data) 
            { 
                this.data = data;
            }

            public Register(byte instruction, ushort[] operands)
            {
                this.instruction = instruction;
                this.operands = operands;
            }

            public ushort read()
            {
                return data;
            }
            public void write(ushort data)
            {
                this.data = data;
            }

            //public byte[] readWord()
            //{
            //    return ;
            //}

            //public void writeWord(byte[] word)
            //{
            //    this.word = word;
            //}
        }

        private static ushort[] w = new ushort[2];

        private static Register DT = new(0);    //DT Регистр данных (стондартный, 16бит), (Data) 

        private static Register IR = new(0);    //IR Регистр инструкций (стондартный, 16бит), (Instructions)

        private static Register AF = new(0);    //AF Регистр арифметический (стондартный, 16бит), (Arithmetic)

        private static Register CT = new(0);    //CT Регистр счётчик (стондартный, 16бит), (Counter)

        private static Register GR = new(0);    //GR Регистр общий (стондартный, 16бит), (Generic)

        private static Register CM = new(0);    //CM Регистр общий (стондартный, 16бит), (Common)

        private static Register JN = new(0);    //JN Регистр общий (стондартный, 16бит), (Joint)

        private static Register MT = new(0);    //MT Регистр общий (стондартный, 16бит), (Mutual)

        public static Register lIR = new(0, w); //Регистр инструкций (разширенный, 40бит), (Long Instructions)

        public static ushort carriagePos = 0;

        public static ushort carriageReservPos = 0;

        public static ushort RegisterbytePos = 0;

        public static Register[] registers = new Register[10] {DT, IR, AF, CT, GR, CM, JN, MT, Bios.BX, Bios.BY};

        public static bool FlagQuit;

        public static byte CodeQuit;

        public static long Counter;

        public static bool FalseFlag;

        public static bool TrueFlag;

        public static bool MoreFlag;

        public static bool LessFlag;

        public static byte InstrLanght = 5;

        public void Update()
        {
            carriagePos++;
            RegisterbytePos++;

            if(RegisterbytePos >= 5) 
            {
                int r = Computer.RAM.readByte(carriagePos - 5, Memory.Code);
                lIR.instruction = Computer.RAM.readByte(carriagePos - 5, Memory.Code);
                lIR.operands[0] = Computer.RAM.read(carriagePos - 4, Memory.Code);
                lIR.operands[1] = Computer.RAM.read(carriagePos - 2, Memory.Code);
                RegisterbytePos = 0; 
                ExecutionInstruction();
            }
            if(carriagePos >= 65535) {FlagQuit = true;}

            Counter++;
        }

        private void ExecutionInstruction()
        {
            byte instruction = lIR.instruction;
            ushort operand1 = lIR.operands[0];
            ushort operand2 = lIR.operands[1];

            ushort buffer = 0;
            ushort buffer1 = 0;

            switch (instruction)
            {
                case 1: //mov
                    registers[operand1].write(Computer.RAM.read(operand2, Memory.Data));                           
                    break;

                case 2: //mov
                    Computer.RAM.write(operand1, Memory.Data, registers[operand2].read()); 
                    break;

                case 3: //add
                    buffer = registers[operand2].read();
                    buffer1 = registers[operand1].read();

                    try 
                    {
                        registers[operand1].write((ushort)(buffer1 + buffer));  
                    }  
                    catch (Exception e)
                    {
                        Console.WriteLine("Error : The amount is greater than 65535. The number is out of byte.");
                        CodeQuit = 1;
                        FlagQuit = true;
                    }
                    break;

                case 4: //add
                    buffer = registers[operand1].read();
                    buffer1 = operand2;

                    try
                    {
                        registers[operand1].write((ushort)(buffer1 + buffer));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error : The amount is greater than 65535. The number is out of byte.");
                        CodeQuit = 1;
                        FlagQuit = true;
                    }
                    break;

                case 5: //sub
                    buffer = registers[operand2].read();
                    buffer1 = registers[operand1].read();

                    try
                    {
                        registers[operand1].write((ushort)(buffer1 - buffer));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error : The difference is less than 0. The number is out of byte.");
                        CodeQuit = 1;
                        FlagQuit = true;
                    }
                    break;

                case 6: //sub
                    buffer = registers[operand1].read();
                    buffer1 = operand2;

                    try
                    {
                        registers[operand1].write((ushort)(buffer1 - buffer));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error : The difference is less than 0. The number is out of byte.");
                        CodeQuit = 1;
                        FlagQuit = true;
                    }
                    break;

                case 8: //nln
                    buffer = registers[operand1].read();

                    if(operand2 == 0)
                    {
                        Console.Write(buffer); 
                    }
                    if (operand2 == 1)
                    {
                        Encoding encoding = Encoding.ASCII;
                        Console.Write(encoding.GetString(new byte[2] {BitConverter.GetBytes(buffer)[0], BitConverter.GetBytes(buffer)[1]}));
                    }

                    break;

                case 9: //nln
                    buffer = operand1;

                    if (operand2 == 0)
                    {
                        
                        Console.Write(buffer);
                    }
                    if (operand2 == 1)
                    {
                        Encoding encoding = Encoding.ASCII;
                        Console.Write(encoding.GetString(new byte[2] { BitConverter.GetBytes(buffer)[0], BitConverter.GetBytes(buffer)[1] }));
                    }

                    break;

                case 10: //nln
                    buffer = Computer.RAM.read(operand1, Memory.Data);

                    if (operand2 == 0)
                    {
                        Console.Write(buffer);
                    }
                    if (operand2 == 1)
                    {
                        Encoding encoding = Encoding.ASCII;
                        Console.Write(encoding.GetString(new byte[2] { BitConverter.GetBytes(buffer)[0], BitConverter.GetBytes(buffer)[1] }));
                    }

                    break;

                case 11: //mvc
                    carriageReservPos = carriagePos;
                    carriagePos = operand1;                 
                    break;

                case 12: //mbc
                    carriagePos = carriageReservPos;
                    break;

                case 13: //end
                    FlagQuit = true;
                    break;

                case 14: //mov
                    registers[operand1].write(operand2);
                    break;

                case 15: //mov
                    Computer.RAM.write(operand1, Memory.Data, operand2);
                    break;

                case 16: //and
                    buffer = (ushort)(registers[operand1].read() & registers[operand2].read());
                    registers[operand1].write(buffer);
                    break;

                case 17: //orl
                    buffer = (ushort)(registers[operand1].read() | registers[operand2].read());
                    registers[operand1].write(buffer);
                    break;

                case 18: //xor
                    buffer = (ushort)(registers[operand1].read() ^ registers[operand2].read());
                    registers[operand1].write(buffer);
                    break;

                case 19: //not
                    buffer = (ushort)(~registers[operand1].read());
                    registers[operand1].write(buffer);
                    break;

                case 20: //shr
                    buffer = (ushort)(registers[operand1].read() >> registers[operand2].read());
                    registers[operand1].write(buffer);
                    break;

                case 21: //shr
                    buffer = (ushort)(registers[operand1].read() >> operand2);
                    registers[operand1].write(buffer);
                    break;

                case 22: //shl
                    buffer = (ushort)(registers[operand1].read() << registers[operand2].read());
                    registers[operand1].write(buffer);
                    break;

                case 23: //shl
                    buffer = (ushort)(registers[operand1].read() << operand2);
                    registers[operand1].write(buffer);
                    break;

                case 24: //ifl
                    FalseFlag = false;
                    TrueFlag = false;
                    MoreFlag = false;
                    LessFlag = false;

                    buffer = registers[operand1].read();
                    buffer1 = registers[operand2].read();

                    if (buffer != buffer1) {FalseFlag = true;}
                    if (buffer == buffer1) {TrueFlag = true;}
                    if (buffer > buffer1) {MoreFlag = true;}
                    if (buffer < buffer1) {LessFlag = true;}

                    break;

                case 25: //ifl
                    FalseFlag = false;
                    TrueFlag = false;
                    MoreFlag = false;
                    LessFlag = false;

                    buffer = registers[operand1].read();
                    buffer1 = Computer.RAM.read(operand2, Memory.Data);

                    if (buffer != buffer1) {FalseFlag = true;}
                    if (buffer == buffer1) {TrueFlag = true;}
                    if (buffer > buffer1) {MoreFlag = true;}
                    if (buffer < buffer1) {LessFlag = true;}

                    break;

                case 26: //ifl
                    FalseFlag = false;
                    TrueFlag = false;
                    MoreFlag = false;
                    LessFlag = false;

                    buffer = Computer.RAM.read(operand1, Memory.Data);
                    buffer1 = Computer.RAM.read(operand2, Memory.Data);

                    if (buffer != buffer1) {FalseFlag = true;}
                    if (buffer == buffer1) {TrueFlag = true;}
                    if (buffer > buffer1) {MoreFlag = true;}
                    if (buffer < buffer1) {LessFlag = true;}

                    break;

                case 27: //fls
                    if (!FalseFlag)
                    {
                        carriagePos += InstrLanght;
                    }
                    break;

                case 28: //tre
                    if (!TrueFlag)
                    {
                        carriagePos += InstrLanght;
                    }
                    break;

                case 29: //mor
                    if (!MoreFlag)
                    {
                        carriagePos += InstrLanght;
                    }
                    break;

                case 30: //les
                    if (!LessFlag)
                    {
                        carriagePos += InstrLanght;
                    }
                    break;

                case 31: //bir
                    Computer.bios.Update(operand1, new object[] {operand2});
                    break;

                case 32: //dmd
                    registers[operand1].write(Computer.RAM.read(registers[operand2].read(), Memory.Data));
                    break;

                case 33: //dmd
                    Computer.RAM.write(operand1, Memory.Data, Computer.RAM.read(registers[operand2].read(), Memory.Data));
                    break;

                case 34: //mov
                    registers[operand1].write(registers[operand2].read());
                    break;

                case 35: //ifl
                    FalseFlag = false;
                    TrueFlag = false;
                    MoreFlag = false;
                    LessFlag = false;

                    buffer = registers[operand1].read();
                    buffer1 = operand2;

                    if (buffer != buffer1) { FalseFlag = true; }
                    if (buffer == buffer1) { TrueFlag = true; }
                    if (buffer > buffer1) { MoreFlag = true; }
                    if (buffer < buffer1) { LessFlag = true; }

                    break;

                default:
                    Console.WriteLine($"This instruction {instruction} is not included in the set of this processor.");
                    Console.WriteLine($"Check the compiler logs, or CPU documentation. The instruction is located in {carriagePos - 5}-{carriagePos} memory cells.");
                    CodeQuit = 1;
                    break;
            }

            buffer = 0;
            buffer1 = 0;
        }
    }
}    
