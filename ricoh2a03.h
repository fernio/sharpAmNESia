#ifndef RICOH2A03_H
#define RICOH2A03_H

#include <cstdlib>
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

///@brief This is the system's CPU
class Ricoh2A03
{
public:
	int Execute(int numCycles);
	void Reset();
	void SetRomPtr(const unsigned char* rom);

private:
	unsigned char ReadMem(unsigned short address);

	unsigned char m_ram[RAM_SIZE];
	const unsigned char* m_rom;
	unsigned short m_pc;	///< program counter register
	unsigned char m_sp;		///< stack pointer register
	unsigned char m_a;		///< accumulator register
	unsigned char m_x;		///< x index register
	unsigned char m_y;		///< y index register
	struct ProcessorStatusRegister
	{
		//TODO: put all these flags into a one byte variable
		bool m_carry;
		bool m_zero;
		bool m_intDisabled;
		bool m_decimal;
		bool m_break;
		bool m_unused;
		bool m_overflow;
		bool m_negative;
	} m_p;
};

int Ricoh2A03::Execute(int numCycles)
{
	while(numCycles > 0)
	{
		switch(ReadMem(m_pc))
		{
			case CLD:
				m_p.m_decimal = false;
				m_pc += 1;
				numCycles -= 2;
				break;
			case SEI:
				m_p.m_intDisabled = true;
				m_pc += 1;
				numCycles -= 2;
				break;
			default:
				std::cerr << "unknown opcode " << std::showbase << std::hex
					<< static_cast<unsigned>(ReadMem(m_pc)) << std::endl;
				exit(EXIT_FAILURE);
		}
	}
	return numCycles;
}

unsigned char Ricoh2A03::ReadMem(unsigned short address)
{
	std::cout << "info: ReadMem at address " << std::showbase << std::hex
		<< address << std::endl;
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

void Ricoh2A03::Reset()
{
	m_p.m_intDisabled = true;
	m_pc = ReadMem(0xFFFC) + (ReadMem(0xFFFD)<<8);
}

void Ricoh2A03::SetRomPtr(const unsigned char* rom)
{
	m_rom = rom;
}

#endif	//RICOH2A03_H
