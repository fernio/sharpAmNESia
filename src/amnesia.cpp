#include <iostream>
#include <string>
#include <vector>
#include "system.hpp"

int main(int argc, char* argv[])
{
	std::vector<std::string> commandLine(argv, argv + argc);
	if(commandLine.size() < 2)
	{
		std::cerr << "rom parameter missing" << std::endl;
		return EXIT_FAILURE;
	}
	NesSystem nes;
	if(nes.LoadRom(commandLine[1].c_str()))
	{
		nes.Run();
	}
	else
	{
		return EXIT_FAILURE;
	}
	return EXIT_SUCCESS;
}
