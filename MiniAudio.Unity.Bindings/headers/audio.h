//
// Created by Blank on 2/6/2022.
//

#ifndef MINIAUDIO_UNITY_BINDINGS_AUDIO_H
#define MINIAUDIO_UNITY_BINDINGS_AUDIO_H

#ifdef MINIAUDIO_EXPORTS
#define MINIAUDIO_API __declspec(dllexport)
#else
#define MINIAUDIO_API __declspec(dllimport)
#endif

#include "../miniaudio/miniaudio.h"

extern "C" {
    static ma_engine* primary_engine;
    MINIAUDIO_API void initialize_engine_handle();
    MINIAUDIO_API void play_sound(const char* path);
    MINIAUDIO_API void release_engine();
}

#endif //MINIAUDIO_UNITY_BINDINGS_AUDIO_H
