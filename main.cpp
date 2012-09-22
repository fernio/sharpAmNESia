#include <iostream>
#include <string>
#include <vector>
#include "rom.h"

int main(int argc, char* argv[])
{
	std::vector<std::string> commandLine(argv, argv + argc);
	if(commandLine.size() < 2)
	{
		std::cerr << "rom parameter missing" << std::endl;
		return -1;
	}
	return 0;
}
