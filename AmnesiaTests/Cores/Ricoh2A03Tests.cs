using Microsoft.VisualStudio.TestTools.UnitTesting;
using Amnesia.Cores;
using System;
using System.Collections.Generic;
using System.Text;

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

        [TestMethod()]
        public void InitialStateTest()
        {
            var cpu = new Ricoh2A03();
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
            var cpu = new Ricoh2A03();

            //@ 0xC91C  69 69     ADC #$69                        A:00 X:00 Y:00 P:6E SP:FB
            cpu.Regs.Set(0, 0, 0, 0x6E, 0xFB);
            cpu.Adc(Ricoh2A03.Decode(0x69), 0x69);
            //expected A: 69 X: 00 Y: 00 P: 2C SP:FB
            Assert.AreEqual(0x69, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0x2C, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xC936  69 69     ADC #$69                        A:01 X:00 Y:00 P:6D SP:FB
            cpu.Regs.Set(0x01, 0, 0, 0x6D, 0xFB);
            cpu.Adc(Ricoh2A03.Decode(0x69), 0x69);
            // expected result A:6B X:00 Y:00 P:2C SP:FB
            Assert.AreEqual(0x6B, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0x2C, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xC94F  69 7F     ADC #$7F                        A:7F X:00 Y:00 P:25 SP:FB
            cpu.Regs.Set(0x7F, 0, 0, 0x25, 0xFB);
            cpu.Adc(Ricoh2A03.Decode(0x69), 0x7F);
            //expected result A:FF X:00 Y:00 P:E4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0xE4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xC968  69 80     ADC #$80                        A:7F X:00 Y:00 P:64 SP:FB
            cpu.Regs.Set(0x7F, 0, 0, 0x64, 0xFB);
            cpu.Adc(Ricoh2A03.Decode(0x69), 0x80);
            //expected result A:FF X:00 Y:00 P:A4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0xA4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xC980  69 80     ADC #$80                        A:7F X:00 Y:00 P:25 SP:FB
            cpu.Regs.Set(0x7F, 0, 0, 0x25, 0xFB);
            cpu.Adc(Ricoh2A03.Decode(0x69), 0x80);
            // expected result A:00 X: 00 Y: 00 P: 27 SP: FB
            Assert.AreEqual(0, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0x27, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xD098  61 80     ADC($80, X) @ 80 = 0200 = 69    A: 00 X: 00 Y: 60 P: 66 SP: FB
            cpu.Regs.Set(0, 0, 0x60, 0x66, 0xFB);
            cpu.Mem.Write(0x80, 0x00);
            cpu.Mem.Write(0x81, 0x02);
            cpu.Mem.Write(0x200, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x61), 0x80);
            // expected result A:69 X:00 Y:60 P:24 SP:FB
            Assert.AreEqual(0x69, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0x60, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xD0A1  61 80     ADC ($80,X) @ 80 = 0200 = 69    A:00 X:00 Y:61 P:67 SP:FB
            cpu.Regs.Set(0, 0, 0x61, 0x67, 0xFB);
            cpu.Mem.Write(0x80, 0x00);
            cpu.Mem.Write(0x81, 0x02);
            cpu.Mem.Write(0x200, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x61), 0x80);
            // expected result A:6A X:00 Y:61 P:24 SP:FB
            Assert.AreEqual(0x6A, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0x61, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xD0AF  61 80     ADC($80, X) @ 80 = 0200 = 7F    A: 7F X: 00 Y: 62 P: 25 SP: FB
            cpu.Regs.Set(0x7F, 0, 0x62, 0x25, 0xFB);
            cpu.Mem.Write(0x80, 0x00);
            cpu.Mem.Write(0x81, 0x02);
            cpu.Mem.Write(0x200, 0x7F);
            cpu.Adc(Ricoh2A03.Decode(0x61), 0x80);
            //expected result A:FF X:00 Y:62 P:E4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0x62, cpu.Regs.Y);
            Assert.AreEqual(0xE4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xD0BD  61 80     ADC ($80,X) @ 80 = 0200 = 80    A:7F X:00 Y:63 P:64 SP:FB
            cpu.Regs.Set(0x7F, 0, 0x63, 0x64, 0xFB);
            cpu.Mem.Write(0x80, 0x00);
            cpu.Mem.Write(0x81, 0x02);
            cpu.Mem.Write(0x200, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x61), 0x80);
            //expected result A:FF X:00 Y:63 P:A4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0x63, cpu.Regs.Y);
            Assert.AreEqual(0xA4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xD0C6  61 80     ADC($80, X) @ 80 = 0200 = 80    A: 7F X: 00 Y: 64 P: 25 SP: FB
            cpu.Regs.Set(0x7F, 0, 0x64, 0x25, 0xFB);
            cpu.Mem.Write(0x80, 0x00);
            cpu.Mem.Write(0x81, 0x02);
            cpu.Mem.Write(0x200, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x61), 0x80);
            //expected result A:00 X:00 Y:64 P:27 SP:FB
            Assert.AreEqual(0, cpu.Regs.A);
            Assert.AreEqual(0, cpu.Regs.X);
            Assert.AreEqual(0x64, cpu.Regs.Y);
            Assert.AreEqual(0x27, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xD2B5  65 78     ADC $78 = 69                    A: 00 X: 33 Y: 84 P: 66 SP: FB
            cpu.Regs.Set(0, 0x33, 0x84, 0x66, 0xFB);
            cpu.Mem.Write(0x78, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x65), 0x78);
            //expected result A:69 X:33 Y:84 P:24 SP:FB
            Assert.AreEqual(0x69, cpu.Regs.A);
            Assert.AreEqual(0x33, cpu.Regs.X);
            Assert.AreEqual(0x84, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //@ 0xD2BE  65 78     ADC $78 = 69                    A: 00 X: 33 Y: 85 P: 67 SP: FB
            cpu.Regs.Set(0, 0x33, 0x85, 0x67, 0xFB);
            cpu.Mem.Write(0x78, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x65), 0x78);
            //expected result A:6A X:33 Y:85 P:24 SP:FB
            Assert.AreEqual(0x6A, cpu.Regs.A);
            Assert.AreEqual(0x33, cpu.Regs.X);
            Assert.AreEqual(0x85, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //D2CB  65 78     ADC $78 = 7F                    A: 7F X: 33 Y: 86 P: 25 SP: FB CYC:231 SL: 18
            cpu.Regs.Set(0x7F, 0x33, 0x86, 0x25, 0xFB);
            cpu.Mem.Write(0x78, 0x7F);
            cpu.Adc(Ricoh2A03.Decode(0x65), 0x78);
            //A:FF X:33 Y:86 P:E4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0x33, cpu.Regs.X);
            Assert.AreEqual(0x86, cpu.Regs.Y);
            Assert.AreEqual(0xE4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //D2D8  65 78     ADC $78 = 80                    A: 7F X: 33 Y: 87 P: 64 SP: FB CYC: 43 SL: 19
            cpu.Regs.Set(0x7F, 0x33, 0x87, 0x64, 0xFB);
            cpu.Mem.Write(0x78, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x65), 0x78);
            //A:FF X:33 Y:87 P:A4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0x33, cpu.Regs.X);
            Assert.AreEqual(0x87, cpu.Regs.Y);
            Assert.AreEqual(0xA4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //D2E1  65 78     ADC $78 = 80                    A: 7F X: 33 Y: 88 P: 25 SP: FB CYC:178 SL: 19
            cpu.Regs.Set(0x7F, 0x33, 0x88, 0x25, 0xFB);
            cpu.Mem.Write(0x78, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x65), 0x78);
            //A:00 X:33 Y:88 P:27 SP:FB
            Assert.AreEqual(0, cpu.Regs.A);
            Assert.AreEqual(0x33, cpu.Regs.X);
            Assert.AreEqual(0x88, cpu.Regs.Y);
            Assert.AreEqual(0x27, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //D659  6D 78 06  ADC $0678 = 69                  A: 00 X: 33 Y: BE P:66 SP: FB CYC:308 SL: 38
            cpu.Regs.Set(0, 0x33, 0xBE, 0x66, 0xFB);
            cpu.Mem.Write(0x678, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x6D), 0x78, 0x06);
            //A:69 X:33 Y:BE P:24 SP:FB
            Assert.AreEqual(0x69, cpu.Regs.A);
            Assert.AreEqual(0x33, cpu.Regs.X);
            Assert.AreEqual(0xBE, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //D663  6D 78 06  ADC $0678 = 69                  A: 00 X: 33 Y: BF P:67 SP: FB CYC:108 SL: 39
            cpu.Regs.Set(0, 0x33, 0xBF, 0x67, 0xFB);
            cpu.Mem.Write(0x678, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x6D), 0x78, 0x06);
            //A:6A X:33 Y:BF P:24 SP:FB
            Assert.AreEqual(0x6A, cpu.Regs.A);
            Assert.AreEqual(0x33, cpu.Regs.X);
            Assert.AreEqual(0xBF, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //D672  6D 78 06  ADC $0678 = 7F                  A:7F X:33 Y:C0 P:25 SP:FB CYC:264 SL:39
            cpu.Regs.Set(0x7F, 0x33, 0xC0, 0x25, 0xFB);
            cpu.Mem.Write(0x678, 0x7F);
            cpu.Adc(Ricoh2A03.Decode(0x6D), 0x78, 0x06);
            //A:FF X:33 Y:C0 P:E4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0x33, cpu.Regs.X);
            Assert.AreEqual(0xC0, cpu.Regs.Y);
            Assert.AreEqual(0xE4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //D681  6D 78 06  ADC $0678 = 80                  A:7F X:33 Y:C1 P:64 SP:FB CYC: 82 SL:40
            cpu.Regs.Set(0x7F, 0x33, 0xC1, 0x64, 0xFB);
            cpu.Mem.Write(0x678, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x6D), 0x78, 0x06);
            //A:FF X:33 Y:C1 P:A4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0x33, cpu.Regs.X);
            Assert.AreEqual(0xC1, cpu.Regs.Y);
            Assert.AreEqual(0xA4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //D68B  6D 78 06  ADC $0678 = 80                  A: 7F X: 33 Y: C2 P:25 SP: FB CYC:220 SL: 40
            cpu.Regs.Set(0x7F, 0x33, 0xC2, 0x25, 0xFB);
            cpu.Mem.Write(0x678, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x6D), 0x78, 0x06);
            //A:00 X:33 Y:C2 P:27 SP:FB
            Assert.AreEqual(0, cpu.Regs.A);
            Assert.AreEqual(0x33, cpu.Regs.X);
            Assert.AreEqual(0xC2, cpu.Regs.Y);
            Assert.AreEqual(0x27, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //DA01  71 33     ADC($33),Y = 0400 @ 0400 = 69  A: 00 X: F3 Y:00 P: 66 SP: FB CYC:151 SL: 58
            cpu.Regs.Set(0, 0xF3, 0, 0x66, 0xFB);
            cpu.Mem.Write(0x33, 0x00);
            cpu.Mem.Write(0x34, 0x04);
            cpu.Mem.Write(0x400, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x71), 0x33);
            //A:69 X:F3 Y:00 P:24 SP:FB
            Assert.AreEqual(0x69, cpu.Regs.A);
            Assert.AreEqual(0xF3, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //DA15  71 33     ADC($33),Y = 0400 @ 0400 = 69  A: 00 X: F4 Y:00 P: 67 SP: FB CYC:226 SL: 58
            cpu.Regs.Set(0, 0xF4, 0, 0x67, 0xFB);
            cpu.Mem.Write(0x33, 0x00);
            cpu.Mem.Write(0x34, 0x04);
            cpu.Mem.Write(0x400, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x71), 0x33);
            //A:6A X:F4 Y:00 P:24 SP:FB
            Assert.AreEqual(0x6A, cpu.Regs.A);
            Assert.AreEqual(0xF4, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //DA2B  71 33     ADC($33),Y = 0400 @ 0400 = 7F  A: 7F X: F5 Y:00 P: 25 SP: FB CYC:310 SL: 58
            cpu.Regs.Set(0x7F, 0xF5, 0, 0x25, 0xFB);
            cpu.Mem.Write(0x33, 0x00);
            cpu.Mem.Write(0x34, 0x04);
            cpu.Mem.Write(0x400, 0x7F);
            cpu.Adc(Ricoh2A03.Decode(0x71), 0x33);
            //A:FF X:F5 Y:00 P:E4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0xF5, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0xE4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //DA44  71 33     ADC($33),Y = 0400 @ 0400 = 80  A: 7F X: F6 Y:00 P: 64 SP: FB CYC: 62 SL: 59
            cpu.Regs.Set(0x7F, 0xF6, 0, 0x64, 0xFB);
            cpu.Mem.Write(0x33, 0x00);
            cpu.Mem.Write(0x34, 0x04);
            cpu.Mem.Write(0x400, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x71), 0x33);
            //A:FF X:F6 Y:00 P:A4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0xF6, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0xA4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //DA5C  71 33     ADC($33),Y = 0400 @ 0400 = 80  A: 7F X: F7 Y:00 P: 25 SP: FB CYC:152 SL: 59
            cpu.Regs.Set(0x7F, 0xF7, 0, 0x25, 0xFB);
            cpu.Mem.Write(0x33, 0x00);
            cpu.Mem.Write(0x34, 0x04);
            cpu.Mem.Write(0x400, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x71), 0x33);
            //A:00 X:F7 Y:00 P:27 SP:FB
            Assert.AreEqual(0, cpu.Regs.A);
            Assert.AreEqual(0xF7, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0x27, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //E034  79 00 04  ADC $0400,Y @ 0400 = 69         A: 00 X: 3F Y: 00 P: 66 SP: FB CYC: 39 SL: 66
            cpu.Regs.Set(0, 0x3F, 0, 0x66, 0xFB);
            cpu.Mem.Write(0x400, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x79), 0, 0x04);
            //A:69 X:3F Y:00 P:24 SP:FB
            Assert.AreEqual(0x69, cpu.Regs.A);
            Assert.AreEqual(0x3F, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //E049  79 00 04  ADC $0400,Y @ 0400 = 69         A: 00 X: 40 Y: 00 P: 67 SP: FB CYC:111 SL: 66
            cpu.Regs.Set(0, 0x40, 0, 0x67, 0xFB);
            cpu.Mem.Write(0x400, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x79), 0, 0x04);
            //A:6A X:40 Y:00 P:24 SP:FB
            Assert.AreEqual(0x6A, cpu.Regs.A);
            Assert.AreEqual(0x40, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //E060  79 00 04  ADC $0400,Y @ 0400 = 7F         A: 7F X: 41 Y: 00 P: 25 SP: FB CYC:192 SL: 66
            cpu.Regs.Set(0x7F, 0x41, 0, 0x25, 0xFB);
            cpu.Mem.Write(0x400, 0x7F);
            cpu.Adc(Ricoh2A03.Decode(0x79), 0, 0x04);
            //A:FF X:41 Y:00 P:E4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0x41, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0xE4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //E07A  79 00 04  ADC $0400,Y @ 0400 = 80         A: 7F X: 42 Y: 00 P: 64 SP: FB CYC:282 SL: 66
            cpu.Regs.Set(0x7F, 0x42, 0, 0x64, 0xFB);
            cpu.Mem.Write(0x400, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x79), 0, 0x04);
            //A:FF X:42 Y:00 P:A4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0x42, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0xA4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //E093  79 00 04  ADC $0400,Y @ 0400 = 80         A: 7F X: 43 Y: 00 P: 25 SP: FB CYC: 28 SL: 67
            cpu.Regs.Set(0x7F, 0x43, 0, 0x25, 0xFB);
            cpu.Mem.Write(0x400, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x79), 0, 0x04);
            //A:00 X:43 Y:00 P:27 SP:FB
            Assert.AreEqual(0, cpu.Regs.A);
            Assert.AreEqual(0x43, cpu.Regs.X);
            Assert.AreEqual(0, cpu.Regs.Y);
            Assert.AreEqual(0x27, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //DC8B  75 00     ADC $00,X @ 78 = 69             A: 00 X: 78 Y: 11 P: 66 SP: FB CYC: 95 SL: 74
            cpu.Regs.Set(0, 0x78, 0x11, 0x66, 0xFB);
            cpu.Mem.Write(0x78, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x75), 0);
            //A:69 X:78 Y:11 P:24 SP:FB
            Assert.AreEqual(0x69, cpu.Regs.A);
            Assert.AreEqual(0x78, cpu.Regs.X);
            Assert.AreEqual(0x11, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //DC94  75 00     ADC $00,X @ 78 = 69             A: 00 X: 78 Y: 12 P: 67 SP: FB CYC:236 SL: 74
            cpu.Regs.Set(0, 0x78, 0x12, 0x67, 0xFB);
            cpu.Mem.Write(0x78, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x75), 0);
            //A:6A X:78 Y:12 P:24 SP:FB
            Assert.AreEqual(0x6A, cpu.Regs.A);
            Assert.AreEqual(0x78, cpu.Regs.X);
            Assert.AreEqual(0x12, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //DCA1  75 00     ADC $00,X @ 78 = 7F             A: 7F X: 78 Y: 13 P: 25 SP: FB CYC: 48 SL: 75
            cpu.Regs.Set(0x7F, 0x78, 0x13, 0x25, 0xFB);
            cpu.Mem.Write(0x78, 0x7F);
            cpu.Adc(Ricoh2A03.Decode(0x75), 0);
            //A:FF X:78 Y:13 P:E4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0x78, cpu.Regs.X);
            Assert.AreEqual(0x13, cpu.Regs.Y);
            Assert.AreEqual(0xE4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //DCAE  75 00     ADC $00,X @ 78 = 80             A: 7F X: 78 Y: 14 P: 64 SP: FB CYC:204 SL: 75
            cpu.Regs.Set(0x7F, 0x78, 0x14, 0x64, 0xFB);
            cpu.Mem.Write(0x78, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x75), 0);
            //A:FF X:78 Y:14 P:A4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0x78, cpu.Regs.X);
            Assert.AreEqual(0x14, cpu.Regs.Y);
            Assert.AreEqual(0xA4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //DCB7  75 00     ADC $00,X @ 78 = 80             A: 7F X: 78 Y: 15 P: 25 SP: FB CYC:  1 SL: 76
            cpu.Regs.Set(0x7F, 0x78, 0x15, 0x25, 0xFB);
            cpu.Mem.Write(0x78, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x75), 0);
            //A:00 X:78 Y:15 P:27 SP:FB
            Assert.AreEqual(0, cpu.Regs.A);
            Assert.AreEqual(0x78, cpu.Regs.X);
            Assert.AreEqual(0x15, cpu.Regs.Y);
            Assert.AreEqual(0x27, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //E265  7D 00 06  ADC $0600,X @ 0678 = 69         A: 00 X: 78 Y: 59 P: 66 SP: FB CYC:242 SL: 92
            cpu.Regs.Set(0, 0x78, 0x59, 0x66, 0xFB);
            cpu.Mem.Write(0x678, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x7D), 0, 0x6);
            //A:69 X:78 Y:59 P:24 SP:FB
            Assert.AreEqual(0x69, cpu.Regs.A);
            Assert.AreEqual(0x78, cpu.Regs.X);
            Assert.AreEqual(0x59, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //E26F  7D 00 06  ADC $0600,X @ 0678 = 69         A:00 X:78 Y:5A P:67 SP:FB CYC: 42 SL:93
            cpu.Regs.Set(0, 0x78, 0x5A, 0x67, 0xFB);
            cpu.Mem.Write(0x678, 0x69);
            cpu.Adc(Ricoh2A03.Decode(0x7D), 0, 0x6);
            //A:6A X:78 Y:5A P:24 SP:FB
            Assert.AreEqual(0x6A, cpu.Regs.A);
            Assert.AreEqual(0x78, cpu.Regs.X);
            Assert.AreEqual(0x5A, cpu.Regs.Y);
            Assert.AreEqual(0x24, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //E27E  7D 00 06  ADC $0600,X @ 0678 = 7F         A: 7F X: 78 Y: 5B P:25 SP: FB CYC:198 SL: 93
            cpu.Regs.Set(0x7F, 0x78, 0x5B, 0x25, 0xFB);
            cpu.Mem.Write(0x678, 0x7F);
            cpu.Adc(Ricoh2A03.Decode(0x7D), 0, 0x6);
            //A:FF X:78 Y:5B P:E4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0x78, cpu.Regs.X);
            Assert.AreEqual(0x5B, cpu.Regs.Y);
            Assert.AreEqual(0xE4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //E28D  7D 00 06  ADC $0600,X @ 0678 = 80         A: 7F X: 78 Y: 5C P:64 SP: FB CYC: 16 SL: 94
            cpu.Regs.Set(0x7F, 0x78, 0x5C, 0x64, 0xFB);
            cpu.Mem.Write(0x678, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x7D), 0, 0x6);
            //A:FF X:78 Y:5C P:A4 SP:FB
            Assert.AreEqual(0xFF, cpu.Regs.A);
            Assert.AreEqual(0x78, cpu.Regs.X);
            Assert.AreEqual(0x5C, cpu.Regs.Y);
            Assert.AreEqual(0xA4, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);

            //E297  7D 00 06  ADC $0600,X @ 0678 = 80         A: 7F X: 78 Y: 5D P: 25 SP: FB CYC:154 SL: 94
            cpu.Regs.Set(0x7F, 0x78, 0x5D, 0x25, 0xFB);
            cpu.Mem.Write(0x678, 0x80);
            cpu.Adc(Ricoh2A03.Decode(0x7D), 0, 0x6);
            //A:00 X:78 Y:5D P:27 SP:FB
            Assert.AreEqual(0, cpu.Regs.A);
            Assert.AreEqual(0x78, cpu.Regs.X);
            Assert.AreEqual(0x5D, cpu.Regs.Y);
            Assert.AreEqual(0x27, cpu.Regs.P.AsByte());
            Assert.AreEqual(0xFB, cpu.Regs.SP);
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
            var cpu = new Ricoh2A03();
            //@ 0xC5F5 A2 00     LDX #$00                        A:00 X:00 Y:00 P:24 SP:FD
            cpu.Regs.Set(0, 0, 0, 0x24, 0xFD);
            cpu.Ldx(Ricoh2A03.Decode(0xA2), 0);
            // expected result A:00 X:00 Y:00 P:26 SP:FD
            AssertRegisterValues(new Ricoh2A03.Registers(0, 0, 0, 0x26, 0xFD), cpu.Regs);

            //@ 0xCAB3 A2 40     LDX #$40                        A:80 X:00 Y:80 P:E5 SP:FB CYC:283 SL:248
            cpu.Regs.Set(0x80, 0, 0x80, 0xE5, 0xFB);
            cpu.Ldx(Ricoh2A03.Decode(0xA2), 0x40);
            // expected result A:80 X:40 Y:80 P:65 SP:FB CYC:289 SL:248
            AssertRegisterValues(new Ricoh2A03.Registers(0x80, 0x40, 0x80, 0x65, 0xFB), cpu.Regs);

            //@ 0xCAEA  A2 80     LDX #$80                        A:80 X:40 Y:80 P:A4 SP:FB CYC: 83 SL:249
            cpu.Regs.Set(0x80, 0x40, 0x80, 0xA4, 0xFB);
            cpu.Ldx(Ricoh2A03.Decode(0xA2), 0x80);
            // expected result A:80 X:80 Y:80 P:A4 SP:FB CYC: 89 SL:249
            AssertRegisterValues(new Ricoh2A03.Registers(0x80, 0x80, 0x80, 0xA4, 0xFB), cpu.Regs);

            //
        }
    }
}