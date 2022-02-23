using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{

    public static Vector3 Vector3Clamp(Vector3 vector, Vector3 min, Vector3 max)
    {
        return new Vector3(Mathf.Clamp(vector.x, min.x, max.x), Mathf.Clamp(vector.y, min.y, max.y), Mathf.Clamp(vector.z, min.z, max.z));
    }

    public static Vector2 Vector2Clamp(Vector2 vector, Vector2 min, Vector2 max) {
        return new Vector2(Mathf.Clamp(vector.x, min.x, max.x), Mathf.Clamp(vector.y, min.y, max.y));
    }

    public static float Vector2DistanceSquared(Vector3 vector1, Vector3 vector2) {
        // Ignore the Z value when computing Vector3DistanceSquared
        return Vector3DistanceSquared(new Vector3(vector1.x, vector1.y, 0), new Vector3(vector2.x, vector2.y, 0));

    }
    public static float Vector2DistanceSquared(Vector2 vector1, Vector2 vector2) {
        // Turn them into Vector3s with 0 as the Z, and compute Vector3DistanceSquared
        return Vector3DistanceSquared(new Vector3(vector1.x, vector1.y, 0), new Vector3(vector2.x, vector2.y, 0));

    }
    public static float Vector3DistanceSquared(Vector3 vector1, Vector3 vector2) {
        // Honestly this is simple enough it doesn't really need its own function, but its good for readability.
        return (vector1 - vector2).sqrMagnitude;
    }
}