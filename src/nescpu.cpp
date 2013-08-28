#include <iomanip>
#include <iostream>
#include "nescpu.hpp"
#include "opcodes.hpp"
#include "opcodesInfo.hpp"	

NESCPU::NESCPU() : m_rom(nullptr)
{
}

int NESCPU::Execute(int numCycles)
{
	int executedCycles = 0;
	while(executedCycles < numCycles)
	{
#ifdef UNIT_TESTING
		std::cout << std::hex << static_cast<unsigned>(m_pc) << "  "
				<< static_cast<unsigned>(ReadMem(m_pc)) << " ";
		int startingCycles = executedCycles;
#endif 
		switch(ReadMem(m_pc))
		{
			case CLD:
				m_p[DECIMAL_MODE] = 0;
				m_pc += 1;
				executedCycles += 2;
				break;
			case JMP_ABSOLUTE:
				m_pc = ReadMem(m_pc+1) + (ReadMem(m_pc+2) << 8);
				executedCycles += 3;
				break;
			case SEI:
				m_p[INTERRUPT_DISABLE] = 1;
				m_pc += 1;
				executedCycles += 2;
				break;
			default:
				std::cerr << "unknown opcode " << std::showbase << std::hex
					<< static_cast<unsigned>(ReadMem(m_pc)) << std::endl;
				exit(EXIT_FAILURE);
		}
#ifdef UNIT_TESTING
		DumpRegisters();
		std::cout << std::dec << " CYC:" << startingCycles
				<< " SL:" << 0 //TODO: is it scanlines?
				<< std::endl;
#endif
	}
	return executedCycles;
}

void NESCPU::DumpRegisters()
{
	std::cout << std::hex << std::setw(2)
			<< "A:" << static_cast<unsigned>(m_a)
			<< " X:" << static_cast<unsigned>(m_x)
			<< " Y:" << static_cast<unsigned>(m_y)
			<< " P:" << m_p.to_ulong()
			<< " SP:" << static_cast<unsigned>(m_sp);
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
}

void NESCPU::PowerUp()
{
	m_sp = 0xFD;
	m_a = 0;
	m_x = 0;
	m_y = 0;
	m_p = 0x34;
	m_pc = 0;	//TODO double check
}

void NESCPU::Reset()
{
	m_p[INTERRUPT_DISABLE] = 1;
	m_pc = ReadMem(0xFFFC) + (ReadMem(0xFFFD)<<8);
	m_sp = 0xFD;
}

void NESCPU::SetRomPtr(const unsigned char* rom)
{
	m_rom = rom;
}
