#ifndef NESCPU_HPP
#define NESCPU_HPP

//DEBUG this will later be used by unit tests
#define LOG_EXECUTION

#include <cstdlib>
#include <bitset>

#define RAM_SIZE 2048

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
	NESCPU();

	///@brief
	///@param numCycles Number of cycles to execute before returning.
	///@returns Actual number of executed cycles (might differ from numCycles)
	int Execute(int numCycles);
	
	void PowerUp();
	void Reset();
	void SetRomPtr(const uint8_t* rom);

#ifdef UNIT_TESTING
//allow unit tests to modify internal state of class
public:
#else
private:
#endif
	std::string DumpRegisters();
	void PrintBytes(std::ostream& stream, unsigned data, unsigned dataWidth);
	void Push(uint8_t data);
	void Push(uint16_t data);
	uint8_t ReadMem(uint16_t address);
	void WriteMem(uint16_t address, uint8_t data);

	uint8_t m_ram[RAM_SIZE];
	const uint8_t* m_rom;
	uint16_t m_pc;		///< program counter register
	uint8_t m_sp;		///< stack pointer register
	uint8_t m_a;		///< accumulator register
	uint8_t m_x;		///< x index register
	uint8_t m_y;		///< y index register
	std::bitset<8> m_p;	///< processor status register
	
	//opcode emulation
	//void Jump(
};

#endif	//NESCPU_HPP
