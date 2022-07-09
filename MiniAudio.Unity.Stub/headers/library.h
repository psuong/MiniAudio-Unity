#ifndef MINIAUDIO_UNITY_STUB_LIBRARY_H
#define MINIAUDIO_UNITY_STUB_LIBRARY_H

#ifdef STUB_EXPORTS
#define STUB_API __declspec(dllexport)
#else
#define STUB_API __declspec(dllimport)
#endif

#include <cstdint>

extern "C" {

struct SoundLoadParameters {
    bool IsLooping;         // Should the sound loop over, great for music I guess
    float Volume;           // Linear volume between 0.0 to 1.0
    uint32_t StartTime;     // Where the sound will start (in ms).
    uint32_t EndTime;       // Where the sound will stop, if <= to the StartTime, the end of the clip will be used (in ms).
};

typedef void(*void_ptr)();
typedef bool(*is_engine_initialized_ptr)();
typedef uint32_t(*unsafe_load_sound_ptr)(const wchar_t*, uint32_t, void*);
typedef uint32_t(*load_sound_ptr)(const wchar_t*, SoundLoadParameters);
typedef void(*sound_ptr)(uint32_t);
typedef void(*stop_sound_ptr)(uint32_t, bool);
typedef void(*volume_ptr)(uint32_t, float);
typedef bool(*sound_props_ptr)(uint32_t);

STUB_API void InitializeEngineCallback(void_ptr callback);
STUB_API void InitializeEngineCheckCallback(is_engine_initialized_ptr callback);
STUB_API void InitializeUnsafeLoadSoundCallback(unsafe_load_sound_ptr callback);
STUB_API void InitializeLoadSoundCallback(load_sound_ptr callback);
STUB_API void InitializeUnloadSoundCallback(sound_ptr callback);
STUB_API void InitializePlaySoundCallback(sound_ptr callback);
STUB_API void InitializeSetVolumeCallback(volume_ptr callback);
STUB_API void InitializeStopSoundCallback(stop_sound_ptr callback);
STUB_API void InitializeIsSoundPlayingCallback(sound_props_ptr callback);
STUB_API void InitializeIsSoundFinishedCallback(sound_props_ptr callback);
STUB_API void InitializeReleaseEngineCallback(void_ptr callback);

STUB_API void ExecuteInitializeEngineCallback();
STUB_API bool ExecuteIsEngineInitializedCallback();
STUB_API uint32_t ExecuteUnsafeLoadSoundCallback(wchar_t* path, int sizeInBytes, void* loadParams);
STUB_API uint32_t ExecuteLoadSoundCallback(wchar_t* path, SoundLoadParameters loadParams);
STUB_API void ExecuteUnloadSoundCallback(uint32_t handle);
STUB_API void ExecutePlaySoundCallback(uint32_t handle);
STUB_API void ExecuteSetVolumeCallback(uint32_t handle, float volume);
STUB_API void ExecuteStopSoundCallback(uint32_t handle, bool rewind);
STUB_API bool ExecuteIsSoundPlayingCallback(uint32_t handle);
STUB_API bool ExecuteIsSoundFinishedCallback(uint32_t handle);
STUB_API void ExecuteReleaseEngineCallback();

};

#endif //MINIAUDIO_UNITY_STUB_LIBRARY_H
