#ifndef SYSTEM_H
#define SYSTEM_H

#include "ricoh2a03.h"
#include "rom.h"

class NesSystem
{
public:
	NesSystem();
	bool LoadRom(const char* filename);
	void Run();

private:
	Ricoh2A03 m_cpu;
	NesRom m_rom;	
};

NesSystem::NesSystem()
{
	m_cpu.SetRomPtr(m_rom.GetDataPtr());
}

bool NesSystem::LoadRom(const char* filename)
{
	return m_rom.Load(filename);
}

void NesSystem::Run()
{
	m_cpu.Execute(1000);
	std::cout << "finished running" << std::endl;
}

#endif	//SYSTEM_H
