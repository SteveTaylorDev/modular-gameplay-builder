using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctions
{
    public static bool IsQuaternionValid(this Quaternion quaternion)
    {
        // Checks if any of the x, y, z or w floats in the chosen quaternion return as NaN.
        bool isNaN = float.IsNaN(quaternion.x + quaternion.y + quaternion.z + quaternion.w);

        // Checks if any of the x, y, z or w floats in the chosen quaternion return as 0.
        bool isZero = quaternion.x == 0 && quaternion.y == 0 && quaternion.z == 0 && quaternion.w == 0;

        return !(isNaN || isZero);
    }
}
