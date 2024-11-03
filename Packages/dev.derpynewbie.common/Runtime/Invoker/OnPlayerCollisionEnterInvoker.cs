using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common.Invoker
{
    [AddComponentMenu("Newbie Commons/Invoker/Collision/On Player Collision Enter Event")]
    public class OnPlayerCollisionEnterInvoker : CommonInvokerBase
    {
        [SerializeField]
        private bool isTriggeringOnLocalOnly = true;

        public override void OnPlayerCollisionEnter(VRCPlayerApi player)
        {
            if (isTriggeringOnLocalOnly && player.isLocal == false)
                return;
            Invoke();
        }
    }
}