using JetBrains.Annotations;
using UdonSharp;
using VRC.SDKBase;

namespace DerpyNewbie.Common.Role
{
    public abstract class RoleProvider : UdonSharpBehaviour
    {
        [PublicAPI]
        public abstract RoleData DefaultRoleData { get; }
        [PublicAPI]
        public abstract RoleData[] Roles { get; }

        [PublicAPI] [NotNull] [ItemNotNull]
        public abstract string[] GetPlayerNamesOf(RoleData roleData, bool includeOffline);

        [PublicAPI]
        public string[] GetPlayerNamesOf(RoleData roleData) => GetPlayerNamesOf(roleData, false);

        [PublicAPI] [NotNull] [ItemNotNull]
        public abstract VRCPlayerApi[] GetPlayersOf(RoleData roleData);

        /// <summary>
        /// Gets current <see cref="RoleData"/> for specified player display name.
        /// </summary>
        /// <remarks>
        /// Should return from most to least relevant roles.
        /// </remarks>
        /// <param name="displayName">Display name of player you want to query roles.</param>
        /// <returns>Current <see cref="RoleData"/>s for player of that display name.</returns>
        [PublicAPI] [NotNull] [ItemNotNull]
        public abstract RoleData[] GetPlayerRoles(string displayName);

        [PublicAPI]
        public RoleData[] GetPlayerRoles(VRCPlayerApi playerApi) =>
            GetPlayerRoles(playerApi != null ? playerApi.displayName : "Default");

        /// <summary>
        /// Gets current roles of local player.
        /// </summary>
        /// <returns>Current <see cref="RoleData"/>s of local player</returns>
        [PublicAPI]
        public RoleData[] GetPlayerRoles() => GetPlayerRoles(Networking.LocalPlayer.displayName);

        /// <summary>
        /// Gets current most relevant <see cref="RoleData"/> for specified player display name.
        /// </summary>
        /// <param name="displayName">Display name of player you want to query role.</param>
        /// <returns>Current most relevant <see cref="RoleData"/> for specified player of that display name.</returns>
        [PublicAPI]
        public RoleData GetPlayerRole(string displayName)
        {
            var result = GetPlayerRoles(displayName);
            return result.Length != 0 ? result[0] : DefaultRoleData;
        }

        [PublicAPI]
        public RoleData GetPlayerRole(VRCPlayerApi playerApi) =>
            GetPlayerRole(playerApi != null ? playerApi.displayName : "Default");

        /// <summary>
        /// Gets current role of local player.
        /// </summary>
        /// <returns>Current <see cref="RoleData"/> of local player</returns>
        [PublicAPI]
        public RoleData GetPlayerRole() => GetPlayerRole(Networking.LocalPlayer.displayName);

        [PublicAPI] [CanBeNull]
        public RoleData RoleOf(string roleName)
        {
            foreach (var role in Roles)
                if (role.RoleName == roleName)
                    return role;
            return null;
        }
    }
}