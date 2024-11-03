using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace DerpyNewbie.Common.ObjectPool
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ObjectPoolSpawner : UdonSharpBehaviour
    {
        [SerializeField] private ObjectPoolProxy targetProxy;
        [SerializeField] private Transform spawnOverride;

        public override void Interact()
        {
            TryToSpawn();
        }

        [PublicAPI]
        public void TryToSpawn()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(Internal_Networked_SpawnGlobally));
        }

        public void Internal_Networked_SpawnGlobally()
        {
            var obj = targetProxy.OwnerOnly_Spawn();
            if (obj == null) return;

            if (spawnOverride != null)
            {
                if (!Networking.IsOwner(obj)) Networking.SetOwner(Networking.LocalPlayer, obj);

                var objSync = obj.GetComponent<VRCObjectSync>();
                if (objSync != null) objSync.FlagDiscontinuity();

                obj.transform.SetPositionAndRotation(spawnOverride.position, spawnOverride.rotation);
            }
        }
    }
}