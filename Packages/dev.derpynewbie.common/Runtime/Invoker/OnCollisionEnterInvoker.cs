using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common.Invoker
{
    [AddComponentMenu("Newbie Commons/Invoker/Collision/On Collision Enter")]
    public class OnCollisionEnterInvoker : CommonInvokerBase
    {
        [SerializeField]
        private bool isTriggeringOnLocalOnly = true;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == null) return;

            if (isTriggeringOnLocalOnly && !Networking.IsOwner(collision.gameObject))
                return;
            Invoke();
        }
    }
}