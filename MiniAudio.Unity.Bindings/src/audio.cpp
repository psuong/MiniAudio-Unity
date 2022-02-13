//
// Created by Blank on 2/6/2022.
//

#include "../headers/audio.h"
#include "../miniaudio/miniaudio.h"
#include <stdlib.h>
#include <string>
#include <vector>

extern void safe_debug_log(const char* message);
extern void safe_debug_error(const char* message);

// TODO: Create a struct wrapper which points to the index of the ma_sound.
// The struct should be tracked in C# so that we can play, stop, pause, set the volume etc.
std::vector<SoundClip> sounds = std::vector<SoundClip>();
std::vector<uint32_t> free_indices = std::vector<uint32_t>();

void initialize_engine_handle() {
    if (primary_engine != NULL) {
        safe_debug_log("You are trying to reinitialize the MiniAudio engine which has already been allocated!");
        return;
    }

    ma_result result;
    primary_engine = (ma_engine*)malloc(sizeof(ma_engine));

    result = ma_engine_init(NULL, primary_engine);

    if (result != MA_SUCCESS) {
        safe_debug_error("Failed to initialize the MiniAudio engine!");
        free(primary_engine);
        primary_engine = NULL;
        return;
    }
}

void release_engine() {
    if (primary_engine != NULL) {
        for (size_t i = 0; i < sounds.size(); i++) {
            SoundClip sound = sounds[i];
            sound.~SoundClip();
        }

        sounds.clear();
        free_indices.clear();

        ma_engine_uninit(primary_engine);
        free(primary_engine);
        primary_engine = NULL;
        safe_debug_log("Successfully released MiniAudio engine.");
    }
}

void play_sound(const char* path) {
    std::string pth = path;
    std::string message = "Playing path: " + pth;
    safe_debug_log(message.c_str());
    ma_engine_play_sound(primary_engine, path, NULL);
}

SoundClip request_sound(const char* path) {
    ma_result result;
    ma_sound* sound = (ma_sound*)malloc(sizeof(ma_sound));

    result = ma_sound_init_from_file(primary_engine, path, 0, NULL, NULL, sound);
    if (result != MA_SUCCESS) {
        free(sound);
        return SoundClip();
    }

    if (free_indices.size() > 0) {
        uint32_t free_index = *free_indices.end();
        // Clean up from back to front.
        free_indices.erase(free_indices.end(), free_indices.end());

        // Reuse the same index and alias a new sound to it.
        SoundClip sound_clip = sounds[free_index];
        sound_clip.sound_alias = sound;
        return sound_clip;
    } else {
        // Push a new sound.
        uint32_t handle = sounds.size();
        SoundClip sound_clip = SoundClip(handle, sound);
        sounds.push_back(sound_clip);
        return sound_clip;
    }
}

void release_sound(SoundClip clip) {
    uint32_t index = clip.handle;
    free_indices.push_back(index);

    SoundClip sound = sounds[index];
    sound.~SoundClip();
}

SoundClip::SoundClip() : handle((uint32_t)(~0)) {
    sound_alias = NULL;
}

SoundClip::SoundClip(uint32_t index, ma_sound* sound_ptr) : handle(index) {
    sound_alias = sound_ptr;
}
