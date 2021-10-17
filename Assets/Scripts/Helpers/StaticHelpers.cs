using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticHelpers { 
    /// <summary>
    /// Gets the distance between 2 Vector3s as a float
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float Vector3Distance(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt((Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2) + Mathf.Pow(a.z - b.z, 2)));
    }

    /// <summary>
    /// Gets the distance between 2 Vector3s as a float on the XZ plane
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float Vector2DistanceXZ(Vector3 a, Vector3 b) {
        return Mathf.Sqrt((Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.z - b.z, 2)));
    }

    /// <summary>
    /// Gets the distance between 2 floats
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float FloatDistance(float a, float b) {
        return Mathf.Abs(Mathf.Abs(a) - Mathf.Abs(b));
    }
}
