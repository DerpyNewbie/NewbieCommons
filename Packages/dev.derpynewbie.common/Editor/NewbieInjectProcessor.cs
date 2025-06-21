using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDKBase.Editor.BuildPipeline;
using static DerpyNewbie.Common.Editor.NewbieCommonsEditorUtil;
using Debug = UnityEngine.Debug;

namespace DerpyNewbie.Common.Editor
{
    public static class NewbieInjectProcessor
    {
        public static int ProcessedFieldCount { get; private set; } = 0;
        public static int ComponentUpdateCount { get; private set; } = 0;

        public static void Inject(Scene scene, bool showProgress = true, bool printResult = true)
        {
            var stopwatch = Stopwatch.StartNew();

            ProcessedFieldCount = 0;
            ComponentUpdateCount = 0;

            var injectFields = GetInjectFields();
            var foundComponentsDict = new Dictionary<FieldInfo, List<Component>>();
            var perObjectComponentsDict = new Dictionary<FieldInfo, Dictionary<GameObject, List<Component>>>();

            foreach (var field in injectFields)
            {
                foreach (var component in GetComponentsInScene(scene, field.DeclaringType))
                {
                    var injectOption = field.GetCustomAttribute<NewbieInject>();

                    if (showProgress && EditorUtility.DisplayCancelableProgressBar(
                            "Injecting Reference",
                            $"{injectFields.Count}/{ProcessedFieldCount} Injecting field `{GetFieldName(field)}({injectOption.Scope.ToString()})` for `{GetHierarchyName(component)}`",
                            injectFields.Count / (float)ProcessedFieldCount))
                    {
                        EditorUtility.ClearProgressBar();
                        throw new InvalidOperationException("Operation cancelled by user interruption");
                    }

                    var go = component.gameObject;
                    List<Component> injectingComponents;
                    switch (injectOption.Scope)
                    {
                        default:
                        {
                            injectingComponents = new List<Component>();
                            break;
                        }
                        case SearchScope.Scene:
                        {
                            if (!foundComponentsDict.TryGetValue(field, out injectingComponents))
                            {
                                injectingComponents = GetComponentsInScene(scene, field.FieldType)
                                    .Where(c => !IsEditorOnly(c)).ToList();

                                foundComponentsDict.Add(field, injectingComponents);

                                Log(
                                    $"Found Scene scoped component `{GetFieldName(field)}` at `{injectingComponents.Select(GetHierarchyName).Join()}`");
                            }

                            break;
                        }
                        case SearchScope.Self:
                        {
                            if (!perObjectComponentsDict.TryGetValue(field, out var goSearchCache))
                            {
                                goSearchCache = new Dictionary<GameObject, List<Component>>();
                                perObjectComponentsDict.Add(field, goSearchCache);
                            }

                            if (!goSearchCache.TryGetValue(go, out injectingComponents))
                            {
                                injectingComponents = go
                                    .GetComponents(GetComponentType(field.FieldType))
                                    .Where(c => !IsEditorOnly(c)).ToList();

                                goSearchCache.Add(go, injectingComponents);
                                Log(
                                    $"Found Self scoped component `{GetHierarchyName(go)}:{GetFieldName(field)}` at `{injectingComponents.Select(GetHierarchyName).Join()}`");
                            }

                            break;
                        }
                        case SearchScope.Children:
                        {
                            if (!perObjectComponentsDict.TryGetValue(field, out var goSearchCache))
                            {
                                goSearchCache = new Dictionary<GameObject, List<Component>>();
                                perObjectComponentsDict.Add(field, goSearchCache);
                            }

                            if (!goSearchCache.TryGetValue(go, out injectingComponents))
                            {
                                injectingComponents = component
                                    .GetComponentsInChildren(GetComponentType(field.FieldType), true)
                                    .Where(c => !IsEditorOnly(c)).ToList();

                                goSearchCache.Add(go, injectingComponents);

                                Log(
                                    $"Found Children scoped component `{GetHierarchyName(go)}:{GetFieldName(field)}` at `{injectingComponents.Select(GetHierarchyName).Join()}`");
                            }

                            break;
                        }
                        case SearchScope.Parents:
                        {
                            if (!perObjectComponentsDict.TryGetValue(field, out var goSearchCache))
                            {
                                goSearchCache = new Dictionary<GameObject, List<Component>>();
                                perObjectComponentsDict.Add(field, goSearchCache);
                            }

                            if (!goSearchCache.TryGetValue(go, out injectingComponents))
                            {
                                injectingComponents = component
                                    .GetComponentsInParent(GetComponentType(field.FieldType), true)
                                    .Where(c => !IsEditorOnly(c)).ToList();

                                goSearchCache.Add(go, injectingComponents);
                                Log(
                                    $"Found Parents scoped component `{GetHierarchyName(go)}:{GetFieldName(field)}` at `{injectingComponents.Select(GetHierarchyName).Join()}`");
                            }

                            break;
                        }
                    }

                    var serializedObject = new SerializedObject(component);
                    var serializedProperty = serializedObject.FindProperty(field.Name);
                    if (serializedProperty == null)
                    {
                        Debug.LogWarning(
                            $"Field `{GetFieldName(field)}` is marked with NewbieInject attribute, but is not found in component `{GetHierarchyName(component)}`.");
                        continue;
                    }

                    if (field.FieldType.IsArray)
                    {
                        serializedProperty.ClearArray();
                        serializedProperty.arraySize = injectingComponents.Count;
                        for (var i = 0; i < injectingComponents.Count; i++)
                            serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue = injectingComponents[i];
                    }
                    else
                    {
                        serializedProperty.objectReferenceValue = injectingComponents.FirstOrDefault();
                    }

                    serializedObject.ApplyModifiedProperties();
                    ++ComponentUpdateCount;
                }

                ++ProcessedFieldCount;
            }

            stopwatch.Stop();

            if (showProgress)
                EditorUtility.ClearProgressBar();

            if (printResult)
            {
                var sb = new StringBuilder("Scene Injection Result:");

                sb.Append(
                    $"\n{ProcessedFieldCount} fields affected, {ComponentUpdateCount} component updates in {stopwatch.ElapsedMilliseconds} ms.");

                sb.Append("\n\n====== Scene Search ======");

                foreach (var pair in foundComponentsDict)
                    sb.Append("\n")
                        .Append(GetFieldName(pair.Key)).Append(", ")
                        .Append('{').Append(pair.Value.Select(GetHierarchyName).Join()).Append('}');

                sb.Append("\n\n====== GameObject Search ======");

                foreach (var pair in perObjectComponentsDict)
                {
                    foreach (var goSearch in pair.Value)
                    {
                        sb.Append("\n")
                            .Append(GetFieldName(pair.Key))
                            .Append(':')
                            .Append(pair.Key.GetCustomAttribute<NewbieInject>().Scope.ToString())
                            .Append(", ")
                            .Append('{').Append(GetHierarchyName(goSearch.Key)).Append("}, ")
                            .Append('{').Append(goSearch.Value.Select(GetHierarchyName).Join()).Append('}');
                    }
                }

                Log(sb.ToString());
            }
        }

        public static void Clear(Scene scene)
        {
            var stopwatch = Stopwatch.StartNew();

            ProcessedFieldCount = 0;
            ComponentUpdateCount = 0;

            var injectFields = GetInjectFields();
            foreach (var field in injectFields)
            {
                foreach (var component in GetComponentsInScene(scene, field.DeclaringType))
                {
                    var serializedObject = new SerializedObject(component);
                    var serializedProperty = serializedObject.FindProperty(field.Name);
                    if (serializedProperty == null)
                    {
                        Debug.LogWarning(
                            $"Field `{field.Module.Name}:{field.Name}:{field.FieldType.Name}` is marked with NewbieInject attribute, but is not found in component `{GetHierarchyName(component)}`.");
                        continue;
                    }

                    if (field.FieldType.IsArray)
                    {
                        serializedProperty.ClearArray();
                        serializedProperty.arraySize = 0;
                    }
                    else
                    {
                        serializedProperty.objectReferenceValue = null;
                    }

                    serializedObject.ApplyModifiedProperties();
                    ++ComponentUpdateCount;
                }

                ++ProcessedFieldCount;
            }

            Debug.Log(
                $"{ProcessedFieldCount} fields affected, {ComponentUpdateCount} component updates in {stopwatch.ElapsedMilliseconds} ms.");
        }

        public static List<FieldInfo> GetInjectFields()
        {
            var result = new List<FieldInfo>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in asm.GetTypes())
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                if (field.IsDefined(typeof(NewbieInject), true))
                {
                    if (!field.IsDefined(typeof(SerializeField), true) && field.IsPrivate)
                    {
                        Debug.LogWarning(
                            $"Field `{GetFieldName(field)}` is marked with NewbieInject attribute, but is not marked with SerializeField attribute. This may cause unexpected behaviour.");
                    }

                    if (field.IsDefined(typeof(NonSerializedAttribute), true))
                    {
                        Debug.LogWarning(
                            $"Field `{GetFieldName(field)}` is marked with NewbieInject attribute, but is marked with NonSerialized attribute. This may cause unexpected behaviour.");
                    }

                    result.Add(field);
                }

            return result;
        }

        public static List<Component> GetComponentsInScene(Scene scene, Type type)
        {
            var components = new List<Component>();
            foreach (var o in scene.GetRootGameObjects())
                components.AddRange(o.GetComponentsInChildren(GetComponentType(type), true));
            return components;
        }

        public static Type GetComponentType(Type type)
        {
            if (type.IsArray || type.IsPointer || type.IsByRef || type.IsMarshalByRef) return type.GetElementType();
            return type;
        }

        public static string GetHierarchyName(Component component)
        {
            return component == null ? "null" : GetHierarchyName(component.transform);
        }

        public static string GetHierarchyName(GameObject go)
        {
            return go == null ? "null" : GetHierarchyName(go.transform);
        }

        public static string GetHierarchyName(Transform t)
        {
            StringBuilder sb = new StringBuilder(t.name);
            while (t.parent != null)
            {
                var parent = t.parent;
                sb.Insert(0, "/").Insert(0, parent.name);
                t = parent;
            }

            return sb.ToString();
        }

        public static string GetFieldName(FieldInfo field)
        {
            return field.DeclaringType != null
                ? $"{field.FieldType.Name} {field.DeclaringType.FullName}#{field.Name}"
                : $"{field.FieldType.Name} {field.Name}";
        }

        public static bool IsEditorOnly(Component component)
        {
            return IsEditorOnly(component.gameObject);
        }

        public static bool IsEditorOnly(GameObject go)
        {
            if (go.transform.parent != null)
            {
                return go.CompareTag("EditorOnly") || IsEditorOnly(go.transform.parent.gameObject);
            }

            return go.CompareTag("EditorOnly");
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