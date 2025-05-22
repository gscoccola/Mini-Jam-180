using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;


public class VibrationHandler : Singleton<VibrationHandler>
{
    [DllImport("__Internal")]
    private static extern void Vibrate(int ms);


    public void LightVibrate()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            Vibrate(50);
#else
        if (Gamepad.current == null) return;
        Gamepad.current.SetMotorSpeeds(0.123f, 0.234f);
        StartCoroutine(IStopVibrating(0.08f));
#endif

    }

    public void MediumVibrate()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            Vibrate(100);
#else
        if (Gamepad.current == null) return;
        Gamepad.current.SetMotorSpeeds(0.123f, 0.234f);
        StartCoroutine(IStopVibrating(0.15f));
#endif

    }

    public void HeavyVibrate()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            Vibrate(250);
#else
        if (Gamepad.current == null) return;
        Gamepad.current.SetMotorSpeeds(0.123f, 0.234f);
        StartCoroutine(IStopVibrating(0.25f));
#endif
    }

    private IEnumerator IStopVibrating(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Gamepad.current.SetMotorSpeeds(0f, 0f);
    }
}



