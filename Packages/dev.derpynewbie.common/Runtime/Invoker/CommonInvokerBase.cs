using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Enums;
using VRC.Udon.Common.Interfaces;

namespace DerpyNewbie.Common.Invoker
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public abstract class CommonInvokerBase : UdonSharpBehaviour
    {
        [SerializeField]
        protected UdonSharpBehaviour[] listeners;
        [SerializeField]
        protected string eventName = "_Interact";
        [SerializeField]
        protected float delay = 0;
        [SerializeField]
        protected EventTiming timing;
        [SerializeField]
        protected bool isGlobal;
        [SerializeField]
        protected bool isMasterOnly;

        [PublicAPI]
        public void Invoke()
        {
            if ((!Utilities.IsValid(Networking.LocalPlayer)) || (isMasterOnly && !Networking.LocalPlayer.isMaster))
                return;
            
            if (Mathf.Approximately(delay, 0F))
                _Internal_InvokeNow();
            else
                SendCustomEventDelayedSeconds(nameof(_Internal_InvokeNow), delay, timing);
        }

        public void _Internal_InvokeNow()
        {
            if (isGlobal)
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(InternalInvoke));
            else
                InternalInvoke();
        }

        protected virtual void InternalInvoke()
        {
            foreach (var listener in listeners)
            {
                if (!Utilities.IsValid(listener)) continue;
                listener.SendCustomEvent(eventName);
            }
        }
    }
}