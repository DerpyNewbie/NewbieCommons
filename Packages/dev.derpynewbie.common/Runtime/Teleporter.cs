using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common
{
    [RequireComponent(typeof(Collider)), UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Teleporter : UdonSharpBehaviour
    {
        [SerializeField]
        private Transform target;

        [PublicAPI]
        public void Teleport()
        {
            _TeleportLocalPlayer();
        }

        private void _TeleportLocalPlayer()
        {
            Networking.LocalPlayer.TeleportTo(target.position, target.rotation,
                VRC_SceneDescriptor.SpawnOrientation.AlignPlayerWithSpawnPoint);
        }
    }
}