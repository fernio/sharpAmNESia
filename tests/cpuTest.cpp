#include <iostream>
#include "nescpu.hpp"
#include "rom.hpp"

int main(int argc, char* argv[])
{
	if(argc < 2)
	{
		std::cerr << "no rom specified" << std::endl;
		return -1;
	}
	NesRom rom;
	rom.Load(argv[1]);
	NESCPU cpu;
	cpu.SetRomPtr(rom.GetDataPtr());
	cpu.PowerUp();
	cpu.Execute(100);
	return 0;
}
