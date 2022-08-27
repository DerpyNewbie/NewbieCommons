using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class EnvironmentSoundPlayer : UdonSharpBehaviour
    {
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private float delayTime;
        [SerializeField]
        private bool useRandomDelayTime = true;
        [SerializeField]
        private float minRandomTime;
        [SerializeField]
        private float maxRandomTime = 60;

        private void Start()
        {
            if (useRandomDelayTime)
                delayTime = Random.Range(minRandomTime, maxRandomTime);
            audioSource.PlayDelayed(delayTime);
        }
    }
}