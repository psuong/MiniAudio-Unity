#define DOCTEST_CONFIG_IMPLEMENT_WITH_MAIN
#include "doctest.h"
#include "../headers/audio.h"

TEST_CASE("Requesting a sound clip") {
    initialize_engine_handle();
    SoundClip sound_clip = request_sound("D:\\Music\\Stronghold.mp3");

    CHECK(sound_clip.handle == 0);
    CHECK(sound_clip.sound_alias != NULL);

    release_engine();
}