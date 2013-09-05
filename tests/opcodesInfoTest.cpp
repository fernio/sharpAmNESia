#include <cstdlib>
#include <iostream>
#include <string>
#include "opcodesInfo.hpp"

#define EXPECTED_ENTRIES 256
#define EXPECTED_VALID_ENTRIES 150	//TODO: double check this number

int main()
{
	int numEntries = sizeof(s_opcodesInfo)/sizeof(OpcodeInfo);
	if(numEntries != EXPECTED_ENTRIES)
	{
		std::cerr << "s_opcodesInfo has a wrong number of entries: " << numEntries << std::endl;
		return -1;
	}
	//check that the right number of entries are valid
	int numValidEntries = 0;
	std::string invalidOpcode(INVALID_OPCODE_MNEMONIC);
	for(int i = 0; i < numEntries; ++i)
	{
		if(invalidOpcode != s_opcodesInfo[i].m_mnemonic && s_opcodesInfo[i].m_numCycles > 1)
		{
			++numValidEntries;
		}
	}
	if(numValidEntries != EXPECTED_VALID_ENTRIES)
	{
		std::cerr << "s_opcodesInfo has " << abs(EXPECTED_VALID_ENTRIES - numValidEntries)
				<< " invalid entries" << std::endl;
		return -1;
	}
	return 0;
}
