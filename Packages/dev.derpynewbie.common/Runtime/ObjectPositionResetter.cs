using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

namespace DerpyNewbie.Common
{
    [AddComponentMenu("Newbie Commons/Utils/Object Position Resetter")]
    [RequireComponent(typeof(VRCPickup))] 
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ObjectPositionResetter : UdonSharpBehaviour
    {
        private VRCPickup _pickup;
        private Vector3 _resetPosition;

        private void Start()
        {
            _pickup = (VRCPickup) GetComponent(typeof(VRCPickup));
            _resetPosition = transform.position;
        }

        public override void OnPickupUseDown()
        {
            ResetPosition();
        }

        public void ResetPosition()
        {
            _pickup.Drop();
            transform.position = _resetPosition;
        }
    }
}