using Unity.Mathematics;
using UnityEngine;

namespace ilsFramework.Core
{
    public static class ilsMathUtils
    {
        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            var v = (value - from1) / (to1 - from1);
            return math.lerp(from2, to2, v);
        }

        public static Vector3 Vec3_xy(this Vector2 value, bool reverse = false)
        {
            return reverse ? new Vector3(value.y, value.x, 0) : new Vector3(value.x, value.y, 0);
        }

        public static Vector3 Vec3_xz(this Vector2 value, bool reverse = false)
        {
            return reverse ? new Vector3(value.y, 0, value.x) : new Vector3(value.x, 0, value.y);
        }

        public static Vector3 Vec3_yz(this Vector2 value, bool reverse = false)
        {
            return reverse ? new Vector3(0, value.y, value.x) : new Vector3(0, value.x, value.y);
        }

        public static float X(this Vector2 value)
        {
            return value.x;
        }

        public static float Y(this Vector2 value)
        {
            return value.y;
        }

        public static float X(this Vector3 value)
        {
            return value.x;
        }

        public static float Y(this Vector3 value)
        {
            return value.y;
        }

        public static float Z(this Vector3 value)
        {
            return value.z;
        }

        public static float NormalizeAngle(this float angle)
        {
            var cur =Mathf.Abs( angle % 360);
            if (cur >= 180)
            {
                return 180 - cur;
            }
            else
            {
                return cur;
            }
        }
    }
}