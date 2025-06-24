using System;
using UnityEngine.Rendering;

[Serializable]
public class CustomVolumeComponent : VolumeComponent
{
    public ClampedFloatParameter horizontalBlur =
        new ClampedFloatParameter(0.05f, 0, 2f);
    public ClampedFloatParameter verticalBlur =
        new ClampedFloatParameter(0.05f, 0, 2f);
}