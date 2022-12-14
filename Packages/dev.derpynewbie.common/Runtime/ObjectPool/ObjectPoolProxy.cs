using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace DerpyNewbie.Common.ObjectPool
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)] [RequireComponent(typeof(VRCObjectPool))]
    public class ObjectPoolProxy : UdonSharpBehaviour
    {
        private VRCObjectPool _pool;
        [SerializeField]
        private Transform spawnTarget;

        public bool IsOwner => Networking.IsOwner(gameObject);

        private void Start()
        {
            _pool = GetComponent<VRCObjectPool>();
        }

        [PublicAPI]
        public void TryToSpawn()
        {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(OwnerOnly_Spawn));
        }

        [PublicAPI]
        public void ReturnAll()
        {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(OwnerOnly_ReturnAll));
        }

        public void OwnerOnly_Spawn()
        {
            if (!Networking.IsOwner(gameObject))
                return;

            var obj = _pool.TryToSpawn();
            if (obj == null)
            {
                Debug.LogError("[ObjectPoolProxy] Could not spawn object");
                return;
            }

            if (spawnTarget != null)
                obj.transform.SetPositionAndRotation(spawnTarget.position, spawnTarget.rotation);
        }

        public void OwnerOnly_ReturnAll()
        {
            if (!Networking.IsOwner(gameObject))
                return;

            var objs = _pool.Pool;
            foreach (var o in objs)
            {
                Networking.SetOwner(Networking.LocalPlayer, o);
                if (o != null && o.GetComponent<VRCPickup>())
                    o.GetComponent<VRC_Pickup>().Drop();
                _pool.Return(o);
            }
        }

        public void OwnerOnly_Return(GameObject target)
        {
            if (!Networking.IsOwner(gameObject))
                return;

            _pool.Return(target);
        }
    }
}