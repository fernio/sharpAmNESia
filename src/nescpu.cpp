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
			PrintBytes(temp, static_cast<unsigned>(ReadMem(m_pc+i)), 2);
			temp << " ";
		}
		std::cout << std::setw(10) << temp.str();
		//instruction
		std::cout << s_opcodesInfo[opcode].m_mnemonic << " ";
		//prepare for outputting instruction argument
		temp.str("");
		std::cout << std::setw(28) << std::left;
#endif 
		switch(opcode)
		{
			// case CLD:
				// m_p[DECIMAL_MODE] = 0;
				// m_pc += s_opcodesInfo[opcode].m_numBytes;
				// break;
			case JMP_ABSOLUTE:
				m_pc = ReadMem(m_pc+1) + (ReadMem(m_pc+2) << 8);
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
				PrintBytes(temp, static_cast<unsigned>(m_x), 2);
				std::cout << temp.str();
#endif
				break;
			// case SEI:
				// m_p[INTERRUPT_DISABLE] = 1;
				// m_pc += s_opcodesInfo[opcode].m_numBytes;
				// break;
			default:
				std::cerr << "unknown opcode " << std::showbase << std::hex
					<< static_cast<unsigned>(ReadMem(m_pc)) << std::endl;
				exit(EXIT_FAILURE);
		}
		executedCycles += s_opcodesInfo[opcode].m_numCycles;
#ifdef UNIT_TESTING
		
		std::cout << regs << std::dec << " CYC:" << startingCycles*PIXELS_PER_CYCLE
				<< " SL:" << 0 //TODO: is it scanlines?
				<< std::endl;
#endif
	}
	return executedCycles;
}

std::string NESCPU::DumpRegisters()
{
	static std::ostringstream temp;
	temp.str("");
	temp << "A:";
	PrintBytes(temp, static_cast<unsigned>(m_a), 2);
	temp << " X:";
	PrintBytes(temp, static_cast<unsigned>(m_x), 2);
	temp << " Y:";
	PrintBytes(temp, static_cast<unsigned>(m_y), 2);
	temp << " P:";
	PrintBytes(temp, m_p.to_ulong(), 2);
	temp << " SP:";
	PrintBytes(temp, static_cast<unsigned>(m_sp), 2);
	return temp.str();
}

unsigned char NESCPU::ReadMem(unsigned short address)
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
	stream << std::hex << std::uppercase << std::setw(dataWidth) << std::left << std::setfill('0')
		<< bytes << std::setfill(' ');
}

void NESCPU::Reset()
{
	m_p[INTERRUPT_DISABLE] = 1;
	m_pc = ReadMem(0xFFFC) + (ReadMem(0xFFFD)<<8);
	//as seen on NesDev Wiki
	m_sp -= 3;
}

void NESCPU::SetRomPtr(const unsigned char* rom)
{
	m_rom = rom;
}
