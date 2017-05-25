using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    // よく使う関数作るところです。

    public static bool PointToCircle(Vector2 point, Vector2 p1, float r1)
    {
        var x = (p1.x - point.x) * (p1.x - point.x);
        var y = (p1.y - point.y) * (p1.y - point.y);
        var r = r1 * r1;
        return x + y <= r;
    }

    public static bool SircleToCircle(Vector3 p1, float r1, Vector3 p2, float r2)
    {
        var x = (p2.x - p1.x) * (p2.x - p1.x);
        var y = (p2.y - p1.y) * (p2.y - p1.y);
        var r = (r1 + r2) * (r1 + r2);
        return x + y <= r;
    }

    public static bool PointToSphere(Vector3 point, Vector3 p1, float r1)
    {
        var x = (p1.x - point.x) * (p1.x - point.x);
        var y = (p1.y - point.y) * (p1.y - point.y);
        var z = (p1.z - point.z) * (p1.z - point.z);
        var r = r1 * r1;
        return x + y + z <= r;
    }

    public static bool SphereToSphere(Vector3 p1, float r1, Vector3 p2, float r2)
    {
        var x = (p2.x - p1.x) * (p2.x - p1.x);
        var y = (p2.y - p1.y) * (p2.y - p1.y);
        var z = (p2.z - p1.z) * (p2.z - p1.z);
        var r = (r1 + r2) * (r1 + r2);
        return x + y + z <= r;
    }
}
