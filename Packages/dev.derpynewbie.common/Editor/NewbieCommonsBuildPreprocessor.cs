using DerpyNewbie.Common.Editor.Inspector;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase.Editor.BuildPipeline;

namespace DerpyNewbie.Common.Editor
{
    [InitializeOnLoad]
    public class NewbieCommonsBuildPreprocessor : IVRCSDKBuildRequestedCallback
    {
        static NewbieCommonsBuildPreprocessor()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        private static void PlayModeStateChanged(PlayModeStateChange change)
        {
            var isBuilding = BuildPipeline.isBuildingPlayer ||
                             Object.FindObjectOfType<PipelineSaver>() != null ||
                             change != PlayModeStateChange.ExitingEditMode;
            if (isBuilding)
                return;

            RoleManagerEditor.DoPreBuildCheck();
            NewbieInjectProcessor.DoPrePlayInject(change);
        }

        public int callbackOrder => 2048;

        public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        {
            if (!RoleManagerEditor.DoPreBuildCheck())
            {
                Debug.LogError("[NewbieCommonsBuildPreprocessor] RoleManager pre build check failed");
                return false;
            }

            NewbieInjectProcessor.DoPreBuildInject(requestedBuildType);
            return true;
        }
    }
}