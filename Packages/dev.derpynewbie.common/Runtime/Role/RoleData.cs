using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common.Role
{
    [AddComponentMenu("Newbie Commons/Role/Role Data")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RoleData : UdonSharpBehaviour
    {
        [SerializeField]
        private string roleName;
        [SerializeField]
        private string[] roleProperties;

        public string RoleName => roleName;
        public string[] RoleProperties => roleProperties;
    }
}