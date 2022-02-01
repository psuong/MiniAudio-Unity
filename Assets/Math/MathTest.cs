using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MathTest : MonoBehaviour {

#if UNITY_EDITOR
    public IntPtr libraryHandle;

    public delegate int MathOperationHandle(int a, int b);

    public MathOperationHandle Add;
    public MathOperationHandle Subtract;
    public MathOperationHandle Multiply;
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
        Debug.Log("Closing external library");
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
    const string LIB_PATH = "/MiniAudio/Plugins/MiniAudio.Bindings.Unity.dll";
#endif

    void OnEnable() {
#if UNITY_EDITOR
        libraryHandle = OpenLibrary(Application.dataPath + LIB_PATH);

        Add = GetDelegate<MathOperationHandle>(libraryHandle, "add");
        Subtract = GetDelegate<MathOperationHandle>(libraryHandle, "subtract");
        Multiply = GetDelegate<MathOperationHandle>(libraryHandle, "multiply");
#endif
    }

    void OnDisable() {
#if UNITY_EDITOR
        CloseLibrary(libraryHandle);
        libraryHandle = IntPtr.Zero;
#endif
    }

    // Start is called before the first frame update
    void Start() {
        Debug.Log(Add(1, 1));
        Debug.Log(Subtract(1, 1));
        Debug.Log(Multiply(2, 5));
    }

    // Update is called once per frame
    void Update() {

    }
}
