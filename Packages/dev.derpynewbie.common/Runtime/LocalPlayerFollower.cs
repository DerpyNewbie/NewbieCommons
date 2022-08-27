using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LocalPlayerFollower : UdonSharpBehaviour
    {
        [SerializeField]
        private Transform origin;
        [SerializeField]
        private Transform player;
        [SerializeField]
        private Transform hips;
        [SerializeField]
        private Transform head;
        [SerializeField]
        private float slerpTime = 0.2F;
        
        private VRCPlayerApi _api;

        private void Start()
        {
            _api = Networking.LocalPlayer;
        }

        public override void PostLateUpdate()
        {
            if (_api == null || !_api.IsValid()) return;

            var playerPosition = _api.GetPosition();
            var playerRotation = Quaternion.Slerp(player.rotation, _api.GetRotation(), slerpTime * Time.deltaTime);
            player.SetPositionAndRotation(playerPosition, playerRotation);

            var originTrackingData = _api.GetTrackingData(VRCPlayerApi.TrackingDataType.Origin);
            var originTrackingDataRotation = Quaternion.Slerp(origin.rotation, originTrackingData.rotation, slerpTime * Time.deltaTime);
            origin.SetPositionAndRotation(originTrackingData.position, originTrackingDataRotation);

            var headTrackingData = _api.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            head.SetPositionAndRotation(headTrackingData.position, headTrackingData.rotation);

            var hipsPosition = _api.GetBonePosition(HumanBodyBones.Hips);
            var hipsRotation = _api.GetBoneRotation(HumanBodyBones.Hips);
            hips.SetPositionAndRotation(hipsPosition, hipsRotation);
        }
    }
}