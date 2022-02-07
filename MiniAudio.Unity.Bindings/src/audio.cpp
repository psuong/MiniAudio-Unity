//
// Created by Blank on 2/6/2022.
//

#include "../headers/audio.h"
#include "../headers/debug.h"
#include <stdlib.h>

void initialize_engine_handle() {
    if (primary_engine != nullptr) {
        debug_warn("You are trying to reinitialize the MiniAudio engine which has already been allocated!");
        return;
    }
    ma_result result;
    primary_engine = (ma_engine*)malloc(sizeof(ma_engine));

    result = ma_engine_init(NULL, primary_engine);

    if (result != MA_SUCCESS) {
        debug_error("Failed to initialize the MiniAudio engine!");
        free(primary_engine);
        primary_engine = nullptr;
        return;
    }
    debug_log("Successfully initialized the MiniAudio engine.");
}

void release_engine() {
    if (primary_engine != nullptr) {
        free(primary_engine);
        primary_engine = nullptr;
    }
}

void play_sound(const char* path) {
    ma_engine_play_sound(primary_engine, path, NULL);
}