using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common.Invoker
{
    [AddComponentMenu("Newbie Commons/Invoker/Collision/On Trigger Exit")]
    public class OnTriggerExitInvoker : CommonInvokerBase
    {
        [SerializeField]
        private bool isTriggeringOnLocalOnly = true;

        private void OnTriggerExit(Collider other)
        {
            // null is possible for VRCPlayerApi colliding with trigger
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (other.gameObject == null) return;

            if (isTriggeringOnLocalOnly && !Networking.IsOwner(other.gameObject))
                return;
            Invoke();
        }
    }
}