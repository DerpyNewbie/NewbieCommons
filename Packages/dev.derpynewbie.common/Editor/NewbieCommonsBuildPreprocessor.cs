using UnityEditor;
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
            NewbieInjectProcessor.DoPrePlayInject(change);
        }

        public int callbackOrder => 2048;

        public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        {
            NewbieInjectProcessor.DoPreBuildInject(requestedBuildType);
            return true;
        }
    }
}