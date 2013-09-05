#ifndef ROM_H
#define ROM_H

#include <fstream>
#include <iostream>
#include <string>
#include <cstring>

#define PRG_ROM_PAGE_SIZE 16*1024
#define HEADER_SIZE 16

struct NesRomHeader
{
	char m_signature[4];
	unsigned char m_numPrgRomPages;
	unsigned char m_numChrRomPages;
	unsigned char m_flags6;			//name chosen to match documentation used
	unsigned char m_flags7;			//name chosen to match documentation used
	unsigned char m_numPrgRamPages;
	unsigned char m_flags9;			//name chosen to match documentation used
	unsigned char m_flags10;		//name chosen to match documentation used
	char m_unused[5];				//header has to be 16 bytes long
};

class NesRom
{
public:
	NesRom();
	const unsigned char* GetDataPtr();
	bool Load(const char* romFilename);

private:	
	char m_data[2*PRG_ROM_PAGE_SIZE];
};

NesRom::NesRom() : m_data()
{
}

const unsigned char* NesRom::GetDataPtr()
{
	return reinterpret_cast<unsigned char*>(m_data);
}

bool NesRom::Load(const char* romFilename)
{
	std::ifstream romFile(romFilename, std::ifstream::binary);
	if(!romFile || !romFile.good())
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
	static char signature[] = { 'N', 'E', 'S', 0x1A };
	if(memcmp(header.m_signature, signature, sizeof(signature)) != 0)
	{
		std::string headerSignature(header.m_signature, sizeof(header.m_signature));
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
	switch(header.m_numPrgRomPages)
	{
		case 1:
			//rom goes into upper half of address space
			romFile.read(m_data+PRG_ROM_PAGE_SIZE, PRG_ROM_PAGE_SIZE);
			break;
		case 2:
			romFile.read(m_data, 2*PRG_ROM_PAGE_SIZE);
			break;
		default:
			std::cerr << "TODO: cannot handle more than 2 PRG ROM pages" << std::endl;
			return false;
	}
	std::cout << "info: read " << static_cast<unsigned>(header.m_numPrgRomPages)
		<< " page" << (header.m_numPrgRomPages > 1 ? "s" : "") << " of PRG ROM\n";
	return true;
}

#endif	//ROM_H
