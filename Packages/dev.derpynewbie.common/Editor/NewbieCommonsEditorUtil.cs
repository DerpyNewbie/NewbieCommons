using UnityEngine;

namespace DerpyNewbie.Common.Editor
{
    public static class NewbieCommonsEditorUtil
    {
        public static void Log(object message)
        {
            Debug.Log($"[NewbieCommons] {message}");
        }

        public static void LogError(object message)
        {
            Debug.LogError($"[NewbieCommons] {message}");
        }
    }
}