using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common.Role
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RolePlayerData : UdonSharpBehaviour
    {
        [SerializeField]
        private string displayName;
        [SerializeField]
        private RoleData[] roles;

        [PublicAPI]
        public string DisplayName => displayName;
        [PublicAPI] [ItemCanBeNull]
        public RoleData[] Roles => roles;
    }
}