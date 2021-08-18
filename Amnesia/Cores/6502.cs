using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amnesia.Cores
{
    public class _6502
    {
        public enum AddressingModes
        {
            Implied,
            Accumulator,
            Immediate,
            ZeroPage,
            ZeroPageIndexed,
            Absolute,
            AbsoluteIndexedX,
            AbsoluteIndexedY,
            Relative,
            Indirect,
            IndirectPreIndexed,
            IndirectPostIndexed,
        }

        public enum Instructions
        {
            Unknown,
            Adc,
            And,
            Asl,
            Bcc,
            Bcs,
            Beq,
            Bit,
            Bmi,
            Bne,
            Bpl,
            Brk,
            Bvc,
            Bvs,
            Clc,
            Cld,
            Cli,
            Clv,
            Cmp,
            Cpx,
            Cpy,
            Dec,
            Dex,
            Dey,
            Eor,
            Inc,
            Inx,
            Iny,
            Jmp,
            Jsr,
            Lda,
            Ldx,
            Ldy,
            Lsr,
            Nop,
            Ora,
            Pha,
            Php,
            Pla,
            Plp,
            Rol,
            Ror,
            Rti,
            Rts,
            Sbc,
            Sec,
            Sed,
            Sei,
            Sta,
            Stx,
            Sty,
            Tax,
            Tay,
            Tsx,
            Txa,
            Txs,
            Tya,
        }

        public class OpcodeInfo
        {
            public Instructions Instruction { get; set; } = Instructions.Unknown;
            public AddressingModes AddressingMode { get; set; } = AddressingModes.Accumulator;
            public int NumBytes { get; set; } = 0;      //includes opcode
            public int NumCycles { get; set; } = 0;
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

            public void Set(byte a, byte x, byte y, byte p, byte sp)
            {
                A = a;
                X = x;
                Y = y;
                SP = sp;
                P.FromByte(p);
            }
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
                    throw new NotImplementedException("PPU registers with mirroring (address: " + address.ToString("X4") + ")");
                }
                else if (address < 0x4018)
                {
                    //APU and IO registers
                    throw new NotImplementedException("APU and IO registers (address: " + address.ToString("X4") + ")");
                }
                else if (address < 0x4020)
                {
                    //APU and I/O functionality that is normally disabled
                    throw new NotImplementedException("APU and I/O functionality that is normally disabled (address: " + address.ToString("X4") + ")");
                }
                else //0x4020 - 0xFFFF
                {
                    //reserved for cartridge
                    throw new NotImplementedException("ROM read not implemented (address: " + address.ToString("X4") + ")");
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

        private static readonly OpcodeInfo[] opcodeInfos = new OpcodeInfo[byte.MaxValue];

        public _6502()
        {
            //Adc
            opcodeInfos[0x69] = new OpcodeInfo() { Instruction = Instructions.Adc, AddressingMode = AddressingModes.Immediate, NumBytes = 2, NumCycles = 2 };
            opcodeInfos[0x65] = new OpcodeInfo() { Instruction = Instructions.Adc, AddressingMode = AddressingModes.ZeroPage, NumBytes = 2, NumCycles = 3 };
            opcodeInfos[0x75] = new OpcodeInfo() { Instruction = Instructions.Adc, AddressingMode = AddressingModes.ZeroPageIndexed, NumBytes = 2, NumCycles = 4 };
            opcodeInfos[0x6D] = new OpcodeInfo() { Instruction = Instructions.Adc, AddressingMode = AddressingModes.Absolute, NumBytes = 3, NumCycles = 4 };
            opcodeInfos[0x7D] = new OpcodeInfo() { Instruction = Instructions.Adc, AddressingMode = AddressingModes.AbsoluteIndexedX, NumBytes = 3, NumCycles = 4 };
            opcodeInfos[0x79] = new OpcodeInfo() { Instruction = Instructions.Adc, AddressingMode = AddressingModes.AbsoluteIndexedY, NumBytes = 3, NumCycles = 4 };
            opcodeInfos[0x61] = new OpcodeInfo() { Instruction = Instructions.Adc, AddressingMode = AddressingModes.IndirectPreIndexed, NumBytes = 2, NumCycles = 6 };
            opcodeInfos[0x71] = new OpcodeInfo() { Instruction = Instructions.Adc, AddressingMode = AddressingModes.IndirectPostIndexed, NumBytes = 2, NumCycles = 5 };
            //And
            opcodeInfos[0x29] = new OpcodeInfo() { Instruction = Instructions.And, AddressingMode = AddressingModes.Immediate, NumBytes = 2, NumCycles = 2 };
            opcodeInfos[0x25] = new OpcodeInfo() { Instruction = Instructions.And, AddressingMode = AddressingModes.ZeroPage, NumBytes = 2, NumCycles = 3 };
            opcodeInfos[0x35] = new OpcodeInfo() { Instruction = Instructions.And, AddressingMode = AddressingModes.ZeroPageIndexed, NumBytes = 2, NumCycles = 4 };
            opcodeInfos[0x2D] = new OpcodeInfo() { Instruction = Instructions.And, AddressingMode = AddressingModes.Absolute, NumBytes = 3, NumCycles = 4 };
            opcodeInfos[0x3D] = new OpcodeInfo() { Instruction = Instructions.And, AddressingMode = AddressingModes.AbsoluteIndexedX, NumBytes = 3, NumCycles = 4 };
            opcodeInfos[0x39] = new OpcodeInfo() { Instruction = Instructions.And, AddressingMode = AddressingModes.AbsoluteIndexedY, NumBytes = 3, NumCycles = 4 };
            opcodeInfos[0x21] = new OpcodeInfo() { Instruction = Instructions.And, AddressingMode = AddressingModes.IndirectPreIndexed, NumBytes = 2, NumCycles = 6 };
            opcodeInfos[0x31] = new OpcodeInfo() { Instruction = Instructions.And, AddressingMode = AddressingModes.IndirectPostIndexed, NumBytes = 2, NumCycles = 5 };
            //Asl
            opcodeInfos[0x0A] = new OpcodeInfo() { Instruction = Instructions.Asl, AddressingMode = AddressingModes.Accumulator, NumBytes = 1, NumCycles = 2 };
            opcodeInfos[0x06] = new OpcodeInfo() { Instruction = Instructions.Asl, AddressingMode = AddressingModes.ZeroPage, NumBytes = 2, NumCycles = 5 };
            opcodeInfos[0x16] = new OpcodeInfo() { Instruction = Instructions.Asl, AddressingMode = AddressingModes.ZeroPageIndexed, NumBytes = 2, NumCycles = 6 };
            opcodeInfos[0x0E] = new OpcodeInfo() { Instruction = Instructions.Asl, AddressingMode = AddressingModes.Absolute, NumBytes = 3, NumCycles = 6 };
            opcodeInfos[0x1E] = new OpcodeInfo() { Instruction = Instructions.Asl, AddressingMode = AddressingModes.AbsoluteIndexedX, NumBytes = 3, NumCycles = 7 };
            //Bcc
            opcodeInfos[0x90] = new OpcodeInfo() { Instruction = Instructions.Bcc, AddressingMode = AddressingModes.Relative, NumBytes = 2, NumCycles = 2 };
            //Bcs
            opcodeInfos[0xB0] = new OpcodeInfo() { Instruction = Instructions.Bcs, AddressingMode = AddressingModes.Relative, NumBytes = 2, NumCycles = 2 };
            //Beq
            opcodeInfos[0xF0] = new OpcodeInfo() { Instruction = Instructions.Beq, AddressingMode = AddressingModes.Relative, NumBytes = 2, NumCycles = 2 };
            //Bit
            opcodeInfos[0x24] = new OpcodeInfo() { Instruction = Instructions.Bit, AddressingMode = AddressingModes.ZeroPage, NumBytes = 2, NumCycles = 3 };
            opcodeInfos[0x2C] = new OpcodeInfo() { Instruction = Instructions.Bit, AddressingMode = AddressingModes.Absolute, NumBytes = 3, NumCycles = 4 };
            //Bmi
            opcodeInfos[0x30] = new OpcodeInfo() { Instruction = Instructions.Bmi, AddressingMode = AddressingModes.Relative, NumBytes = 2, NumCycles = 2 };
            //Bne
            opcodeInfos[0xD0] = new OpcodeInfo() { Instruction = Instructions.Bne, AddressingMode = AddressingModes.Relative, NumBytes = 2, NumCycles = 2 };
            //Bpl
            opcodeInfos[0x10] = new OpcodeInfo() { Instruction = Instructions.Bpl, AddressingMode = AddressingModes.Relative, NumBytes = 2, NumCycles = 2 };
            //Brk
            opcodeInfos[0x00] = new OpcodeInfo() { Instruction = Instructions.Brk, AddressingMode = AddressingModes.Implied, NumBytes = 1, NumCycles = 7 };
            //Bvc
            opcodeInfos[0x50] = new OpcodeInfo() { Instruction = Instructions.Bvc, AddressingMode = AddressingModes.Relative, NumBytes = 2, NumCycles = 2 };
            //Bvs
            opcodeInfos[0x70] = new OpcodeInfo() { Instruction = Instructions.Bvs, AddressingMode = AddressingModes.Relative, NumBytes = 2, NumCycles = 2 };
            //Clc
            opcodeInfos[0x18] = new OpcodeInfo() { Instruction = Instructions.Clc, AddressingMode = AddressingModes.Implied, NumBytes = 1, NumCycles = 2 };
            //...
            //Jmp
            opcodeInfos[0x4C] = new OpcodeInfo() { Instruction = Instructions.Jmp, AddressingMode = AddressingModes.Absolute, NumBytes = 3, NumCycles = 3 };
            opcodeInfos[0x6C] = new OpcodeInfo() { Instruction = Instructions.Jmp, AddressingMode = AddressingModes.Indirect, NumBytes = 3, NumCycles = 5 };
            //...
            //Ldx
            opcodeInfos[0xA2] = new OpcodeInfo() { Instruction = Instructions.Ldx, AddressingMode = AddressingModes.Immediate, NumBytes = 2, NumCycles = 2 };
            opcodeInfos[0xA6] = new OpcodeInfo() { Instruction = Instructions.Ldx, AddressingMode = AddressingModes.ZeroPage, NumBytes = 2, NumCycles = 3 };
            opcodeInfos[0xB6] = new OpcodeInfo() { Instruction = Instructions.Ldx, AddressingMode = AddressingModes.ZeroPageIndexed, NumBytes = 2, NumCycles = 4 };
            opcodeInfos[0xAE] = new OpcodeInfo() { Instruction = Instructions.Ldx, AddressingMode = AddressingModes.Absolute, NumBytes = 3, NumCycles = 4 };
            opcodeInfos[0xBE] = new OpcodeInfo() { Instruction = Instructions.Ldx, AddressingMode = AddressingModes.AbsoluteIndexedY, NumBytes = 3, NumCycles = 4 };
        }

        private static bool IsNegative(byte arg)
        {
            return arg >= 0x80;
        }

        /// <summary>
        /// Check if result of operation gave a wrong result.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="operand"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool IsOverflow(byte a, byte operand, byte result)
        {
            //6502 turns on the overflow flag when an addition goes past the maximum or minimum
            //signed values of an 8 bit register, which causes the result to have the wrong sign bit.
            //This situation can be detected by comparing the sign of the operands with the sign
            //of the result.
            return IsNegative(a) == IsNegative(operand) && IsNegative(a) != IsNegative(result);
        }

        private static ushort ToWord(byte bl, byte bh)
        {
            return (ushort)((bh << 8) | bl);
        }

        /// <summary>
        /// Return opcode information corresponding to the provided machine code
        /// </summary>
        /// <param name="b">Machine code</param>
        /// <returns>Opcode information</returns>
        public static OpcodeInfo Decode(byte b)
        {
            return opcodeInfos[b];
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
        /// Add memory to accumulator with carry
        /// </summary>
        /// <param name="mode">Addressing Mode</param>
        /// <param name="arg1">First byte after opcode</param>
        /// <param name="arg2">Second byte after opcode (optional)</param>
        /// <returns></returns>
        public int Adc(OpcodeInfo info, byte arg1, byte arg2 = 0)
        {
            byte operand = 0;
            switch (info.AddressingMode)
            {
                case AddressingModes.Immediate:
                    operand = arg1;
                    break;
                case AddressingModes.ZeroPage:
                    operand = Mem.Read(arg1);
                    break;
                case AddressingModes.ZeroPageIndexed:
                    operand = Mem.Read((byte)((arg1 + Regs.X) & 0xFF));
                    break;
                case AddressingModes.Absolute:
                    operand = Mem.Read(ToWord(arg1, arg2));
                    break;
                case AddressingModes.AbsoluteIndexedX:
                    {
                        ushort address = ToWord(arg1, arg2);
                        address += Regs.X;
                        operand = Mem.Read(address);
                    }
                    break;
                case AddressingModes.AbsoluteIndexedY:
                    {
                        ushort address = ToWord(arg1, arg2);
                        address += Regs.Y;
                        operand = Mem.Read(address);
                    }
                    break;
                case AddressingModes.IndirectPreIndexed:
                    {
                        arg1 += Regs.X;
                        ushort address = Mem.ReadWord(arg1);
                        operand = Mem.Read(address);
                    }
                    break;
                case AddressingModes.IndirectPostIndexed:
                    {
                        ushort address = Mem.ReadWord(arg1);
                        address += Regs.Y;
                        operand = Mem.Read(address);
                    }
                    break;
                default:
                    throw new NotImplementedException("ADC " + info.AddressingMode);
            }
            int result = Regs.A + operand + (Regs.P.Carry ? 1 : 0);
            Regs.P.Carry = result > 0xFF;
            result &= 0xFF;
            Regs.P.Overflow = IsOverflow(Regs.A, operand, (byte)result);
            Regs.A = (byte)result;
            Regs.P.Zero = Regs.A == 0;
            Regs.P.Negative = IsNegative(Regs.A);
            return info.NumCycles;
        }

        /// <summary>
        /// LDX Load index X with memory
        /// </summary>
        /// <param name="mode">Addressing Mode</param>
        /// <returns>Number of cycles executed</returns>
        public int Ldx(OpcodeInfo info, byte arg1, byte arg2 = 0)
        {
            switch (info.AddressingMode)
            {
                case AddressingModes.Immediate:
                    Regs.X = Mem.Read(arg1);
                    Regs.P.Zero = Regs.X == 0;
                    Regs.P.Negative = IsNegative(Regs.X);
                    return 2;
                default:
                    throw new NotImplementedException("LDX " + info.AddressingMode);
            }
        }

        /// <summary>
        /// JMP Jump to new location
        /// </summary>
        /// <param name="mode">Addressing Mode</param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public int Jmp(OpcodeInfo info, byte arg1, byte arg2)
        {
            switch (info.AddressingMode)
            {
                case AddressingModes.Absolute:
                    Regs.PC = ToWord(arg1, arg2);
                    return 3;
                case AddressingModes.Indirect:
                    Regs.PC = Mem.ReadWord(ToWord(arg1, arg2));
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
        public int Stx(OpcodeInfo info)
        {
            return 0;
        }
    }
}
