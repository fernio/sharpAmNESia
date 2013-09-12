#include <fstream>
#include <sstream>
#include "logHelper.hpp"

void Print(std::ostream& stream, int width)
{
	stream << "Number width: " << width << "\n"
			<< setdataprint(width) << 0 << "\n"
			<< setdataprint(width) << 1 << "\n"
			<< setdataprint(width) << 0xC << "\n"
			<< setdataprint(width) << 0x12 << "\n"
			<< setdataprint(width) << 0xAE << "\n"
			<< setdataprint(width) << 0x9A0 << "\n"
			<< setdataprint(width) << 0xF080 << "\n"
			<< "\n";
}

int main(int argc, const char* argv[])
{
	if(argc < 2)
	{
		std::cerr << "No filename specified for logfile" << std::endl;
		return -1;
	}
	std::ofstream logFile(argv[1]);
	if(!logFile)
	{
		std::cerr << "Error opening log file with filename " << argv[1] << std::endl;
		return -1;
	}
	logFile << "Custom Manipulator Tests\n"
			<< "------------------------\n\n";
	Print(logFile, 1);
	Print(logFile, 2);
	Print(logFile, 4);
	
	std::ostringstream stream;
	stream << "Custom stream test:\n"
		<< setdataprint(1) << 0xC << "\n"
		<< setdataprint(1) << 0x3A << "\n"
		<< setdataprint(2) << 0 << "\n"
		<< setdataprint(2) << 0xC << "\n"
		<< setdataprint(2) << 0xF3A << "\n"
		<< setdataprint(4) << 0xC << "\n"
		<< "\n";
	logFile << stream.str();
} 
