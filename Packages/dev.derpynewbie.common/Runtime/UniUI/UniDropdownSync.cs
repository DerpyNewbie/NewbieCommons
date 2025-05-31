using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common.UniUI
{
    [AddComponentMenu("Newbie Commons/UniUI/UniDropdownSync")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class UniDropdownSync : UniUI
    {
        [SerializeField] [NewbieInject(SearchScope.Parents)]
        private TMP_Dropdown dropdown;

        [UdonSynced] [FieldChangeCallback(nameof(SyncedValue))]
        private int _syncedValue;

        private bool _inSyncedChangeContext;

        public int SyncedValue
        {
            get => _syncedValue;
            set
            {
                _syncedValue = value;
                if (_inSyncedChangeContext) return;

                _inSyncedChangeContext = true;
                dropdown.value = value;
                dropdown.RefreshShownValue();
                _inSyncedChangeContext = false;
            }
        }

        private void Start()
        {
            if (!Networking.IsMaster) return;
            _syncedValue = dropdown.value;
            RequestSerialization();
        }

        public override void OnUniUIUpdate()
        {
            if (_inSyncedChangeContext || dropdown.value == SyncedValue) return;

            SyncedValue = dropdown.value;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }
    }
}