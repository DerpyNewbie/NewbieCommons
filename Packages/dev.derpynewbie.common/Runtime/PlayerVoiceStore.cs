using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common
{
    [AddComponentMenu("Newbie Commons/Utils/Player Voice Store")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayerVoiceStore : UdonSharpBehaviour
    {
        [SerializeField, Tooltip("in decibels, default is 15")] [Range(0, 25)]
        private float gain = 15;
        [SerializeField, Tooltip("in meters. default is 25")] [Range(0, 1000)]
        private float distanceFar = 25;
        [SerializeField, Tooltip("in meters. default is 0")] [Range(0, 1000)]
        private float distanceNear = 0;
        [SerializeField, Tooltip("in meters. default is 0.")] [Range(0, 1000)]
        private float volumetricRadius = 0;

        public float Gain => gain;
        public float DistanceFar => distanceFar;
        public float DistanceNear => distanceNear;
        public float VolumetricRadius => volumetricRadius;

        public void SetVoice(VRCPlayerApi api)
        {
            api.SetVoiceGain(Gain);
            api.SetVoiceDistanceFar(DistanceFar);
            api.SetVoiceDistanceNear(DistanceNear);
            api.SetVoiceVolumetricRadius(VolumetricRadius);
        }
    }
}