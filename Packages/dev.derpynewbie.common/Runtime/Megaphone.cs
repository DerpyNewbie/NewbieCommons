using System;
using DerpyNewbie.Common.Invoker;
using DerpyNewbie.Common.UI;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace DerpyNewbie.Common
{
    [AddComponentMenu("Newbie Commons/Utils/Megaphone")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Megaphone : PickupEventSenderCallback
    {
        [SerializeField]
        private VRC_Pickup pickup;
        [SerializeField]
        private MeshRenderer meshRenderer;
        [UdonSynced]
        private int _amplifiedMode;
        [UdonSynced] [FieldChangeCallback(nameof(IsAmplified))]
        private bool _isAmplified;

        private bool _isPickedUp;
        private bool _isResetConfirming;

        [UdonSynced] [FieldChangeCallback(nameof(PlayerId))]
        private int _playerId;

        private Vector3 _resetPosition;
        private Quaternion _resetRotation;

        public PlayerVoiceStore AmplifiedVoiceStore
        {
            get
            {
                switch (_amplifiedMode)
                {
                    case 0:
                        return amplifiedAllVoice;
                    case 1:
                        return amplified20MVoice;
                    case 2:
                        return amplified30MVoice;
                    default:
                        return amplifiedAllVoice;
                }
            }
        }
        public int PlayerId
        {
            get => _playerId;
            set
            {
                if (_playerId != value && _isAmplified)
                    IsAmplified = false;

                _playerId = value;
            }
        }
        public bool IsAmplified
        {
            get => _isAmplified;
            set
            {
                _isAmplified = value;
                var api = VRCPlayerApi.GetPlayerById(PlayerId);
                if (api == null || !api.IsValid()) return;

                if (value)
                    SetToAmplified(api);
                else
                    SetToDefault(api);
            }
        }
        public bool IsHeld => pickup.IsHeld;

        private void Start()
        {
            var t = pickup.transform;
            _resetPosition = t.position;
            _resetRotation = t.rotation;

            if (defaultMaterial == null)
                defaultMaterial = meshRenderer.materials;
            if (amplifiedMaterial == null)
                amplifiedMaterial = defaultMaterial;

            if (modeSelectionButton == null)
                modeSelectionButton = GameObject.Find(MegaphoneModeSelectionButtonHint).GetComponent<SelectionButton>();
            if (confirmSelectionButton == null)
                confirmSelectionButton =
                    GameObject.Find(ConfirmResetSelectionButtonHint).GetComponent<SelectionButton>();

            // I know. This code smells so bad.
            // TODO: Must use Array instead of independent variables
            if (defaultVoice == null)
                defaultVoice = GameObject.Find(DefaultVoiceHint).GetComponent<PlayerVoiceStore>();
            if (amplified20MVoice == null)
                amplified20MVoice = GameObject.Find(Amplified20MVoiceHint).GetComponent<PlayerVoiceStore>();
            if (amplified30MVoice == null)
                amplified30MVoice = GameObject.Find(Amplified30MVoiceHint).GetComponent<PlayerVoiceStore>();
            if (amplifiedAllVoice == null)
                amplifiedAllVoice = GameObject.Find(AmplifiedAllVoiceHint).GetComponent<PlayerVoiceStore>();
        }

        public override void InputJump(bool value, UdonInputEventArgs args)
        {
            if (_isPickedUp && value)
            {
                if (_isResetConfirming)
                {
                    var i = confirmSelectionButton.selectedElement;
                    confirmSelectionButton.selectedElement = i == 0 ? 1 : 0;
                    return;
                }

                ++_amplifiedMode;
                if (_amplifiedMode > 3)
                    _amplifiedMode = 0;
                modeSelectionButton.selectedElement = _amplifiedMode;
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                RequestSerialization();
            }
        }

        public override void OnPickupRelayed()
        {
            Debug.Log("[Megaphone] OnPickup");
            modeSelectionButton.transform.parent.SetParent(transform, false);

            modeSelectionButton.selectedElement = _amplifiedMode;
            modeSelectionButton.confirmedElement = -1;
            modeSelectionButton.gameObject.SetActive(true);

            confirmSelectionButton.selectedElement = 0;
            confirmSelectionButton.confirmedElement = -1;
            confirmSelectionButton.gameObject.SetActive(false);

            _isPickedUp = true;
            PlayerId = Networking.LocalPlayer.playerId;

            if (!Networking.IsOwner(gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }

        public override void OnPickupUseDownRelayed()
        {
            Debug.Log("[Megaphone] OnPickupUseDown");
            if (_amplifiedMode == 3)
            {
                if (!_isResetConfirming)
                {
                    ShowResetConfirm();
                    return;
                }

                if (confirmSelectionButton.selectedElement == 0)
                {
                    ResetPosition();
                    HideResetConfirm(true);
                }
                else
                {
                    HideResetConfirm(false);
                }

                return;
            }

            if (!enableToggleForVoiceAmplifier)
            {
                modeSelectionButton.confirmedElement = _amplifiedMode;
                IsAmplified = true;
            }
            else
            {
                IsAmplified = !IsAmplified;
                modeSelectionButton.confirmedElement = IsAmplified ? _amplifiedMode : -1;
            }

            if (!Networking.IsOwner(gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }

        public override void OnPickupUseUpRelayed()
        {
            Debug.Log("[Megaphone] OnPickupUseUp");
            if (!enableToggleForVoiceAmplifier)
            {
                modeSelectionButton.confirmedElement = -1;
                IsAmplified = false;
                if (!Networking.IsOwner(gameObject))
                    Networking.SetOwner(Networking.LocalPlayer, gameObject);
                RequestSerialization();
            }
        }

        public override void OnDropRelayed()
        {
            Debug.Log("[Megaphone] OnDrop");
            _isPickedUp = false;
            modeSelectionButton.gameObject.SetActive(false);
            _isResetConfirming = false;
            confirmSelectionButton.gameObject.SetActive(false);

            IsAmplified = false;
            if (!Networking.IsOwner(gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }

        public void SetToDefault(VRCPlayerApi api)
        {
            if (api == null || !api.IsValid())
            {
                Debug.LogError("[Megaphone] SetToDefault: Api is null!");
                return;
            }

            SetMaterial(defaultMaterial);
            defaultVoice.SetVoice(api);
            Debug.Log($"[Megaphone] player {api.displayName} is now default");
        }

        public void SetToAmplified(VRCPlayerApi api)
        {
            if (api == null || !api.IsValid())
            {
                Debug.LogError("[Megaphone] SetToAmplified: Api is null!");
                return;
            }

            SetMaterial(amplifiedMaterial);
            AmplifiedVoiceStore.SetVoice(api);
            Debug.Log($"[Megaphone] player {api.displayName} is now amplified");
        }

        private void SetMaterial(Material[] mat)
        {
            if (mat != null && meshRenderer != null)
                meshRenderer.materials = mat;
        }

        public void ResetPosition()
        {
            _amplifiedMode = 0;
            modeSelectionButton.selectedElement = 0;
            MoveTo(_resetPosition, _resetRotation);
        }

        public void MoveTo(Vector3 pos, Quaternion rot)
        {
            pickup.Drop(Networking.LocalPlayer);
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Networking.SetOwner(Networking.LocalPlayer, pickup.gameObject);
            pickup.transform.SetPositionAndRotation(pos, rot);
        }

        private void ShowResetConfirm()
        {
            modeSelectionButton.gameObject.SetActive(false);

            confirmSelectionButton.selectedElement = 0;
            confirmSelectionButton.confirmedElement = -1;
            confirmSelectionButton.gameObject.SetActive(true);
            _isResetConfirming = true;
        }

        public void HideResetConfirm(bool hideModeSelection)
        {
            modeSelectionButton.gameObject.SetActive(!hideModeSelection);

            confirmSelectionButton.selectedElement = 0;
            confirmSelectionButton.confirmedElement = -1;
            confirmSelectionButton.gameObject.SetActive(false);
            _isResetConfirming = false;
        }

        #region SerializedFields

        [Tooltip("Do UseDown toggle amplifier or be just button")]
        public bool enableToggleForVoiceAmplifier = true;
        public SelectionButton modeSelectionButton;
        public SelectionButton confirmSelectionButton;

        public Material[] defaultMaterial;
        public Material[] amplifiedMaterial;

        [NonSerialized]
        private const string MegaphoneModeSelectionButtonHint = "MegaphoneUISelectionPanel";
        [NonSerialized]
        private const string ConfirmResetSelectionButtonHint = "MegaphoneUIConfirmPanel";
        [NonSerialized]
        private const string DefaultVoiceHint = "DefaultVoice";
        [NonSerialized]
        private const string Amplified20MVoiceHint = "20mVoice";
        [NonSerialized]
        private const string Amplified30MVoiceHint = "30mVoice";
        [NonSerialized]
        private const string AmplifiedAllVoiceHint = "AllVoice";

        public PlayerVoiceStore defaultVoice;
        public PlayerVoiceStore amplified20MVoice;
        public PlayerVoiceStore amplified30MVoice;
        public PlayerVoiceStore amplifiedAllVoice;

        #endregion
    }
}