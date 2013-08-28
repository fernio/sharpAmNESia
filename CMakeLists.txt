cmake_minimum_required(VERSION 2.8.4)
project(amNESia)
include_directories(include)
# all files located in the source folder will be part of the amNESia executable
file(GLOB AMNESIA_SOURCES src/*)
add_executable(amNESia ${AMNESIA_SOURCES})

# tell cmake to look for another CMakeLists.txt in the tests subdir
enable_testing()
add_subdirectory(tests)
set_property(DIRECTORY tests PROPERTY COMPILE_DEFINITIONS UNIT_TESTING=1)
