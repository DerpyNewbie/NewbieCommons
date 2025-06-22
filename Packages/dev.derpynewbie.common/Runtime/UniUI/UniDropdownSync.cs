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

        [UdonSynced]
        private int _syncedValue;

        private bool _inSyncedChangeContext;

        private void Start()
        {
            // serialize the initial value when instance master has first joined
            if (!Networking.IsMaster) return;
            _syncedValue = dropdown.value;
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            // apply deserialized values to UI
            _inSyncedChangeContext = true;
            dropdown.value = _syncedValue;
            dropdown.RefreshShownValue();
            _inSyncedChangeContext = false;
        }

        public override void OnUniUIUpdate()
        {
            // don't resync if the event was called by OnDeserialization change
            if (_inSyncedChangeContext) return;

            _syncedValue = dropdown.value;
            if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }
    }
}