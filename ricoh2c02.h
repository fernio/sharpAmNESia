#ifndef RICOH2C02_H
#define RICOH2C02_H

#define VRAM_SIZE 16*1024	//16KB
#define SPR_RAM_SIZE 256

///@brief This class is in charge of emulating the system's PPU
class Ricoh2C02
{
public:
private:
	unsigned char m_vram[VRAM_SIZE];
	unsigned char m_sprram[SPR_RAM_SIZE];	///< Sprite RAM, stores sprite attributes
}

#endif	//RICOH2C02_H
