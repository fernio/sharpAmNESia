#ifndef RICOH2A03_H
#define RICOH2A03_H

#include <cstdlib>

#define RAM_SIZE 2048

enum AdressingMode
{
	IMMEDIATE,
	ZERO_PAGE,
	ZERO_PAGE_X,
	ABSOLUTE,
	ABSOLUTE_X,
	ABSOLUTE_Y,

}

///@brief This is the system's CPU
class Ricoh2A03
{
public:
private:
	unsigned char m_ram[RAM_SIZE];

	unsigned short m_pc;	///< program counter register
	unsigned char m_sp;		///< stack pointer register
	unsigned char m_a;		///< accumulator register
	unsigned char m_x;		///< x index register
	unsigned char m_y;		///< y index register
	struct ProcessorStatusRegister
	{
		//TODO: damn! bit fields order is platform dependent
		bool m_carry		: 1;
		bool m_zero			: 1;
		bool m_intDisabled 	: 1;
		bool m_decimal		: 1;
		bool m_break		: 1;
		bool m_unused		: 1;
		bool m_overflow		: 1;
		bool m_negative		: 1;
	} m_p;

	void Execute(int numCycles);
};

void Ricoh2A03::Execute(int numCycles)
{
	switch(m_rom[m_pc])
	{
		default:
			std::cerr << "unknown opcode " << std::ios::hex << std::endl;
			exit(EXIT_FAILURE);
	}
}

#endif	//RICOH2A03_H
