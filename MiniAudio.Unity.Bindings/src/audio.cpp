#include "../headers/audio.h"
#include "../miniaudio/miniaudio.h"
#include <cstdlib>
#include <string>
#include <vector>

extern void safe_debug_log(const char *message);
extern void safe_debug_error(const char *message);

static std::hash<const char *> hasher;
static AudioEngine* engine;

void InitializedEngine() {
	if (engine != nullptr) {
		safe_debug_error("You are trying to reinitialize the AudioEngine!");
		return;
	}
	engine = new AudioEngine();
}

bool IsEngineInitialized() {
	return engine != nullptr;
}

void ReleaseEngine() {
	if (engine != nullptr) {
		// engine->free_sounds();
		engine->~AudioEngine();
		free(engine);
		engine = nullptr;
	}
}

void PlaySound(uint32_t handle) {
}

AudioEngine& get_engine() {
	return (AudioEngine &)(*engine);
}

AudioEngine::AudioEngine() {
	if (MA_SUCCESS != ma_engine_init(nullptr, &this->primary_engine)) {
		safe_debug_error("AudioEngine failed to initialize!");
		return;
	}

    this->sound_handles = std::map<uint32_t, std::vector<uint32_t>>();
    this->sounds = std::vector<ma_sound *>();
    this->free_handles = std::vector<uint32_t>();
}

AudioEngine::~AudioEngine() {
	// this->free_sounds();
	ma_engine_uninit(&this->primary_engine);
	safe_debug_log("Successfully released AudioEngine.");
}

size_t AudioEngine::free_sound_count() {
	return this->free_handles.size();
}

// Member AudioEngine implementation
uint32_t AudioEngine::request_sound(const char *path) {
    // size_t key = hasher(path);
    uint32_t handle;
	ma_sound* sound;

	// First check if there is a handle that we can use
	if (!this->free_handles.empty()) {
		auto end = this->free_handles.end();
		// Grab the index
		handle = *end;
		// The index is no longer free so remove it.
		this->free_handles.erase(end);

		// Grab the sound associated with this handle.
		sound = sounds[handle];
	} else {
		// We do not have an available handle to use, so calculate
		// the next index in the sounds that are in use.
		handle = this->sounds.size();

		// We must malloc a new sound and add it into our sounds
		sound = (ma_sound*)malloc(sizeof(ma_sound));
		this->sounds.push_back(sound);
	}

	if (MA_SUCCESS != ma_sound_init_from_file(
			&this->primary_engine,
			path,
			0,
			nullptr,
			nullptr,
			sound)) {
		// The sound was not successfully initialized, so we store the handle back as a
		// free handle that we can reuse.
		this->free_handles.push_back(handle);
		return UINT32_MAX;
	}

	// We've successfully initialized the sound.
	return handle;
}

void AudioEngine::release_sound(uint32_t handle) {
	if (handle < this->sounds.size()) {
		ma_sound* sound = this->sounds[handle];
		ma_sound_uninit(sound);

		// We can reuse this handle
		this->free_handles.push_back(handle);
	}
}

void AudioEngine::free_sounds() {
	for (int i = this->sounds.size() - 1; i >= 0; i--) {
		ma_sound* sound = this->sounds[i];
		if (sound != nullptr) {
			ma_sound_uninit(sound);
			free(sound);
		}
		this->free_handles.push_back(i);
	}
	this->sounds.clear();
}
