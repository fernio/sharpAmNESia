cmake_minimum_required(VERSION 2.8.0)

project(amNESia)

if(CMAKE_VERSION VERSION_LESS "3.1")
	#TODO: enable C++11 features on other platforms
	if(CMAKE_COMPILER_IS_GNUCXX)
		set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")
	endif()
else()
	set(CMAKE_CXX_STANDARD 11)
endif()

include_directories(include)
# all files located in the source folder will be part of the amNESia executable
file(GLOB AMNESIA_SOURCES src/*)
file(GLOB AMNESIA_HEADERS include/*)
add_executable(amNESia ${AMNESIA_SOURCES} ${AMNESIA_HEADERS})
#set_property(TARGET amNESia PROPERTY CXX_STANDARD 11)
#set_property(TARGET amNESia PROPERTY CXX_STANDARD_REQUIRED ON)
if(MSVC)
	# add compiler flag to fix "exception unwind semantics" warning in VS2017
	set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /EHsc")
endif()

# tell cmake to look for another CMakeLists.txt in the tests subdir
enable_testing()
add_subdirectory(tests)
set_property(DIRECTORY tests PROPERTY COMPILE_DEFINITIONS UNIT_TESTING=1)
