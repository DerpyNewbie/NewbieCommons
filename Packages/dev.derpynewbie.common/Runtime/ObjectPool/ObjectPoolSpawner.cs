using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common.ObjectPool
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ObjectPoolSpawner : UdonSharpBehaviour
    {
        [SerializeField]
        private ObjectPoolProxy targetProxy;

        public override void Interact()
        {
            TryToSpawn();
        }

        [PublicAPI]
        public void TryToSpawn()
        {
            targetProxy.TryToSpawn();
        }
    }
}