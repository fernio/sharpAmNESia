#include <iostream>
#include "nescpu.hpp"
#include "rom.hpp"

#define CPU_TEST_ROM "nestest.nes"

int main(int argc, char* argv[])
{
	if(argc < 2)
	{
		std::cerr << "no rom path specified\n";
		return -1;
	}
	std::string romFilename(argv[1]);
	romFilename += CPU_TEST_ROM;
	NesRom rom;
	if(!rom.Load(romFilename.c_str()))
	{
		std::cerr << "failed loading rom: " << romFilename << std::endl;
		return -1;
	}
	NESCPU cpu;
	cpu.SetRomPtr(rom.GetDataPtr());
	cpu.PowerUp();
	//execution has to be set to start at 0xC000 according to NesDev wiki
	cpu.m_pc = 0xC000;
	//TODO: find exact number of cycles that test takes
	static int cyclesToExecute = 10000;
	if(cpu.Execute(cyclesToExecute) != cyclesToExecute)
	{
		std::cerr << "unexpected number of cycles executed\n";
		return -1;
	}
	return 0;
}
