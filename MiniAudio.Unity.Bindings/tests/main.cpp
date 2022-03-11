#define DOCTEST_CONFIG_IMPLEMENT_WITH_MAIN
#include "doctest.h"
#include <locale>
#include <codecvt>
#include "../headers/audio.h"

const char* path = "D:\\Music\\Stronghold.mp3";
TEST_CASE("Initializing the audio engine using an ASCII path.") {
	InitializeEngine();
	CHECK(IsEngineInitialized());

	AudioEngine& engine = get_engine();

	std::wstring t = std::wstring_convert<std::codecvt_utf8<wchar_t>, wchar_t>().from_bytes("D:/Music/¡/Stronghold.mp3");

	SoundLoadParameters default_params = SoundLoadParameters();
	uint32_t valid_handle = engine.request_sound(path, default_params);
	CHECK(valid_handle == 0);
	CHECK(engine.free_sound_count() == 0);

	uint32_t invalid_handle = engine.request_sound("aksjdlask", default_params);
	CHECK(invalid_handle == UINT32_MAX);
	CHECK(engine.free_sound_count() == 1);

	valid_handle = engine.request_sound(path, default_params);
	CHECK(valid_handle == 1);
	CHECK(engine.free_sound_count() == 0);

	engine.release_sound(valid_handle);
	CHECK(engine.free_sound_count() == 1);

	default_params.IsLooping = true;
	default_params.Volume = 0.5f;

	valid_handle = engine.request_sound(path, default_params);
	CHECK(engine.free_sound_count() == 0);

	ReleaseEngine();
	CHECK(IsEngineInitialized() == false);
}

const char* u_path = "D:/Music/¡/Stronghold.mp3";
TEST_CASE("Loading audio from a unicode path") {
	InitializeEngine();
	CHECK(IsEngineInitialized() == true);

	SoundLoadParameters default_params = SoundLoadParameters();
	uint32_t valid_handle = LoadSound(u_path, default_params);
	CHECK(valid_handle != UINT32_MAX);

	ReleaseEngine();
	CHECK(IsEngineInitialized() == false);

	sizeof(char);
}
