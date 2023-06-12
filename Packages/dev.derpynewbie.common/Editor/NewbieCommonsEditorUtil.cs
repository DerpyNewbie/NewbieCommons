using System;
using UnityEditor;
using UnityEngine;

namespace DerpyNewbie.Common.Editor
{
    public static class NewbieCommonsEditorUtil
    {
        public static void Log(object message, UnityEngine.Object context = null)
        {
            Debug.Log($"[NewbieCommons] {message}", context);
        }

        public static void LogError(object message, UnityEngine.Object context = null)
        {
            Debug.LogError($"[NewbieCommons] {message}", context);
        }

        public static void HelpBoxWithButton(
            string message, MessageType type,
            string button1Label = null, Action button1Action = null,
            string button2Label = null, Action button2Action = null,
            float buttonWidth = 100F)
        {
            if (button1Label != null)
                EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox(message, type, true);

            if (button2Label != null)
                EditorGUILayout.BeginVertical(GUILayout.Width(buttonWidth));

            if (button1Label != null &&
                GUILayout.Button(button1Label, GUILayout.Width(buttonWidth), GUILayout.ExpandHeight(button2Label == null)))
                button1Action?.Invoke();

            if (button2Label != null && GUILayout.Button(button2Label))
                button2Action?.Invoke();

            if (button2Label != null)
                EditorGUILayout.EndVertical();

            if (button1Label != null)
                EditorGUILayout.EndHorizontal();
        }
    }
}