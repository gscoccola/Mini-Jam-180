using UnityEngine;
using System.Runtime.InteropServices;


public class VibrationHandler : Singleton<VibrationHandler>
{
    [DllImport("__Internal")]
    private static extern void Vibrate(int ms);

    public void LightVibrate()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            Vibrate(50);
#else
            //Debug.Log("Simulate vibration");
#endif
        
    }

    public void MediumVibrate()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            Vibrate(150);
#else
        //Debug.Log("Simulate vibration");
#endif

    }

    public void HeavyVibrate()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            Vibrate(250);
#else
        //Debug.Log("Simulate vibration");
#endif

    }
}



