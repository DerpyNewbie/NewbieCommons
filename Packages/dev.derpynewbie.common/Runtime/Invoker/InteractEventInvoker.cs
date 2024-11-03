using UnityEngine;

namespace DerpyNewbie.Common.Invoker
{
    [AddComponentMenu("Newbie Commons/Invoker/Interact Event")]
    public class InteractEventInvoker : CommonInvokerBase
    {
        public override void Interact()
        {
            Invoke();
        }
    }
}