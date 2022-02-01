#ifndef MINIAUDIO_UNITY_BINDINGS_DEBUG_H
#define MINIAUDIO_UNITY_BINDINGS_DEBUG_H

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
#ifdef DEBUG_EXPORTS
#define DEBUG_API __declspec(dllexport)
#else
#define DEBUG_API __declspec(dllimport)
#endif

extern "C" {
    DEBUG_API typedef void(*debug_function_ptr)(const char* message); debug_function_ptr debug_log;

    // Hook this in with Debug.Log from Unity.
    DEBUG_API void InitializeLogger(debug_function_ptr function_ptr);

    // Another hook for Unity.
    DEBUG_API int add(int a, int b);
};

#endif //MINIAUDIO_UNITY_BINDINGS_DEBUG_H
