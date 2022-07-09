#include "../headers/library.h"

static void_ptr initialize_engine;
static is_engine_initialized_ptr is_engine_initialized;
static unsafe_load_sound_ptr unsafe_load_sound;
static load_sound_ptr load_sound;
static sound_ptr unload_sound;
static sound_ptr play_sound;
static stop_sound_ptr stop_sound;
static volume_ptr set_volume;
static sound_props_ptr is_sound_playing;
static sound_props_ptr is_sound_finished;
static void_ptr release_engine;

void ReleaseAllCallbacks() {
    initialize_engine = nullptr;
    is_engine_initialized = nullptr;
    unsafe_load_sound = nullptr;
    load_sound = nullptr;
    unload_sound = nullptr;
    play_sound = nullptr;
    stop_sound = nullptr;
    set_volume = nullptr;
    is_sound_playing = nullptr;
    is_sound_finished = nullptr;
    release_engine = nullptr;
}

void InitializeEngineCallback(void_ptr callback) {
    if (initialize_engine == nullptr) {
        initialize_engine = callback;
    }
}

void InitializeEngineCheckCallback(is_engine_initialized_ptr callback) {
    if (is_engine_initialized == nullptr) {
        is_engine_initialized = callback;
    }
}

void InitializeUnsafeLoadSoundCallback(unsafe_load_sound_ptr callback) {
    if (unsafe_load_sound == nullptr) {
        unsafe_load_sound = callback;
    }
}

void InitializeUnloadSoundCallback(sound_ptr callback) {
    if (unload_sound == nullptr) {
        unload_sound = callback;
    }
}

void InitializePlaySoundCallback(sound_ptr callback) {
    if (play_sound == nullptr) {
        play_sound = callback;
    }
}

void InitializeLoadSoundCallback(load_sound_ptr callback) {
    if (load_sound == nullptr) {
        load_sound = callback;
    }
}

void InitializeSetVolumeCallback(volume_ptr callback) {
    if (set_volume == nullptr) {
        set_volume = callback;
    }
}

void InitializeStopSoundCallback(stop_sound_ptr callback) {
    if (stop_sound == nullptr) {
        stop_sound = callback;
    }
}

void InitializeIsSoundPlayingCallback(sound_props_ptr callback) {
    if (is_sound_playing == nullptr) {
        is_sound_playing = callback;
    }
}

void InitializeIsSoundFinishedCallback(sound_props_ptr callback) {
    if (is_sound_finished == nullptr) {
        is_sound_finished = callback;
    }
}

void InitializeReleaseEngineCallback(void_ptr callback) {
    if (release_engine == nullptr) {
        release_engine = callback;
    }
}

void ExecuteInitializeEngineCallback() {
    initialize_engine();
}

bool ExecuteIsEngineInitializedCallback() {
    return is_engine_initialized();
}

uint32_t ExecuteUnsafeLoadSoundCallback(wchar_t *path, int sizeInBytes, void *loadParams) {
    return unsafe_load_sound(path, sizeInBytes, loadParams);
}

uint32_t ExecuteLoadSoundCallback(wchar_t* path, SoundLoadParameters loadParams) {
    return load_sound(path, loadParams);
}

void ExecuteUnloadSoundCallback(uint32_t handle) {
    unload_sound(handle);
}

void ExecutePlaySoundCallback(uint32_t handle) {
    play_sound(handle);
}

void ExecuteSetVolumeCallback(uint32_t handle, float volume) {
    set_volume(handle, volume);
}

void ExecuteStopSoundCallback(uint32_t handle, bool rewind) {
    stop_sound(handle, rewind);
}

bool ExecuteIsSoundPlayingCallback(uint32_t handle) {
    return is_sound_playing(handle);
}

bool ExecuteIsSoundFinishedCallback(uint32_t handle) {
    return is_sound_finished(handle);
}

void ExecuteReleaseEngineCallback() {
    release_engine();
}