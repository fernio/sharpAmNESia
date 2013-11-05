add_executable(logHelperTest logHelperTest.cpp)
add_test(logHelperTestRun logHelperTest ${CMAKE_CURRENT_BINARY_DIR}/logHelperTest.log)
add_test(logHelperTestCheck diff -u ${CMAKE_CURRENT_BINARY_DIR}/logHelperTest.log ${CMAKE_CURRENT_SOURCE_DIR}/logHelperTest.txt)

add_executable(opcodesInfoTest opcodesInfoTest.cpp)
add_test(opcodesInfoTest opcodesInfoTest)

add_executable(cpuTest cpuTest.cpp ${PROJECT_SOURCE_DIR}/src/nescpu.cpp)
add_test(cpuTestRun cpuTest ${PROJECT_SOURCE_DIR}/roms/ ${CMAKE_CURRENT_BINARY_DIR}/cpuTest.log)
add_test(cpuTestCheck diff -uw ${CMAKE_CURRENT_BINARY_DIR}/cpuTest.log ${CMAKE_CURRENT_SOURCE_DIR}/nestest.log)
