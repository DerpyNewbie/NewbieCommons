using System;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace DerpyNewbie.Common.Editor
{
    [InitializeOnLoad]
    public static class NewbieInjectMenuItem
    {
        public const string MenuNamespace = "DerpyNewbie/DI/";
        public const string DoInject = MenuNamespace + "Inject NewbieInject fields";
        public const string DoInjectOnBuild = MenuNamespace + "Inject On Build";
        public const string DoInjectOnPlay = MenuNamespace + "Inject On Play";

        static NewbieInjectMenuItem()
        {
            EditorApplication.delayCall += ApplyPreviousSetting;
        }

        private static void ApplyPreviousSetting()
        {
            UpdateMenuCheck();
            EditorApplication.delayCall -= ApplyPreviousSetting;
        }

        private static void UpdateMenuCheck()
        {
            Menu.SetChecked(DoInjectOnBuild, NewbieInjectConfig.InjectOnBuild);
            Menu.SetChecked(DoInjectOnPlay, NewbieInjectConfig.InjectOnPlay);
        }

        [MenuItem(DoInject, priority = 0)]
        private static void InjectMenu()
        {
            try
            {
                NewbieInjectProcessor.Inject(SceneManager.GetActiveScene());
            }
            catch (InvalidOperationException ex)
            {
                UnityEngine.Debug.LogException(ex);
                EditorUtility.DisplayDialog("Inject", "Injection aborted", "OK!");
                return;
            }

            EditorUtility.DisplayDialog(
                "Inject",
                $"Injected reference to {NewbieInjectProcessor.UpdatedComponentCount} objects ({NewbieInjectProcessor.UpdatedFieldCount} fields).",
                "OK!"
            );
        }

        [MenuItem(DoInjectOnBuild, priority = 1)]
        private static void DoInjectOnBuildMenu()
        {
            NewbieInjectConfig.InjectOnBuild = !NewbieInjectConfig.InjectOnBuild;
            UpdateMenuCheck();
        }

        [MenuItem(DoInjectOnPlay, priority = 2)]
        private static void DoInjectOnPlayMenu()
        {
            NewbieInjectConfig.InjectOnPlay = !NewbieInjectConfig.InjectOnPlay;
            UpdateMenuCheck();
        }
    }
}