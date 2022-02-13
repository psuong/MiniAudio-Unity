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
#include <stdint.h>
#include <map>

extern "C" {
    static ma_engine* primary_engine;
    MINIAUDIO_API void initialize_engine_handle();
    MINIAUDIO_API void play_sound(const char* path);
    MINIAUDIO_API void release_engine();

    MINIAUDIO_API typedef struct SoundClip {
        // Because sound_clips are _aliasing_ the actual ma_sounds, we should
        // not implement destructors for automatic resource management.
        SoundClip();
        SoundClip(uint32_t index, ma_sound* sound_ptr);

        void* sound_alias; // Stores an alias that is managed elsewhere
        const uint32_t handle; // Stores the index of the sound_alias so that we can access it.
    };

    MINIAUDIO_API SoundClip request_sound(const char* path);
    MINIAUDIO_API void release_sound(SoundClip clip);
}

#endif //MINIAUDIO_UNITY_BINDINGS_AUDIO_H
