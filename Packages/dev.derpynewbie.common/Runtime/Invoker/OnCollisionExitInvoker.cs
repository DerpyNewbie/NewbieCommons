using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common.Invoker
{
    [AddComponentMenu("Newbie Commons/Invoker/Collision/On Collision Exit")]
    public class OnCollisionExitInvoker : CommonInvokerBase
    {
        [SerializeField]
        private bool isTriggeringOnLocalOnly = true;

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject == null) return;

            if (isTriggeringOnLocalOnly && !Networking.IsOwner(collision.gameObject))
                return;
            Invoke();
        }
    }
}