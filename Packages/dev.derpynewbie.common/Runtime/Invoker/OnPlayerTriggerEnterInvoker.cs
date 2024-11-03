using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common.Invoker
{
    [AddComponentMenu("Newbie Commons/Invoker/On Player Trigger Enter Event")]
    public class OnPlayerTriggerEnterInvoker : CommonInvokerBase
    {
        [SerializeField]
        private bool isTriggeringOnLocalOnly = true;

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (isTriggeringOnLocalOnly && player.isLocal == false)
                return;
            Invoke();
        }
    }
}