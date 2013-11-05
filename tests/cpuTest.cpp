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
	if(argc < 3)
	{
		std::cerr << "no log file specified\n";
		return -1;
	}
	//load test rom from specified folder
	std::string romFilename(argv[1]);
	romFilename += CPU_TEST_ROM;
	NesRom rom;
	if(!rom.Load(romFilename.c_str()))
	{
		std::cerr << "failed loading rom: " << romFilename << std::endl;
		return -1;
	}
	//create CPU, load rom into it and set log file
	NESCPU cpu;
	cpu.SetRomPtr(rom.GetDataPtr());
	if(!cpu.SetLogFile(argv[2]))
	{
		std::cerr << "failed opening log file: " << argv[2] << std::endl;
		return -1;
	}
	//first power cpu up, which sets all registers, then modify PC
	cpu.PowerUp();
	//execution has to be set to start at 0xC000 according to NesDev wiki
	cpu.SetPC(0xC000);
	//TODO: find exact number of cycles that test takes
	static int cyclesToExecute = 10000;
	if(cpu.Execute(cyclesToExecute) != cyclesToExecute)
	{
		std::cerr << "unexpected number of cycles executed\n";
		return -1;
	}
	return 0;
}
