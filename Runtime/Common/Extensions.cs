using UnityEngine;

namespace Koala.Simulation.Common
{
    public static class Extensions
    {
        #region Native Types Extensions
        public static Vector3 ToVector3(this float[] arr)
        {
            if (arr == null || arr.Length < 3)
                return Vector3.zero;
            return new Vector3(arr[0], arr[1], arr[2]);
        }

        public static float[] ToFloatArray(this Vector3 v)
        {
            return new float[] { v.x, v.y, v.z };
        }

        public static Quaternion ToQuaternion(this float[] arr)
        {
            if (arr == null || arr.Length < 3)
                return Quaternion.identity;
            return Quaternion.Euler(arr[0], arr[1], arr[2]);
        }

        public static float[] ToFloatArray(this Quaternion q)
        {
            var euler = q.eulerAngles;
            return new float[] { euler.x, euler.y, euler.z };
        }
        #endregion
    }
}