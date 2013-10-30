#include <iomanip>
#include <iostream>
#include <sstream>
#include "logHelper.hpp"
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
		int startingCycles = executedCycles;
		const std::string regs(DumpRegisters());
		//program counter
		std::cout << setdataprint(4) << m_pc << "  ";
		//bytes read
		static std::ostringstream temp;
		temp.str("");
		for(unsigned i = 0; i < s_opcodesInfo[opcode].m_numBytes; ++i)
		{
			temp << setdataprint(2) << ReadMem(m_pc+i) << " ";
		}
		std::cout << std::setw(10) << std::setfill(' ') << std::left << temp.str();
		//instruction
		std::cout << s_opcodesInfo[opcode].m_mnemonic << " ";
		//prepare for outputting instruction argument
		temp.str("");
		std::cout << std::setw(28) << std::left;
#endif 
		switch(opcode)
		{
			case BCC:
				executedCycles += BranchOnCondition(m_p[CARRY] == 0, opcode);
				break;
			case BCS:
				executedCycles += BranchOnCondition(m_p[CARRY] == 1, opcode);
				break;
			case BEQ:
				executedCycles += BranchOnCondition(m_p[ZERO] == 1, opcode);
				break;
			case BIT_ZEROPAGE:
				{
					unsigned arg = ReadMem(m_pc+1);
#ifdef UNIT_TESTING
					temp << "$" << setdataprint(2) << arg << " = " << setdataprint(2) << ReadMem(arg);
					std::cout << temp.str();
#endif
					unsigned data = ReadMem(arg);
					m_p[ZERO] = (data & m_a) == 0;
					m_p[NEGATIVE] = (data >> 7) & 1;
					m_p[OVERFLOW] = (data >> 6) & 1;
					m_pc += s_opcodesInfo[opcode].m_numBytes;
				}
				break;
			case BMI:
				executedCycles += BranchOnCondition(m_p[NEGATIVE] == 1, opcode);
				break;
			case BNE:
				executedCycles += BranchOnCondition(m_p[ZERO] == 0, opcode);
				break;
			case BPL:
				executedCycles += BranchOnCondition(m_p[NEGATIVE] == 0, opcode);
				break;
			case BVC:
				executedCycles += BranchOnCondition(m_p[OVERFLOW] == 0, opcode);
				break;
			case BVS:
				executedCycles += BranchOnCondition(m_p[OVERFLOW] == 1, opcode);
				break;
			case CLC:
#ifdef UNIT_TESTING
				std::cout << "";
#endif
				m_p[CARRY] = 0;
				m_pc += s_opcodesInfo[opcode].m_numBytes;
				break;
			// case CLD:
				// m_p[DECIMAL_MODE] = 0;
				// m_pc += s_opcodesInfo[opcode].m_numBytes;
				// break;
			case JMP_ABSOLUTE:
				m_pc = ReadMem(m_pc+1) | (ReadMem(m_pc+2) << 8);
#ifdef UNIT_TESTING
				temp << "$" << setdataprint(4) << m_pc;
				std::cout << temp.str();
#endif
				break;
			case JSR:
				PushWord(m_pc+2);
				m_pc = ReadMem(m_pc+1) | (ReadMem(m_pc+2) << 8);
#ifdef UNIT_TESTING
				temp << "$" << setdataprint(4) << m_pc;
				std::cout << temp.str();
#endif
				break;
			case LDA_IMMEDIATE:
#ifdef UNIT_TESTING
				temp << "#$" << setdataprint(2) << ReadMem(m_pc+1);
				std::cout << temp.str();
#endif
				LoadRegister(m_a, m_pc+1, opcode);
				break;
			case LDX_IMMEDIATE:
#ifdef UNIT_TESTING
				temp << "#$" << setdataprint(2) << ReadMem(m_pc+1);
				std::cout << temp.str();
#endif
				LoadRegister(m_x, m_pc+1, opcode);
				break;
			case NOP:
#ifdef UNIT_TESTING
				std::cout << " ";
#endif
				m_pc += s_opcodesInfo[opcode].m_numBytes;
				break;
			case RTS:
#ifdef UNIT_TESTING
				std::cout << " ";
#endif
				m_pc = PopWord();
				++m_pc;
				break;
			case SEC:
#ifdef UNIT_TESTING
				std::cout << " ";
#endif
				m_p[CARRY] = true;
				m_pc += s_opcodesInfo[opcode].m_numBytes;
				break;
			case SEI:
#ifdef UNIT_TESTING
				std::cout << " ";
#endif
				m_p[INTERRUPT_DISABLE] = true;
				m_pc += s_opcodesInfo[opcode].m_numBytes;
				break;
			case STA_ZEROPAGE:
#ifdef UNIT_TESTING
				{
					unsigned arg = ReadMem(m_pc+1);
					temp << "$" << setdataprint(2) << arg << " = " << setdataprint(2) << ReadMem(arg);
					std::cout << temp.str();
				}
#endif
				WriteMem(ReadMem(m_pc+1), m_a);
				m_pc += s_opcodesInfo[opcode].m_numBytes;
				break;
			case STX_ZEROPAGE:
#ifdef UNIT_TESTING
				{
					unsigned arg = ReadMem(m_pc+1);
					temp << "$" << setdataprint(2) << arg << " = " << setdataprint(2) << ReadMem(arg);
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
#ifdef UNIT_TESTING
				std::cout << "";
				numCycles = 0;
#else
				std::cerr << "unknown opcode " << std::showbase << std::hex
					<< opcode << std::endl;
				exit(EXIT_FAILURE);
#endif
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

unsigned NESCPU::BranchOnCondition(bool conditionResult, unsigned opcode)
{
#ifdef UNIT_TESTING
	std::ostringstream temp("");
	temp << "$" << setdataprint(4) << m_pc + ReadMem(m_pc+1) + s_opcodesInfo[opcode].m_numBytes;
	std::cout << temp.str();
#endif
	unsigned executedCycles = 0;
	if(conditionResult)
	{
		//branch taken
		if((m_pc & 0xFF) + ReadMem(m_pc+1) > 0xFF)
		{
			//branch to next page, add 2 cycles to base count
			executedCycles += 2;
		}
		else
		{
			//branch on same page, add 1 cycle to base count
			++executedCycles;
		}
		m_pc += ReadMem(m_pc+1);
	}
	m_pc += s_opcodesInfo[opcode].m_numBytes;
	return executedCycles;
}

std::string NESCPU::DumpRegisters()
{
	static std::ostringstream temp;
	temp.str("");
	temp << "A:" << setdataprint(2) << m_a
		<< " X:" << setdataprint(2) << m_x
		<< " Y:" << setdataprint(2) << m_y
		<< " P:" << setdataprint(2) << m_p.to_ulong()
		<< " SP:" << setdataprint(2) << m_sp;
	return temp.str();
}

void NESCPU::LoadRegister(unsigned& reg, unsigned address, unsigned opcode)
{
	reg = ReadMem(address);
	m_p[NEGATIVE] = reg > 0x7F;
	m_p[ZERO] = reg == 0;
	m_pc += s_opcodesInfo[opcode].m_numBytes;	
}

unsigned NESCPU::ReadMem(unsigned address)
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

unsigned NESCPU::PopByte()
{
	++m_sp;
	return ReadMem(m_sp);
}

unsigned NESCPU::PopWord()
{
	unsigned lowByte = PopByte();
	return (PopByte() << 8) + lowByte;
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

void NESCPU::PushByte(unsigned data)
{
	WriteMem(m_sp, data);
	--m_sp;
}

void NESCPU::PushWord(unsigned data)
{
	//push high byte first, then low byte
	PushByte((data >> 8) & 0xFF);
	PushByte(data & 0xFF);
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

void NESCPU::WriteMem(unsigned address, unsigned data)
{
	if(address < 0x2000)
	{
		//RAM write, wrap around to simulate mirroring
		m_ram[address % RAM_SIZE] = static_cast<uint8_t>(data & 0xFF);
	}	
}
