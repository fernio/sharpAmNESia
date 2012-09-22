#ifndef ROM_H
#define ROM_H

#include <fstream>
#include <iostream>
#include <string>

#define MAX_PRG_PAGE_SIZE 32*1024
#define HEADER_SIZE 16
#define SIGNATURE_SIZE 4


struct NesRomHeader
{
	char m_signature[SIGNATURE_SIZE];
	unsigned char m_numPrgRomPages;
	unsigned char m_numChrRomPages;
	unsigned char m_flags6;			//name chosen to match documentation used
	unsigned char m_flags7;			//name chosen to match documentation used
	unsigned char m_numPrgRamPages;
	unsigned char m_flags9;			//name chosen to match documentation used
	unsigned char m_flags10;		//name chosen to match documentation used
	char m_unused[5];
};

class NesRom
{
public:
	bool Load(const char* romFilename);
	
	unsigned char m_data[MAX_ROM_SIZE];
};

bool NesRom::Load(const char* romFilename)
{
	std::ifstream romFile(romFilename, std::ifstream::binary);
	if(!romFile)
	{
		std::cerr << "error opening rom, filename: " << romFilename << std::endl;
		return false;
	}
	//read file header into header struct
	NesRomHeader header;
	romFile.read(header.m_signature, HEADER_SIZE);
	if(!romFile.good())
	{
		std::cerr << "error reading rom header" << std::endl;
		return false;
	}
	//check header signature
	static char signature[SIGNATURE_SIZE] = { 'N', 'E', 'S', 0x1A };
	if(memcmp(header.m_signature, signature, SIGNATURE_SIZE) != 0)
	{
		std::string headerSignature(header.m_signature, SIGNATURE_SIZE);
		std::cerr << "wrong signature in header: " << headerSignature << std::endl;
		return false;
	}
	//check for trainer presence
	if(header.m_flags6 & 1<<2)
	{
		char trainer[512];
		romFile.read(trainer, 512);
		if(!romFile.good())
		{
			std::cerr << "error reading trainer" << std::endl;
			return false;
		}
	}
	//read prg rom
	
}

#endif	//ROM_H
