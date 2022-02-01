using System;
using System.Runtime.InteropServices;
using UnityEngine;
using MiniAudio.Unity.Logging;

public class BindingTest : MonoBehaviour {

    [DllImport("MiniAudio_Unity_Bindings")]
    static extern int add(int a, int b);

    [DllImport("MiniAudio_Unity_Bindings")]
    static extern void InitializeLogger(IntPtr function);

    DebugLogHandler logHandler;

    // Start is called before the first frame update
    void Start() {
        logHandler = new DebugLogHandler(NativeDebug.Log);
        IntPtr logFunctionPtr = Marshal.GetFunctionPointerForDelegate(logHandler);
        InitializeLogger(logFunctionPtr);
        int result = add(1, 1);
        Debug.Log(result);
    }

    // Update is called once per frame
    void Update() {

    }
}
