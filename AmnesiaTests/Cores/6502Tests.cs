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
        public void InitialStateTest()
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
        public void AdcTest()
        {
            _6502 cpu = new _6502();

            //@ 0xC91C  69 69     ADC #$69                        A:00 X:00 Y:00 P:6E SP:FB
            cpu.Regs.Set(0, 0, 0, 0x6E, 0xFB);
            cpu.Adc(_6502.Decode(0x69), 0x69);
            //expected A: 69 X: 00 Y: 00 P: 2C SP:FB
            Assert.AreEqual(0x69, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0x2C, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xC936  69 69     ADC #$69                        A:01 X:00 Y:00 P:6D SP:FB
            cpu.Regs.Set(0x01, 0, 0, 0x6D, 0xFB);
            cpu.Adc(_6502.Decode(0x69), 0x69);
            // expected result A:6B X:00 Y:00 P:2C SP:FB
            Assert.AreEqual(0x6B, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0x2C, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xC94F  69 7F     ADC #$7F                        A:7F X:00 Y:00 P:25 SP:FB
            cpu.Regs.Set(0x7F, 0, 0, 0x25, 0xFB);
            cpu.Adc(_6502.Decode(0x69), 0x7F);
            //expected result A:FF X:00 Y:00 P:E4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0xE4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xC968  69 80     ADC #$80                        A:7F X:00 Y:00 P:64 SP:FB
            cpu.Regs.Set(0x7F, 0, 0, 0x64, 0xFB);
            cpu.Adc(_6502.Decode(0x69), 0x80);
            //expected result A:FF X:00 Y:00 P:A4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0xA4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xC980  69 80     ADC #$80                        A:7F X:00 Y:00 P:25 SP:FB
            cpu.Regs.Set(0x7F, 0, 0, 0x25, 0xFB);
            cpu.Adc(_6502.Decode(0x69), 0x80);
            // expected result A:00 X: 00 Y: 00 P: 27 SP: FB
            Assert.AreEqual(0, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0x27, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xD098  61 80     ADC($80, X) @ 80 = 0200 = 69    A: 00 X: 00 Y: 60 P: 66 SP: FB
            cpu.Regs.Set(0, 0, 0x60, 0x66, 0xFB);
            cpu.Mem.Write(0x80, 0x69);
            cpu.Adc(_6502.Decode(0x61), 0x80);
            // expected result A:69 X:00 Y:60 P:24 SP:FB
            Assert.AreEqual(0x69, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0x60, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xD0A1  61 80     ADC ($80,X) @ 80 = 0200 = 69    A:00 X:00 Y:61 P:67 SP:FB
            cpu.Regs.Set(0, 0, 0x61, 0x67, 0xFB);
            cpu.Mem.Write(0x80, 0x69);
            cpu.Adc(_6502.Decode(0x61), 0x80);
            // expected result A:6A X:00 Y:61 P:24 SP:FB
            Assert.AreEqual(0x6A, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0x61, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xD0AF  61 80     ADC($80, X) @ 80 = 0200 = 7F    A: 7F X: 00 Y: 62 P: 25 SP: FB
            cpu.Regs.Set(0x7F, 0, 0x62, 0x25, 0xFB);
            cpu.Mem.Write(0x80, 0x7F);
            cpu.Adc(_6502.Decode(0x61), 0x80);
            //expected result A:FF X:00 Y:62 P:E4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0x62, cpu.Regs.Y);
            Assert.AreEqual(0xE4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xD0BD  61 80     ADC ($80,X) @ 80 = 0200 = 80    A:7F X:00 Y:63 P:64 SP:FB
            cpu.Regs.Set(0x7F, 0, 0x63, 0x64, 0xFB);
            cpu.Mem.Write(0x80, 0x80);
            cpu.Adc(_6502.Decode(0x61), 0x80);
            //expected result A:FF X:00 Y:63 P:A4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0x63, cpu.Regs.Y);
            Assert.AreEqual(0xA4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xD0C6  61 80     ADC($80, X) @ 80 = 0200 = 80    A: 7F X: 00 Y: 64 P: 25 SP: FB
            cpu.Regs.Set(0x7F, 0, 0x64, 0x25, 0xFB);
            cpu.Mem.Write(0x80, 0x80);
            cpu.Adc(_6502.Decode(0x61), 0x80);
            //expected result A:00 X:00 Y:64 P:27 SP:FB
            Assert.AreEqual(0, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0x64, cpu.Regs.Y);
            Assert.AreEqual(0x27, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

        }

        [TestMethod()]
        public void JmpTest()
        {
            _6502 cpu = new _6502();
            //@ 0xC000 4C F5 C5  JMP $C5F5                       A:00 X:00 Y:00 P:24 SP:FD
            cpu.Regs.PC = 0xC000;
            cpu.Jmp(_6502.Decode(0x4C), 0xF5, 0xC5);
            Assert.AreEqual(0xC5F5, cpu.Regs.PC);
            //@ 0xDB7B 6C 00 02  JMP ($0200) = DB7E              A:DB X:07 Y:00 P:E5 SP:FB
            cpu.Regs.Set(0xDB, 0x07, 0, 0xE5, 0xFB);
            cpu.Regs.PC = 0xDB7B;
            cpu.Mem.Write(0x0200, 0x7E);
            cpu.Mem.Write(0x0201, 0xDB);
            cpu.Jmp(_6502.Decode(0x6C), 0x00, 0x02);
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
            _6502 cpu = new _6502();
            //@ 0xC5F5 A2 00     LDX #$00                        A:00 X:00 Y:00 P:24 SP:FD
            cpu.Regs.Set(0, 0, 0, 0x24, 0xFD);
            cpu.Ldx(_6502.Decode(0xA2), 0);
            // expected result A:00 X:00 Y:00 P:26 SP:FD
            Assert.AreEqual(0, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0x26, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFD, cpu.Regs.SP);
        }
    }
}