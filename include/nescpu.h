#ifndef NESCPU_H
#define NESCPU_H

//DEBUG this will later be used by unit tests
#define LOG_EXECUTION

#include <cstdlib>
#include <bitset>
#include <iomanip>
#include <iostream>
#include "opcodes.h"

#define RAM_SIZE 2048
#define ROM_BASE_ADDRESS 0x8000

enum AdressingMode
{
	IMMEDIATE,
	ZERO_PAGE,
	ZERO_PAGE_X,
	ABSOLUTE,
	ABSOLUTE_X,
	ABSOLUTE_Y
};

enum StatusFlags
{
	CARRY = 0,
	ZERO = 1,
	INTERRUPT_DISABLE = 2,
	DECIMAL_MODE = 3,
	OVERFLOW = 6,
	NEGATIVE = 7
};

///@brief This is the system's CPU
class NESCPU
{
public:
	///@brief
	///@param numCycles Number of cycles to execute before returning.
	///@returns Actual number of executed cycles (might differ from numCycles)
	int Execute(int numCycles);
	
	void Reset();
	void SetRomPtr(const unsigned char* rom);

private:
	void DumpRegisters();
	unsigned char ReadMem(unsigned short address);

	unsigned char m_ram[RAM_SIZE];
	const unsigned char* m_rom;
	unsigned short m_pc;	///< program counter register
	unsigned char m_sp;		///< stack pointer register
	unsigned char m_a;		///< accumulator register
	unsigned char m_x;		///< x index register
	unsigned char m_y;		///< y index register
	std::bitset<8> m_p;		///< processor status register
};

int NESCPU::Execute(int numCycles)
{
	int executedCycles = 0;
	while(executedCycles < numCycles)
	{
#ifdef LOG_EXECUTION
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
#ifdef LOG_EXECUTION
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

#endif	//NESCPU_H
