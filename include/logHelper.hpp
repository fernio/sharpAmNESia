#ifndef LOGHELPER_HPP
#define LOGHELPER_HPP

#include <iostream>

//custom stream manipulator, inspired by GCC's iomanip header
struct _Setdataprint
{
	int m_width;
};

inline _Setdataprint setdataprint(int width)
{
	return { width };
}

inline std::ostream& operator<<(std::ostream& os, _Setdataprint __f)
{
	//configure stream for printing bytes
	os.flags(std::ios::right | std::ios::hex | std::ios::uppercase);
	os.width(__f.m_width);
	os.fill('0');
	return os;
}

#endif	//LOGHELPER_HPP
