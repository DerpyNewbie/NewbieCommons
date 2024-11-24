using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace DerpyNewbie.Common
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PickupReferenceChanger : UdonSharpBehaviour
    {
        [SerializeField] private VRCPickup pickup;
        [SerializeField] private Transform pickupReference;
        [SerializeField] private Transform leftHandReference;
        [SerializeField] private Transform rightHandReference;

        public override void OnPickup()
        {
            _SetPickupReference(
                pickup.currentHand == VRC_Pickup.PickupHand.Left
                    ? leftHandReference
                    : rightHandReference
            );
        }

        private void _SetPickupReference(Transform t)
        {
            pickupReference.SetPositionAndRotation(t.position, t.rotation);
        }
    }
}