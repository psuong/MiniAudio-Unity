//
// Created by Blank on 2/6/2022.
//

#include "../headers/audio.h"
#include "../headers/debug.h"
#include <stdlib.h>
#include <string>

extern log_info_ptr debug_log;
extern log_warn_ptr debug_warn;
extern log_error_ptr debug_error;

void initialize_engine_handle() {
    if (primary_engine != NULL) {
        debug_warn("You are trying to reinitialize the MiniAudio engine which has already been allocated!");
        return;
    }
    ma_result result;
    primary_engine = (ma_engine*)malloc(sizeof(ma_engine));

    result = ma_engine_init(NULL, primary_engine);

    if (result != MA_SUCCESS) {
        debug_error("Failed to initialize the MiniAudio engine!");
        free(primary_engine);
        primary_engine = NULL;
        return;
    }
    debug_log("Successfully initialized the MiniAudio engine.");
}

void release_engine() {
    if (primary_engine != NULL) {
        ma_engine_uninit(primary_engine);
        free(primary_engine);
        primary_engine = nullptr;
        debug_log("Successfully released MiniAudio engine.");
    }
}

void play_sound(const char* path) {
    std::string pth = path;
    std::string message = "Playing path: " + pth;
    debug_log(message.c_str());
    ma_engine_play_sound(primary_engine, path, NULL);
}