using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common.Role
{
    [AddComponentMenu("Newbie Commons/Role/Role Manager")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RoleManager : RoleProvider
    {
        [SerializeField]
        private RoleData[] availableRoles;
        [SerializeField]
        private RolePlayerData[] players;

        public override RoleData DefaultRoleData => availableRoles[0];
        public override RoleData[] Roles => availableRoles;

        public override string[] GetPlayerNamesOf(RoleData roleData, bool includeOffline)
        {
            if (roleData == DefaultRoleData)
            {
                var allPlayers = VRCPlayerApi.GetPlayers(new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()]);
                var names = new string[allPlayers.Length];
                for (int i = 0; i < allPlayers.Length; i++)
                    names[i] = allPlayers[i].displayName;
                return names;
            }

            if (includeOffline)
            {
                var names = new string[0];
                foreach (var player in players)
                    if (player.Roles.ContainsItem(roleData))
                        names = names.AddAsSet(player.DisplayName);
                return names;
            }
            else
            {
                var names = new string[0];
                foreach (var player in GetPlayersOf(roleData))
                    names = names.AddAsSet(player.displayName);
                return names;
            }
        }

        public override VRCPlayerApi[] GetPlayersOf(RoleData roleData)
        {
            var allPlayers = VRCPlayerApi.GetPlayers(new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()]);
            var queryResult = new VRCPlayerApi[0];
            foreach (var player in allPlayers)
                if (GetPlayerRoles(player).ContainsItem(roleData))
                    queryResult = queryResult.AddAsSet(player);
            return queryResult;
        }

        public override RoleData[] GetPlayerRoles(string displayName)
        {
            foreach (var player in players)
                if (player.DisplayName == displayName)
                    return player.Roles;
            return new[] { DefaultRoleData };
        }
    }
}