using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PlayerMovement : UdonSharpBehaviour
    {
        [UdonSynced]
        public float jumpImpulse = 0F;
        [UdonSynced]
        public float runSpeed = 2F;
        [UdonSynced]
        public float walkSpeed = 1F;
        [UdonSynced]
        public float strafeSpeed = 1F;
        [UdonSynced]
        public float gravityStrength = 1F;

        private float _initJumpImpulse;
        private float _initRunSpeed;
        private float _initWalkSpeed;
        private float _initStrafeSpeed;
        private float _initGravityStr;

        private void Start()
        {
            _initJumpImpulse = jumpImpulse;
            _initRunSpeed = runSpeed;
            _initWalkSpeed = walkSpeed;
            _initStrafeSpeed = strafeSpeed;
            _initGravityStr = gravityStrength;

            UpdatePlayerMovement();
        }

        public override void OnPreSerialization()
        {
            UpdatePlayerMovement();
        }

        public override void OnDeserialization()
        {
            UpdatePlayerMovement();
        }

        [PublicAPI]
        public void ApplySetting()
        {
            Debug.Log("PlayerMovement: applying movement config");
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();

            UpdateSetting();
        }

        [PublicAPI]
        public void ResetSetting()
        {
            Debug.Log("PlayerMovement: resetting movement config");
            jumpImpulse = _initJumpImpulse;
            runSpeed = _initRunSpeed;
            walkSpeed = _initWalkSpeed;
            strafeSpeed = _initStrafeSpeed;
            gravityStrength = _initGravityStr;

            UpdateSetting();
        }

        [PublicAPI]
        public void UpdateSetting()
        {
            UpdatePlayerMovement();
        }

        private void UpdatePlayerMovement()
        {
            Debug.Log("PlayerMovement: updating movement config");
            var p = Networking.LocalPlayer;

            p.SetWalkSpeed(walkSpeed);
            p.SetStrafeSpeed(strafeSpeed);
            p.SetGravityStrength(gravityStrength);
            p.SetJumpImpulse(jumpImpulse);
            p.SetRunSpeed(runSpeed);
        }
    }
}