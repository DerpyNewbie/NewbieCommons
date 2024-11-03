using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common.Invoker
{
    [AddComponentMenu("Newbie Commons/Invoker/Collision/On Player Trigger Exit Event")]
    public class OnPlayerTriggerExitInvoker : CommonInvokerBase
    {
        [SerializeField]
        private bool isTriggeringOnLocalOnly = true;

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (isTriggeringOnLocalOnly && player.isLocal == false)
                return;
            Invoke();
        }
    }
}