using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{
    public static class VectorExtensions
    {
        public static float AngleDir(this Vector3 A, Vector3 B)
        {
            return -A.x * B.y + A.y * B.x;
        }
    }
}