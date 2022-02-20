#define DOCTEST_CONFIG_IMPLEMENT_WITH_MAIN
#include "doctest.h"
#include "../headers/audio.h"

const char* path = "D:\\Music\\Stronghold.mp3";
TEST_CASE("Initializing the audio engine.") {
	InitializedEngine();
	CHECK(IsEngineInitialized());

	AudioEngine& engine = get_engine();

	uint32_t valid_handle = engine.request_sound(path);
	CHECK(valid_handle == 0);
	CHECK(engine.free_sound_count() == 0);

	uint32_t invalid_handle = engine.request_sound("aksjdlask");
	CHECK(invalid_handle == UINT32_MAX);
	CHECK(engine.free_sound_count() == 1);

	valid_handle = engine.request_sound(path);
	CHECK(valid_handle == 1);
	CHECK(engine.free_sound_count() == 0);

	// engine.free_sounds();
	ReleaseEngine();
	CHECK(IsEngineInitialized() == false);
}
