using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MiniAudioTest : MonoBehaviour {

#if UNITY_EDITOR
    public IntPtr libraryHandle;

    public delegate IntPtr InitializationHandler();

    public InitializationHandler InitializeEngineOnStack;

    public delegate void ReleaseHandler(IntPtr ptr);
    public ReleaseHandler ReleaseAudioEngine;
#endif

#if UNITY_EDITOR_WIN
    [DllImport("kernel32")]
    public static extern IntPtr LoadLibrary(string path);

    [DllImport("kernel32")]
    public static extern IntPtr GetProcAddress(IntPtr libraryHandle, string symbolName);

    [DllImport("kernel32")]
    public static extern bool FreeLibrary(IntPtr libraryHandle);

    public static IntPtr OpenLibrary(string path) {
        IntPtr handle = LoadLibrary(path);

        if (handle == IntPtr.Zero) {
            throw new Exception("Couldn't open native library: " + path);
        }
        return handle;
    }

    public static void CloseLibrary(IntPtr libraryHandle) {
        FreeLibrary(libraryHandle);
    }

    public static T GetDelegate<T>(IntPtr libraryHandle, string functionName) where T : class {
        IntPtr symbol = GetProcAddress(libraryHandle, functionName);

        if (symbol == IntPtr.Zero) {
            throw new Exception($"Could not find function: {functionName}");
        }

        return Marshal.GetDelegateForFunctionPointer(symbol, typeof(T)) as T;
    }

    // const string LIB_PATH = "/Math/Plugins/math-dll.dll";
    const string LIB_PATH = "/MiniAudio/Plugins/libminiaudio_impl.dll";
#endif

    IntPtr audioEngineHandle;

    void OnEnable() {
        libraryHandle = OpenLibrary(Application.dataPath + LIB_PATH);
        InitializeEngineOnStack = GetDelegate<InitializationHandler>(libraryHandle, "init_engine");
        ReleaseAudioEngine = GetDelegate<ReleaseHandler>(libraryHandle, "release_audio_engine");

        audioEngineHandle = InitializeEngineOnStack();
        Debug.Log(audioEngineHandle == IntPtr.Zero);
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.Space)) {
            ReleaseAudioEngine(audioEngineHandle);
        }
    }

    void OnDisable() {
        // ReleaseAudioEngine(audioEngineHandle);
        FreeLibrary(libraryHandle);
    }

}