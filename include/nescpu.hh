#ifndef NESCPU_H
#define NESCPU_H

//DEBUG this will later be used by unit tests
#define LOG_EXECUTION

#include <cstdlib>
#include <bitset>
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

#endif	//NESCPU_H
