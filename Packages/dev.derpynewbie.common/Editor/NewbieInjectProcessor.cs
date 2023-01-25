using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDKBase.Editor.BuildPipeline;

namespace DerpyNewbie.Common.Editor
{
    public static class NewbieInjectProcessor
    {
        public static int UpdatedFieldCount { get; private set; } = 0;
        public static int UpdatedComponentCount { get; private set; } = 0;

        public static void Inject(Scene scene, bool showProgress = true, bool printResult = true)
        {
            var stopwatch = Stopwatch.StartNew();

            UpdatedFieldCount = 0;
            UpdatedComponentCount = 0;

            var injectFields = GetInjectFields();
            var foundComponentsDict = new Dictionary<Type, Component>();

            foreach (var field in injectFields)
            {
                if (!foundComponentsDict.ContainsKey(field.FieldType))
                {
                    var foundComponent = GetComponentsInScene(scene, field.FieldType).FirstOrDefault();
                    foundComponentsDict.Add(field.FieldType, foundComponent);
                    UnityEngine.Debug.Log(
                        $"[NewbieCommons] Found requested component `{field.DeclaringType?.FullName}` at `{GetHierarchyName(foundComponent)}`");
                }

                foreach (var component in GetComponentsInScene(scene, field.DeclaringType))
                {
                    if (showProgress && EditorUtility.DisplayCancelableProgressBar(
                            "Injecting Reference",
                            $"{injectFields.Count}/{UpdatedFieldCount} Injecting field `{field.Module.Name}:{field.Name}:{field.FieldType.Name}` for `{GetHierarchyName(component)}`",
                            injectFields.Count / (float)UpdatedFieldCount))
                    {
                        EditorUtility.ClearProgressBar();
                        throw new InvalidOperationException("Operation cancelled by user interruption");
                    }

                    var serializedObject = new SerializedObject(component);
                    var serializedProperty = serializedObject.FindProperty(field.Name);
                    serializedProperty.objectReferenceValue = foundComponentsDict[field.FieldType];
                    serializedObject.ApplyModifiedProperties();
                    ++UpdatedComponentCount;
                }

                ++UpdatedFieldCount;
            }

            stopwatch.Stop();

            if (printResult)
            {
                var sb = new StringBuilder("[NewbieCommons] Inject Result:");

                foreach (var pair in foundComponentsDict)
                    sb.Append("\n").Append(pair.Key.FullName).Append(", ").Append(GetHierarchyName(pair.Value));

                sb.Append(
                    $"\n\n{UpdatedFieldCount} fields affected, {UpdatedComponentCount} components updated in {stopwatch.ElapsedMilliseconds} ms.");

                UnityEngine.Debug.Log(sb.ToString());
            }
        }

        public static List<FieldInfo> GetInjectFields()
        {
            var result = new List<FieldInfo>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in asm.GetTypes())
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                if (field.IsDefined(typeof(NewbieInject), true))
                    result.Add(field);

            return result;
        }

        public static List<Component> GetComponentsInScene(Scene scene, Type type)
        {
            var components = new List<Component>();
            foreach (var o in scene.GetRootGameObjects())
                components.AddRange(o.GetComponentsInChildren(type));
            return components;
        }

        public static string GetHierarchyName(Component component)
        {
            if (component == null)
                return "null";

            StringBuilder sb = new StringBuilder(component.transform.name);
            var t = component.transform.parent;
            while (t.parent != null)
            {
                sb.Insert(0, "/").Insert(0, t.name);
                t = t.parent;
            }

            return sb.ToString();
        }

        [MenuItem("DerpyNewbie/Inject `NewbieInject` fields")]
        public static void InjectMenu()
        {
            try
            {
                Inject(SceneManager.GetActiveScene());
            }
            catch (InvalidOperationException ex)
            {
                UnityEngine.Debug.LogException(ex);
                EditorUtility.DisplayDialog("Inject", "Injection aborted", "OK!");
                return;
            }

            EditorUtility.DisplayDialog(
                "Inject",
                $"Injected reference to {UpdatedComponentCount} objects ({UpdatedFieldCount} fields).",
                "OK!"
            );
        }
    }

    public class NewbieCommonsBuildInject : IVRCSDKBuildRequestedCallback
    {
        public int callbackOrder => 0;

        public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        {
            if (requestedBuildType == VRCSDKRequestedBuildType.Avatar)
            {
                UnityEngine.Debug.LogError("[NewbieCommons] Build Injection not supported with Avatar Build.");
                return true;
            }

            UnityEngine.Debug.Log("[NewbieCommons] Pre-Build injection started.");
            NewbieInjectProcessor.Inject(SceneManager.GetActiveScene());
            UnityEngine.Debug.Log("[NewbieCommons] Pre-Build injection ended.");
            return true;
        }
    }
}