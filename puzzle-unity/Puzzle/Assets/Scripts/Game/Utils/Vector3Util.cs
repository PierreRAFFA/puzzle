using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class Vector3Util
{
    public Vector3Util()
    {
    }

    public static bool AreVectorEqual(Vector3 v1, Vector3 v2, int tolerance)
    {
        return ((v1.x - v2.x <= tolerance)
            && (v1.y - v2.y <= tolerance));
        //return v1.Equals(v2) && bounds1[1].Equals(bounds2[1]);
    }

}