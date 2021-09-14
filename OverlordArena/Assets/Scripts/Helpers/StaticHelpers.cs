using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticHelpers { 

    public static float Vector3Distance(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt((Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2) + Mathf.Pow(a.z - b.z, 2)));
    }
}
