using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common.Invoker
{
    [AddComponentMenu("Newbie Commons/Invoker/Collision/On Player Collision Exit Event")]
    public class OnPlayerCollisionExitInvoker : CommonInvokerBase
    {
        [SerializeField]
        private bool isTriggeringOnLocalOnly = true;

        public override void OnPlayerCollisionExit(VRCPlayerApi player)
        {
            if (isTriggeringOnLocalOnly && player.isLocal == false)
                return;
            Invoke();
        }
    }
}