cmake_minimum_required(VERSION 3.21)
project(MiniAudio_Unity_Bindings)

add_compile_definitions(DEBUG_EXPORTS)

set(CMAKE_CXX_STANDARD 14)

add_library(MiniAudio_Unity_Bindings SHARED src/debug.cpp)
