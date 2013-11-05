#ifndef NESCPU_HPP
#define NESCPU_HPP

//DEBUG this will later be used by unit tests
#define LOG_EXECUTION

#include <cstdlib>
#include <bitset>

#define RAM_SIZE 2048
/*
enum AdressingMode
{
	IMMEDIATE,
	ZERO_PAGE,
	ZERO_PAGE_X,
	ABSOLUTE,
	ABSOLUTE_X,
	ABSOLUTE_Y
};
*/
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
	bool SetLogFile(const char* logFilename);
	void SetRomPtr(const uint8_t* rom);

#ifdef UNIT_TESTING
	//allow unit test to change internal state
	void SetPC(unsigned pc) { m_pc = pc; }
#endif

private:
	std::string DumpRegisters();
	unsigned PopByte();
	unsigned PopWord();
	void PushByte(unsigned data);
	void PushWord(unsigned data);
	unsigned ReadMem(unsigned address);
	void WriteMem(unsigned address, unsigned data);

	uint8_t m_ram[RAM_SIZE];
	const uint8_t* m_rom;
	unsigned m_pc;		///< program counter register
	unsigned m_sp;		///< stack pointer register
	unsigned m_a;		///< accumulator register
	unsigned m_x;		///< x index register
	unsigned m_y;		///< y index register
	std::bitset<8> m_p;	///< processor status register
	
	//opcode emulation
	///@brief
	///@param conditionResult Result of condition tested by emulated kind of branch
	///@returns Number of extra cycles run due to branching
	unsigned BranchOnCondition(bool conditionResult, unsigned opcode);

	///@brief Compare register and memory values, and update status register with result
	///@param reg Value of register being compared
	///@param mem Value from memory begin compared
	void Compare(unsigned reg, unsigned mem);

	void LoadRegister(unsigned& reg, unsigned address, unsigned opcode);
	//void Jump(
};

#endif	//NESCPU_HPP
