using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common
{
    [AddComponentMenu("Newbie Commons/Utils/Object Player Follower")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ObjectPlayerFollower : UdonSharpBehaviour
    {
        [SerializeField]
        private Transform original;
        [SerializeField]
        private Transform playerFollower;
        [SerializeField]
        private GameObject objectToToggle;

        private bool _isFollowing = false;

        public override void OnPickupUseDown()
        {
            ToggleFollow();
        }

        private void ToggleFollow()
        {
            _isFollowing = !_isFollowing;
            transform.SetParent(_isFollowing ? playerFollower : original);
            objectToToggle.SetActive(_isFollowing);
        }
    }
}