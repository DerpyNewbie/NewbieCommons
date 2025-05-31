using UdonSharp;

namespace DerpyNewbie.Common.UniUI
{
    public abstract class UniUI : UdonSharpBehaviour
    {
        public override void Interact()
        {
            OnUniUIUpdate();
        }

        public abstract void OnUniUIUpdate();
    }
}