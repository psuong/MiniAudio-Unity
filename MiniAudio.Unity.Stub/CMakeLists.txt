cmake_minimum_required(VERSION 3.23)
project(MiniAudio_Unity_Stub)

add_compile_definitions(STUB_EXPORTS)

set(CMAKE_CXX_STANDARD 14)

add_library(MiniAudio_Unity_Stub SHARED src/library.cpp)

install(TARGETS ${PROJECT_NAME} DESTINATION ${CMAKE_CURRENT_SOURCE_DIR}/../Assets/MiniAudio.Unity/Plugins)