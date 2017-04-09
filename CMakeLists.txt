cmake_minimum_required(VERSION 3.1.0)
project(amNESia)

include_directories(include)
# all files located in the source folder will be part of the amNESia executable
file(GLOB AMNESIA_SOURCES src/*)
file(GLOB AMNESIA_HEADERS include/*)
add_executable(amNESia ${AMNESIA_SOURCES} ${AMNESIA_HEADERS})
set_property(TARGET amNESia PROPERTY CXX_STANDARD 11)
set_property(TARGET amNESia PROPERTY CXX_STANDARD_REQUIRED ON)

# tell cmake to look for another CMakeLists.txt in the tests subdir
enable_testing()
add_subdirectory(tests)
set_property(DIRECTORY tests PROPERTY COMPILE_DEFINITIONS UNIT_TESTING=1)
