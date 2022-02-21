#ifndef MINIAUDIO_UNITY_BINDINGS_AUDIO_H
#define MINIAUDIO_UNITY_BINDINGS_AUDIO_H

#ifdef MINIAUDIO_EXPORTS
#define MINIAUDIO_API __declspec(dllexport)
#else
#define MINIAUDIO_API __declspec(dllimport)
#endif

#include "../miniaudio/miniaudio.h"
#include <cstdint>
#include <vector>

// All extern will follow UpperPascal casing to interop with C#.
// Everything else aside from classes and structs will follow snake_case.
extern "C" {

struct SoundLoadParameters {
	bool IsLooping;
	float Volume;       // Linear volume between 0.0 to 1.0
	uint32_t StartTime;    // Where the sound will start (in ms).
	uint32_t EndTime;      // Where the sound will stop, if <= to the StartTime, the end of the clip will be used (in ms).
};

MINIAUDIO_API bool IsEngineInitialized();
MINIAUDIO_API void InitializedEngine();
MINIAUDIO_API uint32_t LoadSound(const char* path, SoundLoadParameters loadParams);
MINIAUDIO_API bool PlaySound(uint32_t handle);
MINIAUDIO_API bool StopSound(uint32_t handle);
MINIAUDIO_API void ReleaseEngine();
}

/**
 * The AudioEngine will effectively store a sparse set unique paths and sounds.
 */
class AudioEngine {
public:
	AudioEngine();
	~AudioEngine();
	size_t free_sound_count();
	uint32_t request_sound(const char* path, SoundLoadParameters load_params);
	void release_sound(uint32_t handle);
private:
	ma_engine primary_engine{}{};
	std::vector<ma_sound *> sounds;
	std::vector<uint32_t> free_handles;
};


AudioEngine& get_engine();

#endif //MINIAUDIO_UNITY_BINDINGS_AUDIO_H
