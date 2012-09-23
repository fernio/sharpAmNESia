#ifndef RICOH2A03_H
#define RICOH2A03_H

#include <cstdlib>

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
	void Execute(int numCycles);
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

void Ricoh2A03::Execute(int numCycles)
{
	switch(ReadMem(m_pc))
	{
		default:
			std::cerr << "unknown opcode " << std::ios::hex << std::endl;
			exit(EXIT_FAILURE);
	}
}

unsigned char Ricoh2A03::ReadMem(unsigned short address)
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

void Ricoh2A03::SetRomPtr(const unsigned char* rom)
{
	m_rom = rom;
}

#endif	//RICOH2A03_H
