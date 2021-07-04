using Microsoft.VisualStudio.TestTools.UnitTesting;
using Amnesia.Cores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amnesia.Cores.Tests
{
    [TestClass()]
    public class _6502Tests
    {
        [TestMethod()]
        public void InitialState()
        {
            _6502 cpu = new _6502();
            Assert.AreEqual(0, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFD, cpu.Regs.SP);
            Assert.AreEqual(0, cpu.Regs.PC);
        }

        [TestMethod()]
        public void JmpTest()
        {
            //@ 0xC000
            _6502 cpu = new _6502();
            cpu.Regs.PC = 0xC000;
            cpu.Jmp(_6502.AddressingMode.Absolute, 0xF5, 0xC5);
            Assert.AreEqual(0xC5F5, cpu.Regs.PC);
            //@ 0xDB7B
            cpu.Regs.A = 0xDB;
            cpu.Regs.X = 0x07;
            cpu.Regs.Y = 0;
            cpu.Regs.P.FromByte(0xE5);
            cpu.Regs.SP = 0xFB;
            cpu.Regs.PC = 0xDB7B;
            cpu.Mem.Write(0x0200, 0x7E);
            cpu.Mem.Write(0x0201, 0xDB);
            cpu.Jmp(_6502.AddressingMode.Indirect, 0x00, 0x02);
            Assert.AreEqual(0xDB, cpu.Regs.A);
            Assert.AreEqual(0x07, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0xE5, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);
            Assert.AreEqual(0xDB7E, cpu.Regs.PC);
        }

        [TestMethod()]
        public void LdxTest()
        {
            //@ 0xC5F5
            _6502 cpu = new _6502();
            cpu.Ldx(_6502.AddressingMode.Immediate, 0);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0x26, cpu.Regs.P.AsByte());
        }
    }
}