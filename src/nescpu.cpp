#include <fstream>
#include <iomanip>
#include <iostream>
#include <sstream>
#include "logHelper.hpp"
#include "nescpu.hpp"
#include "opcodes.hpp"
#include "opcodesInfo.hpp"	

#define ROM_BASE_ADDRESS 0x8000
#define STACK_BASE_ADDRESS 0x100
#define PIXELS_PER_CYCLE 3		//only on NTSC
#define PIXELS_PER_SCANLINE 341

#ifdef UNIT_TESTING
	std::ofstream s_logFile;
#endif

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
		s_logFile << setdataprint(4) << m_pc << "  ";
		//bytes read
		static std::ostringstream temp;
		temp.str("");
		for(unsigned i = 0; i < s_opcodesInfo[opcode].m_numBytes; ++i)
		{
			temp << setdataprint(2) << ReadMem(m_pc+i) << " ";
		}
		s_logFile << std::setw(10) << std::setfill(' ') << std::left << temp.str();
		//instruction
		s_logFile << s_opcodesInfo[opcode].m_mnemonic << " ";
		//prepare for outputting instruction argument
		temp.str("");
		s_logFile << std::setw(28) << std::left;
#endif
		bool advancePC = true;
		switch(opcode)
		{
			case AND_IMMEDIATE:
				{
					unsigned arg = ReadMem(m_pc+1);
#ifdef UNIT_TESTING
					temp << "#$" << setdataprint(2) << arg;
					s_logFile << temp.str();
#endif
					m_a &= arg;
					//UpdateStatusRegister(SF_ZERO | SF_NEGATIVE);
					m_p[SF_ZERO] = m_a == 0;
					m_p[SF_NEGATIVE] = IsNegative(m_a);
				}
				break;
			case BCC:
				advancePC = false;
				executedCycles += BranchOnCondition(m_p[SF_CARRY] == 0, opcode);
				break;
			case BCS:
				advancePC = false;
				executedCycles += BranchOnCondition(m_p[SF_CARRY] == 1, opcode);
				break;
			case BEQ:
				advancePC = false;
				executedCycles += BranchOnCondition(m_p[SF_ZERO] == 1, opcode);
				break;
			case BIT_ZEROPAGE:
				{
					unsigned arg = ReadMem(m_pc+1);
#ifdef UNIT_TESTING
					temp << "$" << setdataprint(2) << arg << " = " << setdataprint(2) << ReadMem(arg);
					s_logFile << temp.str();
#endif
					unsigned data = ReadMem(arg);
					m_p[SF_ZERO] = (data & m_a) == 0;
					m_p[SF_NEGATIVE] = IsNegative(data);
					m_p[SF_OVERFLOW] = (data >> 6) & 1;
				}
				break;
			case BMI:
				advancePC = false;
				executedCycles += BranchOnCondition(m_p[SF_NEGATIVE] == 1, opcode);
				break;
			case BNE:
				advancePC = false;
				executedCycles += BranchOnCondition(m_p[SF_ZERO] == 0, opcode);
				break;
			case BPL:
				advancePC = false;
				executedCycles += BranchOnCondition(m_p[SF_NEGATIVE] == 0, opcode);
				break;
			case BVC:
				advancePC = false;
				executedCycles += BranchOnCondition(m_p[SF_OVERFLOW] == 0, opcode);
				break;
			case BVS:
				advancePC = false;
				executedCycles += BranchOnCondition(m_p[SF_OVERFLOW] == 1, opcode);
				break;
			case CLC:
#ifdef UNIT_TESTING
				s_logFile << "";
#endif
				m_p[SF_CARRY] = 0;
				break;
			case CLD:
#ifdef UNIT_TESTING
				s_logFile << "";
#endif
				m_p[SF_DECIMAL_MODE] = false;
				break;
			case CLV:
#ifdef UNIT_TESTING
				s_logFile << "";
#endif
				m_p[SF_OVERFLOW] = false;
				break;
			case CMP_IMMEDIATE:
#ifdef UNIT_TESTING
				temp << "#$" << setdataprint(2) << ReadMem(m_pc+1);
				s_logFile << temp.str();
#endif
				Compare(m_a, ReadMem(m_pc+1));
				break;
			case EOR_IMMEDIATE:
				{
					unsigned arg = ReadMem(m_pc + 1);
#ifdef UNIT_TESTING
					temp << "#$" << setdataprint(2) << arg;
					s_logFile << temp.str();
#endif
					m_a ^= arg;
					m_p[SF_ZERO] = m_a == 0;
					m_p[SF_NEGATIVE] = IsNegative(m_a);
				}
				break;
			case JMP_ABSOLUTE:
				advancePC = false;
				m_pc = ReadMem(m_pc+1) | (ReadMem(m_pc+2) << 8);
#ifdef UNIT_TESTING
				temp << "$" << setdataprint(4) << m_pc;
				s_logFile << temp.str();
#endif
				break;
			case JSR:
				advancePC = false;
				PushWord(m_pc+2);
				m_pc = ReadMem(m_pc+1) | (ReadMem(m_pc+2) << 8);
#ifdef UNIT_TESTING
				temp << "$" << setdataprint(4) << m_pc;
				s_logFile << temp.str();
#endif
				break;
			case LDA_IMMEDIATE:
#ifdef UNIT_TESTING
				temp << "#$" << setdataprint(2) << ReadMem(m_pc+1);
				s_logFile << temp.str();
#endif
				LoadRegister(m_a, m_pc+1, opcode);
				break;
			case LDX_IMMEDIATE:
#ifdef UNIT_TESTING
				temp << "#$" << setdataprint(2) << ReadMem(m_pc+1);
				s_logFile << temp.str();
#endif
				LoadRegister(m_x, m_pc+1, opcode);
				break;
			case NOP:
#ifdef UNIT_TESTING
				s_logFile << " ";
#endif
				break;
			case ORA_IMMEDIATE:
#ifdef UNIT_TESTING
				temp << "#$" << setdataprint(2) << ReadMem(m_pc + 1);
				s_logFile << temp.str();
#endif
				m_a |= ReadMem(m_pc + 1);
				m_p[SF_NEGATIVE] = IsNegative(m_a);
				m_p[SF_ZERO] = m_a == 0;
				break;
			case PHA:
#ifdef UNIT_TESTING
				s_logFile << "";
#endif
				PushByte(m_a);
				break;
			case PHP:
#ifdef UNIT_TESTING
				s_logFile << " ";
#endif
				//according to nesdev wiki, this instruction will set bits 4 and 5 in the stack copy
				PushByte(m_p.to_ulong() | (1<<5) | (1<<4));
				break;
			case PLA:
#ifdef UNIT_TESTING
				s_logFile << " ";
#endif
				m_a = PopByte();
				m_p[SF_NEGATIVE] = IsNegative(m_a);
				m_p[SF_ZERO] = m_a == 0; 
				break;
			case PLP:
#ifdef UNIT_TESTING
				s_logFile << "";
#endif
				//PLP doesn't touch bits 4 and 5
				{
					unsigned oldP = m_p.to_ulong();
					//since bitset doesn't support assignment of all bits, do bitwise operations
					m_p.set();
					m_p &= (PopByte() & 0xCF) | (oldP & 0x30);					
				}
				break;
			case RTS:
				advancePC = false;
#ifdef UNIT_TESTING
				s_logFile << "";
#endif
				m_pc = PopWord();
				++m_pc;
				break;
			case SEC:
#ifdef UNIT_TESTING
				s_logFile << "";
#endif
				m_p[SF_CARRY] = true;
				break;
			case SED:
#ifdef UNIT_TESTING
				s_logFile << "";
#endif
				m_p[SF_DECIMAL_MODE] = true;
				break;
			case SEI:
#ifdef UNIT_TESTING
				s_logFile << "";
#endif
				m_p[SF_INTERRUPT_DISABLE] = true;
				break;
			case STA_ZEROPAGE:
#ifdef UNIT_TESTING
				{
					unsigned arg = ReadMem(m_pc+1);
					temp << "$" << setdataprint(2) << arg << " = " << setdataprint(2) << ReadMem(arg);
					s_logFile << temp.str();
				}
#endif
				WriteMem(ReadMem(m_pc+1), m_a);
				break;
			case STX_ZEROPAGE:
#ifdef UNIT_TESTING
				{
					unsigned arg = ReadMem(m_pc+1);
					temp << "$" << setdataprint(2) << arg << " = " << setdataprint(2) << ReadMem(arg);
					s_logFile << temp.str();
				}
#endif
				WriteMem(ReadMem(m_pc+1), m_x);
				break;
			// case SEI:
				// m_p[SF_INTERRUPT_DISABLE] = 1;
				// break;
			default:
#ifdef UNIT_TESTING
				s_logFile << "";
				numCycles = 0;
#else
				std::cerr << "unknown opcode " << std::showbase << std::hex
					<< opcode << std::endl;
				exit(EXIT_FAILURE);
#endif
		}
		if (advancePC)
		{
			m_pc += s_opcodesInfo[opcode].m_numBytes;
		}
		executedCycles += s_opcodesInfo[opcode].m_numCycles;
#ifdef UNIT_TESTING		
		s_logFile << regs << " CYC:"
				<< std::dec << std::setw(3) << std::right << (startingCycles*PIXELS_PER_CYCLE)%PIXELS_PER_SCANLINE
				<< " SL:" << startingCycles*PIXELS_PER_CYCLE/PIXELS_PER_SCANLINE+241
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
	s_logFile << temp.str();
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

void NESCPU::Compare(unsigned reg, unsigned mem)
{
	//flags updated as described in http://www.6502.org/tutorials/compare_beyond.html
	m_p[SF_ZERO] = reg == mem;
	m_p[SF_CARRY] = reg >= mem;
	m_p[SF_NEGATIVE] = IsNegative(reg-mem);
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

bool NESCPU::IsNegative(unsigned value)
{
	//From http://www.6502.org/tutorials/compare_beyond.html
	//"The N flag contains most significant bit of the of the subtraction result."
	return ((value >> 7) & 1) != 0;
}

void NESCPU::LoadRegister(unsigned& reg, unsigned address, unsigned opcode)
{
	reg = ReadMem(address);
	m_p[SF_NEGATIVE] = IsNegative(reg);
	m_p[SF_ZERO] = reg == 0;
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

bool NESCPU::SetLogFile(const char* logFilename)
{
#ifdef UNIT_TESTING
	s_logFile.open(logFilename, std::ios::out | std::ios::trunc);
	return s_logFile.is_open() && s_logFile.good();
#else
	return false;
#endif
}

unsigned NESCPU::PopByte()
{
	++m_sp;
	return ReadMem(STACK_BASE_ADDRESS+m_sp);
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
	WriteMem(STACK_BASE_ADDRESS+m_sp, data);
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
	m_p[SF_INTERRUPT_DISABLE] = 1;
	m_pc = ReadMem(0xFFFC) + (ReadMem(0xFFFD)<<8);
	//as seen on NesDev Wiki
	m_sp -= 3;
}

void NESCPU::SetRomPtr(const uint8_t* rom)
{
	m_rom = rom;
}

void NESCPU::UpdateStatusRegister(uint8_t mask)
{
	m_p[SF_ZERO] = mask & SF_ZERO ? m_a == 0 : m_p[SF_ZERO];
	m_p[SF_NEGATIVE] = mask & SF_NEGATIVE ? IsNegative(m_a) : m_p[SF_ZERO];
	m_p[SF_OVERFLOW] = mask & SF_OVERFLOW ? ((m_a >> 6) & 1) != 0 : m_p[SF_OVERFLOW];
}

void NESCPU::WriteMem(unsigned address, unsigned data)
{
	if(address < 0x2000)
	{
		//RAM write, wrap around to simulate mirroring
		m_ram[address % RAM_SIZE] = static_cast<uint8_t>(data & 0xFF);
	}	
}
