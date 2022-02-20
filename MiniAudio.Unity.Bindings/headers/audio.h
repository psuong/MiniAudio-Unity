#ifndef MINIAUDIO_UNITY_BINDINGS_AUDIO_H
#define MINIAUDIO_UNITY_BINDINGS_AUDIO_H

#ifdef MINIAUDIO_EXPORTS
#define MINIAUDIO_API __declspec(dllexport)
#else
#define MINIAUDIO_API __declspec(dllimport)
#endif

#include "../miniaudio/miniaudio.h"
#include <map>
#include <cstdint>
#include <vector>

/**
 * The AudioEngine will effectively store a sparse set unique paths.
 * You can have multiple sounds associated with the same audio clip.
 */
class AudioEngine {
public:
	AudioEngine();
	~AudioEngine();
	size_t free_sound_count();
	uint32_t request_sound(const char* path);
	void release_sound(uint32_t handle);

	void free_sounds();
private:
	ma_engine primary_engine;
	std::map<uint32_t, std::vector<uint32_t>> sound_handles;
	std::vector<ma_sound *> sounds;
	std::vector<uint32_t> free_handles;
};

extern "C" {

MINIAUDIO_API bool IsEngineInitialized();
MINIAUDIO_API void InitializedEngine();
MINIAUDIO_API void PlaySound(uint32_t handle);
MINIAUDIO_API void ReleaseEngine();

}

AudioEngine& get_engine();


#endif //MINIAUDIO_UNITY_BINDINGS_AUDIO_H
