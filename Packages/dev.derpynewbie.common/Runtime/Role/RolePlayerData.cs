using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common.Role
{
    [AddComponentMenu("Newbie Commons/Role/Player Data")]
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