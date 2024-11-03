using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common
{
    [AddComponentMenu("Newbie Commons/Utils/Deactivate After Start")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class DeactivateAfterStart : UdonSharpBehaviour
    {
        private void Start()
        {
            SendCustomEventDelayedSeconds(nameof(Deactivate), 1F);
        }

        public void Deactivate()
        {
            Debug.Log($"[DeactivateAfterStart] deactivating {name}!");
            gameObject.SetActive(false);
            Destroy(this);
        }
    }
}