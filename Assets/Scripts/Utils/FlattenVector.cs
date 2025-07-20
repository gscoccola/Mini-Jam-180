using System.Collections.Generic;
using UnityEngine;

public static class FlattenVector
{
    public static Vector3 Flatten(this Vector3 vector)
    {
        vector.z = 0f;
        return vector;
    }
}
