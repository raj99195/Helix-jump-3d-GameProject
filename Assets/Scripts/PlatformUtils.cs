using UnityEngine;

public static class PlatformUtils
{
    public static AndroidJavaObject Context => new AndroidJavaClass("com.unity3d.player.UnityPlayer")
        .GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext");

    public static void ShowToast(string message)
    {
        var toastClass = new AndroidJavaClass("android.widget.Toast");
        var javaString = new AndroidJavaObject("java.lang.String", message);
        var toast = toastClass.CallStatic<AndroidJavaObject>("makeText", Context, javaString, toastClass.GetStatic<int>("LENGTH_SHORT"));
        toast.Call("show");
    }
}