using Amnesia.Cores;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static Amnesia.Cores.Ricoh2A03;

namespace Amnesia.Cores.Tests
{
    [TestClass()]
    public class Ricoh2A03Tests
    {
        public void AssertRegisterValues(Ricoh2A03.Registers expectedValues, Ricoh2A03.Registers actualValues)
        {
            Assert.AreEqual(expectedValues.A, actualValues.A);
            Assert.AreEqual(expectedValues.X, actualValues.X);
            Assert.AreEqual(expectedValues.Y, actualValues.Y);
            Assert.AreEqual(expectedValues.P.AsByte(), actualValues.P.AsByte());
            Assert.AreEqual(expectedValues.SP, actualValues.SP);
        }
        public void AssertMemoryValue(byte expectedValue, Ricoh2A03.Memory mem, ushort address)
        {
            Assert.AreEqual(expectedValue, mem.Read(address));
        }

        [TestMethod()]
        public void InitialStateTest()
        {
            var cpu = new Ricoh2A03();
            AssertRegisterValues(new Ricoh2A03.Registers(0, 0, 0, 0x24, 0xFD), cpu.Regs);
            Assert.AreEqual(0, cpu.Regs.PC);
        }

        [TestMethod()]
        public void AdcTest()
        {
            var cpu = new Ricoh2A03();

            //@ 0xC91C  69 69     ADC #$69                        A:00 X:00 Y:00 P:6E SP:FB
            cpu.Regs.Set(0, 0, 0, 0x6E, 0xFB);
            cpu.Adc(Ricoh2A03.Decode(0x69), 0x69);
            //expected A: 69 X: 00 Y: 00 P: 2C SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x69, 0, 0, 0x2C, 0xFB), cpu.Regs);

            //@ 0xC936  69 69     ADC #$69                        A:01 X:00 Y:00 P:6D SP:FB
            cpu.Regs.Set(0x01, 0, 0, 0x6D, 0xFB);
            cpu.Adc(Ricoh2A03.Decode(0x69), 0x69);
            // expected result A:6B X:00 Y:00 P:2C SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x6B, 0, 0, 0x2C, 0xFB), cpu.Regs);

            //@ 0xC94F  69 7F     ADC #$7F                        A:7F X:00 Y:00 P:25 SP:FB
            cpu.Regs.Set(0x7F, 0, 0, 0x25, 0xFB);
            cpu.Adc(Ricoh2A03.Decode(0x69), 0x7F);
            //expected result A:FF X:00 Y:00 P:E4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0, 0, 0xE4, 0xFB), cpu.Regs);

            //@ 0xC968  69 80     ADC #$80                        A:7F X:00 Y:00 P:64 SP:FB
            cpu.Regs.Set(0x7F, 0, 0, 0x64, 0xFB);
            cpu.Adc(Ricoh2A03.Decode(0x69), 0x80);
            //expected result A:FF X:00 Y:00 P:A4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0, 0, 0xA4, 0xFB), cpu.Regs);

            //@ 0xC980  69 80     ADC #$80                        A:7F X:00 Y:00 P:25 SP:FB
            cpu.Regs.Set(0x7F, 0, 0, 0x25, 0xFB);
            cpu.Adc(Ricoh2A03.Decode(0x69), 0x80);
            // expected result A:00 X: 00 Y: 00 P: 27 SP: FB
            AssertRegisterValues(new Ricoh2A03.Registers(0, 0, 0, 0x27, 0xFB), cpu.Regs);

            //@ 0xD098  61 80     ADC($80, X) @ 80 = 0200 = 69    A: 00 X: 00 Y: 60 P: 66 SP: FB
            cpu.Regs.Set(0, 0, 0x60, 0x66, 0xFB);
            cpu.Mem.Write(0x80, 0x00);
            cpu.Mem.Write(0x81, 0x02);
            cpu.Mem.Write(0x200, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x61), 0x80);
            // expected result A:69 X:00 Y:60 P:24 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x69, 0, 0x60, 0x24, 0xFB), cpu.Regs);

            //@ 0xD0A1  61 80     ADC ($80,X) @ 80 = 0200 = 69    A:00 X:00 Y:61 P:67 SP:FB
            cpu.Regs.Set(0, 0, 0x61, 0x67, 0xFB);
            cpu.Mem.Write(0x80, 0x00);
            cpu.Mem.Write(0x81, 0x02);
            cpu.Mem.Write(0x200, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x61), 0x80);
            // expected result A:6A X:00 Y:61 P:24 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x6A, 0, 0x61, 0x24, 0xFB), cpu.Regs);

            //@ 0xD0AF  61 80     ADC($80, X) @ 80 = 0200 = 7F    A: 7F X: 00 Y: 62 P: 25 SP: FB
            cpu.Regs.Set(0x7F, 0, 0x62, 0x25, 0xFB);
            cpu.Mem.Write(0x80, 0x00);
            cpu.Mem.Write(0x81, 0x02);
            cpu.Mem.Write(0x200, 0x7F);
            cpu.Adc(Ricoh2A03.Decode(0x61), 0x80);
            //expected result A:FF X:00 Y:62 P:E4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0, 0x62, 0xE4, 0xFB), cpu.Regs);

            //@ 0xD0BD  61 80     ADC ($80,X) @ 80 = 0200 = 80    A:7F X:00 Y:63 P:64 SP:FB
            cpu.Regs.Set(0x7F, 0, 0x63, 0x64, 0xFB);
            cpu.Mem.Write(0x80, 0x00);
            cpu.Mem.Write(0x81, 0x02);
            cpu.Mem.Write(0x200, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x61), 0x80);
            //expected result A:FF X:00 Y:63 P:A4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0, 0x63, 0xA4, 0xFB), cpu.Regs);

            //@ 0xD0C6  61 80     ADC($80, X) @ 80 = 0200 = 80    A: 7F X: 00 Y: 64 P: 25 SP: FB
            cpu.Regs.Set(0x7F, 0, 0x64, 0x25, 0xFB);
            cpu.Mem.Write(0x80, 0x00);
            cpu.Mem.Write(0x81, 0x02);
            cpu.Mem.Write(0x200, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x61), 0x80);
            //expected result A:00 X:00 Y:64 P:27 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0, 0, 0x64, 0x27, 0xFB), cpu.Regs);

            //@ 0xD2B5  65 78     ADC $78 = 69                    A: 00 X: 33 Y: 84 P: 66 SP: FB
            cpu.Regs.Set(0, 0x33, 0x84, 0x66, 0xFB);
            cpu.Mem.Write(0x78, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x65), 0x78);
            //expected result A:69 X:33 Y:84 P:24 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x69, 0x33, 0x84, 0x24, 0xFB), cpu.Regs);

            //@ 0xD2BE  65 78     ADC $78 = 69                    A: 00 X: 33 Y: 85 P: 67 SP: FB
            cpu.Regs.Set(0, 0x33, 0x85, 0x67, 0xFB);
            cpu.Mem.Write(0x78, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x65), 0x78);
            //expected result A:6A X:33 Y:85 P:24 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x6A, 0x33, 0x85, 0x24, 0xFB), cpu.Regs);

            //D2CB  65 78     ADC $78 = 7F                    A: 7F X: 33 Y: 86 P: 25 SP: FB CYC:231 SL: 18
            cpu.Regs.Set(0x7F, 0x33, 0x86, 0x25, 0xFB);
            cpu.Mem.Write(0x78, 0x7F);
            cpu.Adc(Ricoh2A03.Decode(0x65), 0x78);
            //expected result A:FF X:33 Y:86 P:E4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0x33, 0x86, 0xE4, 0xFB), cpu.Regs);

            //D2D8  65 78     ADC $78 = 80                    A: 7F X: 33 Y: 87 P: 64 SP: FB CYC: 43 SL: 19
            cpu.Regs.Set(0x7F, 0x33, 0x87, 0x64, 0xFB);
            cpu.Mem.Write(0x78, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x65), 0x78);
            //A:FF X:33 Y:87 P:A4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0x33, 0x87, 0xA4, 0xFB), cpu.Regs);

            //D2E1  65 78     ADC $78 = 80                    A: 7F X: 33 Y: 88 P: 25 SP: FB CYC:178 SL: 19
            cpu.Regs.Set(0x7F, 0x33, 0x88, 0x25, 0xFB);
            cpu.Mem.Write(0x78, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x65), 0x78);
            //A:00 X:33 Y:88 P:27 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0, 0x33, 0x88, 0x27, 0xFB), cpu.Regs);

            //D659  6D 78 06  ADC $0678 = 69                  A: 00 X: 33 Y: BE P:66 SP: FB CYC:308 SL: 38
            cpu.Regs.Set(0, 0x33, 0xBE, 0x66, 0xFB);
            cpu.Mem.Write(0x678, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x6D), 0x78, 0x06);
            //A:69 X:33 Y:BE P:24 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x69, 0x33, 0xBE, 0x24, 0xFB), cpu.Regs);

            //D663  6D 78 06  ADC $0678 = 69                  A: 00 X: 33 Y: BF P:67 SP: FB CYC:108 SL: 39
            cpu.Regs.Set(0, 0x33, 0xBF, 0x67, 0xFB);
            cpu.Mem.Write(0x678, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x6D), 0x78, 0x06);
            //A:6A X:33 Y:BF P:24 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x6A, 0x33, 0xBF, 0x24, 0xFB), cpu.Regs);

            //D672  6D 78 06  ADC $0678 = 7F                  A:7F X:33 Y:C0 P:25 SP:FB CYC:264 SL:39
            cpu.Regs.Set(0x7F, 0x33, 0xC0, 0x25, 0xFB);
            cpu.Mem.Write(0x678, 0x7F);
            cpu.Adc(Ricoh2A03.Decode(0x6D), 0x78, 0x06);
            //A:FF X:33 Y:C0 P:E4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0x33, 0xC0, 0xE4, 0xFB), cpu.Regs);

            //D681  6D 78 06  ADC $0678 = 80                  A:7F X:33 Y:C1 P:64 SP:FB CYC: 82 SL:40
            cpu.Regs.Set(0x7F, 0x33, 0xC1, 0x64, 0xFB);
            cpu.Mem.Write(0x678, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x6D), 0x78, 0x06);
            //A:FF X:33 Y:C1 P:A4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0x33, 0xC1, 0xA4, 0xFB), cpu.Regs);

            //D68B  6D 78 06  ADC $0678 = 80                  A: 7F X: 33 Y: C2 P:25 SP: FB CYC:220 SL: 40
            cpu.Regs.Set(0x7F, 0x33, 0xC2, 0x25, 0xFB);
            cpu.Mem.Write(0x678, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x6D), 0x78, 0x06);
            //A:00 X:33 Y:C2 P:27 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0, 0x33, 0xC2, 0x27, 0xFB), cpu.Regs);

            //DA01  71 33     ADC($33),Y = 0400 @ 0400 = 69  A: 00 X: F3 Y:00 P: 66 SP: FB CYC:151 SL: 58
            cpu.Regs.Set(0, 0xF3, 0, 0x66, 0xFB);
            cpu.Mem.Write(0x33, 0x00);
            cpu.Mem.Write(0x34, 0x04);
            cpu.Mem.Write(0x400, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x71), 0x33);
            //A:69 X:F3 Y:00 P:24 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x69, 0xF3, 0, 0x24, 0xFB), cpu.Regs);

            //DA15  71 33     ADC($33),Y = 0400 @ 0400 = 69  A: 00 X: F4 Y:00 P: 67 SP: FB CYC:226 SL: 58
            cpu.Regs.Set(0, 0xF4, 0, 0x67, 0xFB);
            cpu.Mem.Write(0x33, 0x00);
            cpu.Mem.Write(0x34, 0x04);
            cpu.Mem.Write(0x400, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x71), 0x33);
            //A:6A X:F4 Y:00 P:24 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x6A, 0xF4, 0, 0x24, 0xFB), cpu.Regs);

            //DA2B  71 33     ADC($33),Y = 0400 @ 0400 = 7F  A: 7F X: F5 Y:00 P: 25 SP: FB CYC:310 SL: 58
            cpu.Regs.Set(0x7F, 0xF5, 0, 0x25, 0xFB);
            cpu.Mem.Write(0x33, 0x00);
            cpu.Mem.Write(0x34, 0x04);
            cpu.Mem.Write(0x400, 0x7F);
            cpu.Adc(Ricoh2A03.Decode(0x71), 0x33);
            //A:FF X:F5 Y:00 P:E4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0xF5, 0, 0xE4, 0xFB), cpu.Regs);

            //DA44  71 33     ADC($33),Y = 0400 @ 0400 = 80  A: 7F X: F6 Y:00 P: 64 SP: FB CYC: 62 SL: 59
            cpu.Regs.Set(0x7F, 0xF6, 0, 0x64, 0xFB);
            cpu.Mem.Write(0x33, 0x00);
            cpu.Mem.Write(0x34, 0x04);
            cpu.Mem.Write(0x400, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x71), 0x33);
            //A:FF X:F6 Y:00 P:A4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0xF6, 0, 0xA4, 0xFB), cpu.Regs);

            //DA5C  71 33     ADC($33),Y = 0400 @ 0400 = 80  A: 7F X: F7 Y:00 P: 25 SP: FB CYC:152 SL: 59
            cpu.Regs.Set(0x7F, 0xF7, 0, 0x25, 0xFB);
            cpu.Mem.Write(0x33, 0x00);
            cpu.Mem.Write(0x34, 0x04);
            cpu.Mem.Write(0x400, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x71), 0x33);
            //A:00 X:F7 Y:00 P:27 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0, 0xF7, 0, 0x27, 0xFB), cpu.Regs);

            //E034  79 00 04  ADC $0400,Y @ 0400 = 69         A: 00 X: 3F Y: 00 P: 66 SP: FB CYC: 39 SL: 66
            cpu.Regs.Set(0, 0x3F, 0, 0x66, 0xFB);
            cpu.Mem.Write(0x400, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x79), 0, 0x04);
            //A:69 X:3F Y:00 P:24 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x69, 0x3F, 0, 0x24, 0xFB), cpu.Regs);

            //E049  79 00 04  ADC $0400,Y @ 0400 = 69         A: 00 X: 40 Y: 00 P: 67 SP: FB CYC:111 SL: 66
            cpu.Regs.Set(0, 0x40, 0, 0x67, 0xFB);
            cpu.Mem.Write(0x400, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x79), 0, 0x04);
            //A:6A X:40 Y:00 P:24 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x6A, 0x40, 0, 0x24, 0xFB), cpu.Regs);

            //E060  79 00 04  ADC $0400,Y @ 0400 = 7F         A: 7F X: 41 Y: 00 P: 25 SP: FB CYC:192 SL: 66
            cpu.Regs.Set(0x7F, 0x41, 0, 0x25, 0xFB);
            cpu.Mem.Write(0x400, 0x7F);
            cpu.Adc(Ricoh2A03.Decode(0x79), 0, 0x04);
            //A:FF X:41 Y:00 P:E4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0x41, 0, 0xE4, 0xFB), cpu.Regs);

            //E07A  79 00 04  ADC $0400,Y @ 0400 = 80         A: 7F X: 42 Y: 00 P: 64 SP: FB CYC:282 SL: 66
            cpu.Regs.Set(0x7F, 0x42, 0, 0x64, 0xFB);
            cpu.Mem.Write(0x400, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x79), 0, 0x04);
            //A:FF X:42 Y:00 P:A4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0x42, 0, 0xA4, 0xFB), cpu.Regs);

            //E093  79 00 04  ADC $0400,Y @ 0400 = 80         A: 7F X: 43 Y: 00 P: 25 SP: FB CYC: 28 SL: 67
            cpu.Regs.Set(0x7F, 0x43, 0, 0x25, 0xFB);
            cpu.Mem.Write(0x400, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x79), 0, 0x04);
            //A:00 X:43 Y:00 P:27 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0, 0x43, 0, 0x27, 0xFB), cpu.Regs);

            //DC8B  75 00     ADC $00,X @ 78 = 69             A: 00 X: 78 Y: 11 P: 66 SP: FB CYC: 95 SL: 74
            cpu.Regs.Set(0, 0x78, 0x11, 0x66, 0xFB);
            cpu.Mem.Write(0x78, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x75), 0);
            //A:69 X:78 Y:11 P:24 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x69, 0x78, 0x11, 0x24, 0xFB), cpu.Regs);

            //DC94  75 00     ADC $00,X @ 78 = 69             A: 00 X: 78 Y: 12 P: 67 SP: FB CYC:236 SL: 74
            cpu.Regs.Set(0, 0x78, 0x12, 0x67, 0xFB);
            cpu.Mem.Write(0x78, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x75), 0);
            //A:6A X:78 Y:12 P:24 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x6A, 0x78, 0x12, 0x24, 0xFB), cpu.Regs);

            //DCA1  75 00     ADC $00,X @ 78 = 7F             A: 7F X: 78 Y: 13 P: 25 SP: FB CYC: 48 SL: 75
            cpu.Regs.Set(0x7F, 0x78, 0x13, 0x25, 0xFB);
            cpu.Mem.Write(0x78, 0x7F);
            cpu.Adc(Ricoh2A03.Decode(0x75), 0);
            //A:FF X:78 Y:13 P:E4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0x78, 0x13, 0xE4, 0xFB), cpu.Regs);

            //DCAE  75 00     ADC $00,X @ 78 = 80             A: 7F X: 78 Y: 14 P: 64 SP: FB CYC:204 SL: 75
            cpu.Regs.Set(0x7F, 0x78, 0x14, 0x64, 0xFB);
            cpu.Mem.Write(0x78, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x75), 0);
            //A:FF X:78 Y:14 P:A4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0x78, 0x14, 0xA4, 0xFB), cpu.Regs);

            //DCB7  75 00     ADC $00,X @ 78 = 80             A: 7F X: 78 Y: 15 P: 25 SP: FB CYC:  1 SL: 76
            cpu.Regs.Set(0x7F, 0x78, 0x15, 0x25, 0xFB);
            cpu.Mem.Write(0x78, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x75), 0);
            //A:00 X:78 Y:15 P:27 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0, 0x78, 0x15, 0x27, 0xFB), cpu.Regs);

            //E265  7D 00 06  ADC $0600,X @ 0678 = 69         A: 00 X: 78 Y: 59 P: 66 SP: FB CYC:242 SL: 92
            cpu.Regs.Set(0, 0x78, 0x59, 0x66, 0xFB);
            cpu.Mem.Write(0x678, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x7D), 0, 0x6);
            //A:69 X:78 Y:59 P:24 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x69, 0x78, 0x59, 0x24, 0xFB), cpu.Regs);

            //E26F  7D 00 06  ADC $0600,X @ 0678 = 69         A:00 X:78 Y:5A P:67 SP:FB CYC: 42 SL:93
            cpu.Regs.Set(0, 0x78, 0x5A, 0x67, 0xFB);
            cpu.Mem.Write(0x678, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x7D), 0, 0x6);
            //A:6A X:78 Y:5A P:24 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x6A, 0x78, 0x5A, 0x24, 0xFB), cpu.Regs);

            //E27E  7D 00 06  ADC $0600,X @ 0678 = 7F         A: 7F X: 78 Y: 5B P:25 SP: FB CYC:198 SL: 93
            cpu.Regs.Set(0x7F, 0x78, 0x5B, 0x25, 0xFB);
            cpu.Mem.Write(0x678, 0x7F);
            cpu.Adc(Ricoh2A03.Decode(0x7D), 0, 0x6);
            //A:FF X:78 Y:5B P:E4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0x78, 0x5B, 0xE4, 0xFB), cpu.Regs);

            //E28D  7D 00 06  ADC $0600,X @ 0678 = 80         A: 7F X: 78 Y: 5C P:64 SP: FB CYC: 16 SL: 94
            cpu.Regs.Set(0x7F, 0x78, 0x5C, 0x64, 0xFB);
            cpu.Mem.Write(0x678, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x7D), 0, 0x6);
            //A:FF X:78 Y:5C P:A4 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0xFF, 0x78, 0x5C, 0xA4, 0xFB), cpu.Regs);

            //E297  7D 00 06  ADC $0600,X @ 0678 = 80         A: 7F X: 78 Y: 5D P: 25 SP: FB CYC:154 SL: 94
            cpu.Regs.Set(0x7F, 0x78, 0x5D, 0x25, 0xFB);
            cpu.Mem.Write(0x678, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x7D), 0, 0x6);
            //A:00 X:78 Y:5D P:27 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0, 0x78, 0x5D, 0x27, 0xFB), cpu.Regs);
        }

        [TestMethod()]
        public void AndTest()
        {
            var cpu = new Ricoh2A03();
            //@0xC7E9 29 EF     AND #$EF                        A:7F X:00 Y:00 P:6D SP:FB
            cpu.Regs.Set(0x7F, 0, 0, 0x6D, 0xFB);
            cpu.And(Ricoh2A03.Decode(0x29), 0xEF);
            //expected result A:6F X:00 Y:00 P:6D SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x6F, 0, 0, 0x6D, 0xFB), cpu.Regs);

            //@0xD281  25 78     AND $78 = AA                    A:55 X:33 Y:80 P:64 SP:FB
            cpu.Regs.Set(0x55, 0x33, 0x80, 0x64, 0xFB);
            cpu.And(Ricoh2A03.Decode(0x25), 0x78);
            //expected result A:00 X:33 Y:80 P:66 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0, 0x33, 0x80, 0x66, 0xFB), cpu.Regs);

            //@0xDC64  35 00     AND $00,X @ 78 = EF             A:F8 X:78 Y:0E P:A5 SP:FB

            //@0xD61D  2D 78 06  AND $0678 = AA                  A:55 X:33 Y:BA P:64 SP:FB CYC: 31 SL:37


            //@0xD065 21 80     AND($80, X) @ 80 = 0200 = AA    A:55 X:00 Y:5C P:64 SP:FB
            cpu.Regs.Set(0x55, 0, 0x5C, 0x64, 0xFB);
            cpu.Mem.Write(0x80, 0x00);
            cpu.Mem.Write(0x81, 0x02);
            cpu.Mem.Write(0x200, 0xAA);
            cpu.And(Ricoh2A03.Decode(0x21), 0x80);
            //expected result A:00 X:00 Y:5C P:66 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0, 0, 0x5C, 0x66, 0xFB), cpu.Regs);

        }


        [TestMethod()]
        public void JmpTest()
        {
            var cpu = new Ricoh2A03();
            //@ 0xC000 4C F5 C5  JMP $C5F5                       A:00 X:00 Y:00 P:24 SP:FD
            cpu.Regs.PC = 0xC000;
            cpu.Jmp(Ricoh2A03.Decode(0x4C), 0xF5, 0xC5);
            Assert.AreEqual(0xC5F5, cpu.Regs.PC);
            //@ 0xDB7B 6C 00 02  JMP ($0200) = DB7E              A:DB X:07 Y:00 P:E5 SP:FB
            cpu.Regs.Set(0xDB, 0x07, 0, 0xE5, 0xFB);
            cpu.Regs.PC = 0xDB7B;
            cpu.Mem.Write(0x0200, 0x7E);
            cpu.Mem.Write(0x0201, 0xDB);
            cpu.Jmp(Ricoh2A03.Decode(0x6C), 0x00, 0x02);
            AssertRegisterValues(new Ricoh2A03.Registers(0xDB, 0x07, 0, 0xE5, 0xFB), cpu.Regs);
            Assert.AreEqual(0xDB7E, cpu.Regs.PC);
        }

        [TestMethod()]
        public void LdxTest()
        {
            var cpu = new Ricoh2A03();
            //@ 0xC5F5 A2 00     LDX #$00                        A:00 X:00 Y:00 P:24 SP:FD
            cpu.Regs.Set(0, 0, 0, 0x24, 0xFD);
            cpu.Ldx(Ricoh2A03.Decode(0xA2), 0);
            // expected result A:00 X:00 Y:00 P:26 SP:FD
            AssertRegisterValues(new Ricoh2A03.Registers(0, 0, 0, 0x26, 0xFD), cpu.Regs);

            //@ 0xD1F8  A6 78     LDX $78 = 55                    A:23 X:00 Y:11 P:67 SP:FB
            cpu.Regs.Set(0x23, 0, 0x11, 0x67, 0xFB);
            cpu.Mem.Write(0x78, 0x55);
            cpu.Ldx(Ricoh2A03.Decode(0xA6), 0x78);
            // expected result A:23 X:55 Y:11 P:65 SP:FB
            AssertRegisterValues(new Ricoh2A03.Registers(0x23, 0x55, 0x11, 0x65, 0xFB), cpu.Regs);
        }

        [TestMethod()]
        public void StxTest()
        {
            var cpu = new Ricoh2A03();
            //@ 0xC5F9  86 10     STX $10 = 00                    A:00 X:00 Y:00 P:26 SP:FD CYC: 24 SL:241
            cpu.Regs.Set(0x00, 0x00, 0x00, 0x26, 0xFD);
            cpu.Stx(Ricoh2A03.Decode(0x86), 0x10);
            //expected result cpu.Mem[0x10]=X and A:00 X:00 Y:00 P:26 SP:FD CYC: 33 SL:241
            AssertMemoryValue(cpu.Regs.X, cpu.Mem, 0x10);

            //@ 0xCDAE  8E FF 07  STX $07FF = 00                  A:00 X:FB Y:99 P:A5 SP:FB CYC: 64 SL:257
            cpu.Regs.Set(0, 0xFB, 0x99, 0xA5, 0xFB);
            cpu.Stx(Ricoh2A03.Decode(0x8E), 0xFF, 0x07);
            //expected result cpu.Mem[0x07FF]=X and A:00 X:FB Y:99 P:A5 SP:FB CYC: 76 SL:257
            AssertMemoryValue(cpu.Regs.X, cpu.Mem, 0x07FF);

            //@ 0xDEFE  96 80     STX $80,Y @ 7F = 00             A:47 X:69 Y:FF P:24 SP:FB CYC: 40 SL:88
            cpu.Regs.Set(0x47, 0x69, 0xFF, 0x24, 0xFB);
            cpu.Stx(Ricoh2A03.Decode(0x96), 0x80);
            //expected result cpu.Mem[0xFF & (0x80+Y)]=X and A:47 X:69 Y:FF P:24 SP:FB CYC: 52 SL:88
            AssertMemoryValue(cpu.Regs.X, cpu.Mem, (byte)(0xFF & (0x80 + cpu.Regs.Y)));
        }
    }
}
