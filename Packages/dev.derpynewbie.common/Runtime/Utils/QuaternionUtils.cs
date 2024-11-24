using UnityEngine;

namespace DerpyNewbie.Common
{
    public static class QuaternionUtils
    {
        public static float GetPitch(this Quaternion q)
        {
            return Mathf.Rad2Deg * Mathf.Asin(2 * q.x * q.y + 2 * q.z * q.w);
        }

        public static float GetYaw(this Quaternion q)
        {
            return Mathf.Rad2Deg * Mathf.Atan2(2 * q.y * q.w - 2 * q.x * q.z,
                1 - 2 * q.y * q.y - 2 * q.z * q.z);
        }

        public static float GetRoll(this Quaternion q)
        {
            return Mathf.Rad2Deg * Mathf.Atan2(2 * q.x * q.w - 2 * q.y * q.z,
                1 - 2 * q.x * q.x - 2 * q.z * q.z);
        }
    }
}