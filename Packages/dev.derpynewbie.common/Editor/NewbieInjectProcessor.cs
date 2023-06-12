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
using static DerpyNewbie.Common.Editor.NewbieCommonsEditorUtil;

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
                    Log(
                        $"Found requested component `{field.FieldType.FullName}` at `{GetHierarchyName(foundComponent)}`");
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

            if (showProgress)
                EditorUtility.ClearProgressBar();

            if (printResult)
            {
                var sb = new StringBuilder("Injection Result:");

                foreach (var pair in foundComponentsDict)
                    sb.Append("\n").Append(pair.Key.FullName).Append(", ").Append(GetHierarchyName(pair.Value));

                sb.Append(
                    $"\n\n{UpdatedFieldCount} fields affected, {UpdatedComponentCount} components updated in {stopwatch.ElapsedMilliseconds} ms.");

                Log(sb.ToString());
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

            var t = component.transform;
            StringBuilder sb = new StringBuilder(t.name);
            while (t.parent != null)
            {
                var parent = t.parent;
                sb.Insert(0, "/").Insert(0, parent.name);
                t = parent;
            }

            return sb.ToString();
        }

        public static void DoPrePlayInject(PlayModeStateChange change)
        {
            if (!NewbieInjectConfig.InjectOnPlay)
                return;

            Log("Pre-Play injection started.");
            Inject(SceneManager.GetActiveScene());
            Log("Pre-Play injection ended.");
        }

        public static bool DoPreBuildInject(VRCSDKRequestedBuildType requestedBuildType)
        {
            if (!NewbieInjectConfig.InjectOnBuild)
                return true;

            if (requestedBuildType == VRCSDKRequestedBuildType.Avatar)
                return true;

            Log("Pre-Build injection started.");
            Inject(SceneManager.GetActiveScene());
            Log("Pre-Build injection ended.");
            return true;
        }
    }

    public static class NewbieInjectConfig
    {
        private const string Namespace = "NewbieCommons.NewbieInject.";
        private const string OnBuild = Namespace + "OnBuild";
        private const string OnPlay = Namespace + "OnPlay";

        public static bool InjectOnBuild
        {
            get => GetConfigValue(OnBuild);
            set => SetConfigValue(OnBuild, value);
        }

        public static bool InjectOnPlay
        {
            get => GetConfigValue(OnPlay);
            set => SetConfigValue(OnPlay, value);
        }

        private static bool GetConfigValue(string name)
        {
            var v = EditorUserSettings.GetConfigValue(name);
            if (string.IsNullOrWhiteSpace(v) || !bool.TryParse(v, out var result))
                return true;

            return result;
        }

        private static void SetConfigValue(string name, bool value)
        {
            EditorUserSettings.SetConfigValue(name, value.ToString());
        }
    }
}