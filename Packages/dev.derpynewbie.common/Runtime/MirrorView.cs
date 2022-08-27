using UdonSharp;

namespace DerpyNewbie.Common
{
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