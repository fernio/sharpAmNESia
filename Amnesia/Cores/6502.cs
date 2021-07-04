using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amnesia.Cores
{
    public class _6502
    {
        public enum AddressingMode
        {
            Accumulator,
            Immediate,
            ZeroPage,
            Absolute,
            Relative,
            Indirect,
        }

        public class StatusRegister
        {
            public bool Negative { get; set; } = false;
            public bool Overflow { get; set; } = false;
            public bool Decimal { get; set; } = false;
            public bool InterruptDisable { get; set; } = true;
            public bool Zero { get; set; } = false;
            public bool Carry { get; set; } = false;

            public byte AsByte()
            {
                int result = ((Negative ? 1 : 0) << 7)
                    | ((Overflow ? 1 : 0) << 6)
                    | (1 << 5)
                    | (0 << 4)
                    | ((Decimal ? 1 : 0) << 3)
                    | ((InterruptDisable ? 1 : 0) << 2)
                    | ((Zero ? 1 : 0) << 1)
                    | ((Carry ? 1 : 0) << 0);
                return (byte)result;
            }

            public void FromByte(byte b)
            {
                Negative = (b & (1 << 7)) != 0;
                Overflow = (b & (1 << 6)) != 0;
                //1 << 5
                //1 << 4;
                Decimal = (b & (1 << 3)) != 0;
                InterruptDisable = (b & (1 << 2)) != 0;
                Zero = (b & (1 << 1)) != 0;
                Carry = (b & (1 << 0)) != 0;
            }
        }

        public class Registers
        {
            public byte A { get; set; }             //accumulator
            public byte X { get; set; }             //index x
            public byte Y { get; set; }             //index y
            public ushort PC { get; set; }          //program counter
            public byte SP { get; set; } = 0xFD;     //stack pointer
            public StatusRegister P { get; } = new StatusRegister();  //status register
        }

        public class Memory
        {
            private readonly byte[] ram = new byte[2048];

            public byte Read(ushort address)
            {
                if (address < 0x2000)
                {
                    //2KB of RAM with mirroring
                    return ram[address % 2048];
                }
                else if (address < 0x4000)
                {
                    //PPU registers with mirroring
                    throw new NotImplementedException("PPU registers with mirroring");
                }
                else if (address < 0x4018)
                {
                    //APU and IO registers
                    throw new NotImplementedException("APU and IO registers");
                }
                else if (address < 0x4020)
                {
                    //APU and I/O functionality that is normally disabled
                    throw new NotImplementedException("APU and I/O functionality that is normally disabled");
                }
                else //0x4020 - 0xFFFF
                {
                    //reserved for cartridge
                    throw new NotImplementedException("ROM read not implemented");
                }
            }

            public ushort ReadWord(ushort address)
            {
                ushort upperAddress = (ushort)(address + 1);
                return (ushort)((Read(upperAddress) << 8) | Read(address));
            }

            public void Write(ushort address, byte data)
            {
                if (address < 0x2000)
                {
                    //2KB of RAM with mirroring
                    ram[address % 2048] = data;
                }
                else if (address < 0x4000)
                {
                    //PPU registers with mirroring
                    throw new NotImplementedException("PPU registers with mirroring");
                }
                else if (address < 0x4018)
                {
                    //APU and IO registers
                    throw new NotImplementedException("APU and IO registers");
                }
                else if (address < 0x4020)
                {
                    //APU and I/O functionality that is normally disabled
                    throw new NotImplementedException("APU and I/O functionality that is normally disabled");
                }
                else //0x4020 - 0xFFFF
                {
                    //reserved for cartridge
                    throw new NotImplementedException("ROM read not implemented");
                }
            }
        }

        public Memory Mem { get; } = new Memory();

        public Registers Regs { get; } = new Registers();

        public static bool IsNegative(byte arg)
        {
            return arg >= 0x80;
        }

        /// <summary>
        /// Reset CPU state
        /// </summary>
        public void Reset()
        {
            //load reset vector from ROM to PC
            Regs.PC = Mem.ReadWord(0xFFFC);
        }

        /// <summary>
        /// LDX Load index X with memory
        /// </summary>
        /// <param name="mode">Addressing Mode</param>
        /// <returns>Number of cycles executed</returns>
        public int Ldx(AddressingMode mode, byte arg1, byte arg2 = 0)
        {
            switch (mode)
            {
                case AddressingMode.Immediate:
                    Regs.X = Mem.Read(arg1);
                    Regs.P.Zero = Regs.X == 0;
                    Regs.P.Negative = IsNegative(Regs.X);
                    return 2;
                case AddressingMode.ZeroPage:
                    throw new NotImplementedException("LDX ZeroPage");
                case AddressingMode.Absolute:
                    throw new NotImplementedException("LDX Absolute");
                default:
                    throw new ArgumentException("addressing mode not supported by opcode");
            }
        }

        public int Jmp(AddressingMode mode, byte arg1, byte arg2)
        {
            switch (mode)
            {
                case AddressingMode.Absolute:
                    Regs.PC = (ushort)((arg2 << 8) | arg1);
                    return 3;
                case AddressingMode.Indirect:
                    ushort address = (ushort)((arg2 << 8) | arg1);
                    Regs.PC = Mem.ReadWord(address);
                    return 5;
                default:
                    throw new ArgumentException("addressing mode not supported by opcode");
            }
        }

        /// <summary>
        /// STX Store index X in memory
        /// </summary>
        /// <param name="mode">Addressing Mode</param>
        /// <returns>Number of cycles executed</returns>
        public int Stx(AddressingMode mode)
        {
            return 0;
        }
    }
}
