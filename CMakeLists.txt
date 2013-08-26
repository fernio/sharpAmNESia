cmake_minimum_required(VERSION 2.8)
project(amNESia)
include_directories(include)
file(GLOB AMNESIA_SOURCES src/*)
add_executable(amNESia ${AMNESIA_SOURCES})

enable_testing()
add_executable(opcodesInfoTest tests/opcodesInfoTest.cpp)
add_test(opcodesInfoTest opcodesInfoTest)
