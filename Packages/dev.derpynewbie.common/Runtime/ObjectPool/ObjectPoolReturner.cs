using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

namespace DerpyNewbie.Common.ObjectPool
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ObjectPoolReturner : UdonSharpBehaviour
    {
        [SerializeField]
        private ObjectPoolProxy[] targetProxies;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision == null || collision.gameObject == null)
                return;

            ReturnObject(collision.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null)
                return;

            ReturnObject(other.gameObject);
        }

        private void ReturnObject(GameObject obj)
        {
            if (obj.GetComponent<VRCPickup>())
                obj.GetComponent<VRCPickup>().Drop();

            foreach (var proxy in targetProxies)
                if (proxy != null && proxy.IsOwner)
                    proxy.OwnerOnly_Return(obj);
        }
    }
}