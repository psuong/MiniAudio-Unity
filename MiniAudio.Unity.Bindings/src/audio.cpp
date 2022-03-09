#include "../headers/audio.h"
#include "../miniaudio/miniaudio.h"
#include <cstdlib>
#include <vector>

extern void safe_debug_log(const char *message);
extern void safe_debug_error(const char *message);

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
		delete engine;
		engine = nullptr;
	}
}

uint32_t LoadSound(const char* path, SoundLoadParameters loadParams) {
	return engine->request_sound(path, loadParams);
}

void PlaySound(uint32_t handle) {
	engine->play_sound(handle);
}

void StopSound(uint32_t handle) {
	engine->stop_sound(handle, true);
}

AudioEngine& get_engine() {
	return (AudioEngine &)(*engine);
}

AudioEngine::AudioEngine() {
	if (MA_SUCCESS != ma_engine_init(nullptr, &this->primary_engine)) {
		safe_debug_error("AudioEngine failed to initialize!");
		return;
	}
    this->sounds = std::vector<ma_sound *>();
    this->free_handles = std::vector<uint32_t>();
}

AudioEngine::~AudioEngine() {
	for (uint32_t i = 0; i < this->sounds.size(); i++) {
		ma_sound *sound = sounds[i];

		if (std::count(this->free_handles.begin(), this->free_handles.end(), i) == 0) {
			ma_sound_uninit(sound);
		}

		free(sound);
	}
	ma_engine_uninit(&this->primary_engine);
	safe_debug_log("Successfully released AudioEngine.");
}

size_t AudioEngine::free_sound_count() {
	return this->free_handles.size();
}

// Member AudioEngine implementation
uint32_t AudioEngine::request_sound(const char *path, SoundLoadParameters load_params) {
    uint32_t handle;
	ma_sound* sound;

	// First check if there is a handle that we can use
	if (!this->free_handles.empty()) {
		// Grab the index
		handle = this->free_handles.back();

		// The index is no longer free so remove it.
		this->free_handles.pop_back();

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
			MA_SOUND_FLAG_WAIT_INIT,
			nullptr,
			nullptr,
			sound)) {
		// The sound was not successfully initialized, so we store the handle back as a
		// free handle that we can reuse.
		this->free_handles.push_back(handle);
		return UINT32_MAX;
	}

	ma_sound_set_looping(sound, load_params.IsLooping);
	ma_sound_set_volume(sound, load_params.Volume);
	ma_sound_set_start_time_in_milliseconds(sound, load_params.StartTime);
	if (load_params.EndTime > load_params.StartTime) {
		ma_sound_set_stop_time_in_milliseconds(sound, load_params.EndTime);
	}

	// We've successfully initialized the sound.
	return handle;
}

void AudioEngine::release_sound(uint32_t handle) {
	if (handle < this->sounds.size()) {
		ma_sound* sound = this->sounds[handle];

		if (ma_sound_is_playing(sound)) {
			ma_sound_stop(sound);
		}

		ma_sound_uninit(sound);

		// We can reuse this handle
		this->free_handles.push_back(handle);
	}
}

void AudioEngine::play_sound(uint32_t handle) {
	if (handle < this->sounds.size()) {
		ma_sound* sound = this->sounds[handle];
		ma_sound_start(sound);
	}
}

void AudioEngine::stop_sound(uint32_t handle, bool rewind) {
	if (handle < this->sounds.size()) {
		ma_sound* sound = this->sounds[handle];
		ma_sound_stop(sound);

		if (rewind) {
			ma_sound_seek_to_pcm_frame(sound, 0);
		}
	}
}

bool AudioEngine::is_sound_playing(uint32_t handle) {
	if (handle < this->sounds.size()) {
		ma_sound* sound = this->sounds[handle];
		return ma_sound_is_playing(sound);
	}
	return false;
}
