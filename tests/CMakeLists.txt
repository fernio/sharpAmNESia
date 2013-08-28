add_executable(opcodesInfoTest opcodesInfoTest.cpp)
add_test(opcodesInfoTest opcodesInfoTest)

add_executable(cpuTest cpuTest.cpp ${PROJECT_SOURCE_DIR}/src/nescpu.cpp)
add_test(cpuTest cpuTest ${PROJECT_SOURCE_DIR}/roms/nestest.nes)
