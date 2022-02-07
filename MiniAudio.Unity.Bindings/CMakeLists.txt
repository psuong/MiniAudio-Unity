cmake_minimum_required(VERSION 3.21)
project(MiniAudio_Unity_Bindings)

add_compile_definitions(DEBUG_EXPORTS)
add_compile_definitions(MINIAUDIO_EXPORTS)

set(CMAKE_CXX_STANDARD 14)

add_library(MiniAudio_Unity_Bindings SHARED src/debug.cpp headers/audio.h src/audio.cpp src/miniaudio.c)
