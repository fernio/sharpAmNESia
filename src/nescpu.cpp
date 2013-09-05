#include <iomanip>
#include <iostream>
#include <sstream>
#include "nescpu.hpp"
#include "opcodes.hpp"
#include "opcodesInfo.hpp"	

#define ROM_BASE_ADDRESS 0x8000
#define PIXELS_PER_CYCLE 3		//only on NTSC

NESCPU::NESCPU() : m_rom(nullptr)
{
}

int NESCPU::Execute(int numCycles)
{
	int executedCycles = 0;
	while(executedCycles < numCycles)
	{
		unsigned opcode = ReadMem(m_pc);
#ifdef UNIT_TESTING
		static std::ostringstream temp;
		int startingCycles = executedCycles;
		const std::string regs(DumpRegisters());
		//program counter
		PrintBytes(std::cout, m_pc, 4);
		std::cout << "  ";
		//bytes read
		temp.str("");
		for(unsigned i = 0; i < s_opcodesInfo[opcode].m_numBytes; ++i)
		{
			PrintBytes(temp, ReadMem(m_pc+i), 2);
			temp << " ";
		}
		std::cout << std::setw(10) << std::left << temp.str();
		//instruction
		std::cout << s_opcodesInfo[opcode].m_mnemonic << " ";
		//prepare for outputting instruction argument
		temp.str("");
		std::cout << std::setw(28) << std::left;
#endif 
		switch(opcode)
		{
			case BCS:
				if(m_p[CARRY])
				{
					//branch
					if((m_pc & 0xFF) + ReadMem(m_pc+1) > 0xFF)
					{
						//branch to next page
						executedCycles += 2;
					}
					else
					{
						//branch on same page
						++executedCycles;
					}
					m_pc += ReadMem(m_pc+1);
					m_pc += s_opcodesInfo[opcode].m_numBytes;
				}
				else
				{
					//no branch
					m_pc += s_opcodesInfo[opcode].m_numBytes;
				}
#ifdef UNIT_TESTING
				temp << "$";
				PrintBytes(temp, m_pc, 4);
				std::cout << temp.str();
#endif
				break;
			// case CLD:
				// m_p[DECIMAL_MODE] = 0;
				// m_pc += s_opcodesInfo[opcode].m_numBytes;
				// break;
			case JMP_ABSOLUTE:
				m_pc = ReadMem(m_pc+1) | (ReadMem(m_pc+2) << 8);
#ifdef UNIT_TESTING
				temp << "$";
				PrintBytes(temp, m_pc, 4);
				std::cout << temp.str();
#endif
				break;
			case JSR:
				Push(m_pc);
				m_pc = ReadMem(m_pc+1) | (ReadMem(m_pc+2) << 8);
#ifdef UNIT_TESTING
				temp << "$";
				PrintBytes(temp, m_pc, 4);
				std::cout << temp.str();
#endif
				break;
			case LDX_IMMEDIATE:
				m_x = ReadMem(m_pc+1);
				m_p[NEGATIVE] = m_x < 0;
				m_p[ZERO] = m_x == 0;
				m_pc += s_opcodesInfo[opcode].m_numBytes;
#ifdef UNIT_TESTING
				temp << "#$";
				PrintBytes(temp, m_x, 2);
				std::cout << temp.str();
#endif
				break;
			case NOP:
				m_pc += s_opcodesInfo[opcode].m_numBytes;
#ifdef UNIT_TESTING
				std::cout << " ";
#endif
				break;
			case SEC:
				m_p[CARRY] = true;
				m_pc += s_opcodesInfo[opcode].m_numBytes;
#ifdef UNIT_TESTING
				std::cout << " ";
#endif
				break;
			case STX_ZEROPAGE:
#ifdef UNIT_TESTING
				{
					temp << "$";
					uint8_t arg = ReadMem(m_pc+1);
					PrintBytes(temp, arg, 2);
					temp << " = ";
					PrintBytes(temp, ReadMem(arg), 2);
					std::cout << temp.str();
				}
#endif
				WriteMem(ReadMem(m_pc+1), m_x);
				m_pc += s_opcodesInfo[opcode].m_numBytes;
				break;
			// case SEI:
				// m_p[INTERRUPT_DISABLE] = 1;
				// m_pc += s_opcodesInfo[opcode].m_numBytes;
				// break;
			default:
				std::cerr << "unknown opcode " << std::showbase << std::hex
					<< opcode << std::endl;
				exit(EXIT_FAILURE);
		}
		executedCycles += s_opcodesInfo[opcode].m_numCycles;
#ifdef UNIT_TESTING
		
		std::cout << regs << std::dec << " CYC:" << startingCycles*PIXELS_PER_CYCLE
				<< " SL:" << 0 //TODO: scanline counter
				<< "\n";
#endif
	}
	return executedCycles;
}

std::string NESCPU::DumpRegisters()
{
	static std::ostringstream temp;
	temp.str("");
	//~ temp << "A:" << setdataprint(2) << m_a
		//~ << " X:" << setdataprint(2) << m_x
		//~ << " Y:" << setdataprint(2) << m_y
		//~ << " P:" << setdataprint(2) << m_p.to_ulong()
		//~ << " SP:" << setdataprint(2) << m_sp;
	temp << "A:";
	PrintBytes(temp, m_a, 2);
	temp << " X:";
	PrintBytes(temp, m_x, 2);
	temp << " Y:";
	PrintBytes(temp, m_y, 2);
	temp << " P:";
	PrintBytes(temp, m_p.to_ulong(), 2);
	temp << " SP:";
	PrintBytes(temp, m_sp, 2);
	return temp.str();
}

uint8_t NESCPU::ReadMem(uint16_t address)
{
	if(address < 0x2000)
	{
		//RAM read, wrap around to simulate mirroring
		return m_ram[address % RAM_SIZE];
	}
	else if(address >= ROM_BASE_ADDRESS)
	{
		//ROM read
		return m_rom[address - ROM_BASE_ADDRESS];
	}
	//TODO complete other adress ranges
	return 0;
}

void NESCPU::PowerUp()
{
	//first call Reset, to set the value of the PC, then set the rest of the registers
	Reset();
	m_p = 0x24;
	m_sp = 0xFD;
	m_a = 0;
	m_x = 0;
	m_y = 0;
}

void NESCPU::PrintBytes(std::ostream& stream, unsigned bytes, unsigned dataWidth)
{
	stream << std::hex << std::uppercase << std::setw(dataWidth) << std::right << std::setfill('0')
		<< bytes << std::setfill(' ');
}

void NESCPU::Push(uint8_t data)
{
	WriteMem(m_sp, data);
	--m_sp;
}

void NESCPU::Push(uint16_t data)
{
	//push high byte first, then low byte
	Push(static_cast<uint8_t>((data >> 8) & 0xFF));
	Push(static_cast<uint8_t>(data & 0xFF));	
}

void NESCPU::Reset()
{
	m_p[INTERRUPT_DISABLE] = 1;
	m_pc = ReadMem(0xFFFC) + (ReadMem(0xFFFD)<<8);
	//as seen on NesDev Wiki
	m_sp -= 3;
}

void NESCPU::SetRomPtr(const uint8_t* rom)
{
	m_rom = rom;
}

void NESCPU::WriteMem(uint16_t address, uint8_t data)
{
	if(address < 0x2000)
	{
		//RAM write, wrap around to simulate mirroring
		m_ram[address % RAM_SIZE] = data;
	}	
}
