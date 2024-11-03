using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common
{
    [AddComponentMenu("Newbie Commons/Utils/Mirror View")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MirrorView : UdonSharpBehaviour
    {
        public MirrorHandler handler;

        public void OnHighMirrorButton()
        {
            if (!handler.IsLow() || !handler.IsOn())
                handler.ToggleMirror();

            handler.SetLowQuality(false);
        }

        public void OnLowMirrorButton()
        {
            if (handler.IsLow() || !handler.IsOn())
                handler.ToggleMirror();
            handler.SetLowQuality(true);
        }
    }
}