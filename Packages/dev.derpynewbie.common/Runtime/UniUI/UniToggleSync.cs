using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace DerpyNewbie.Common.UniUI
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class UniToggleSync : UniUI
    {
        [SerializeField] [NewbieInject(SearchScope.Parents)]
        private Toggle toggle;

        [UdonSynced] [FieldChangeCallback(nameof(SyncedIsOn))]
        private bool _syncedIsOn;

        private bool _inSyncedChangeContext;

        public bool SyncedIsOn
        {
            get => _syncedIsOn;
            set
            {
                _syncedIsOn = value;
                if (_inSyncedChangeContext) return;

                _inSyncedChangeContext = true;
                toggle.isOn = value;
                _inSyncedChangeContext = false;
            }
        }

        private void Start()
        {
            if (!Networking.IsMaster) return;
            _syncedIsOn = toggle.isOn;
            RequestSerialization();
        }

        public override void OnUniUIUpdate()
        {
            if (_inSyncedChangeContext || toggle.isOn == SyncedIsOn) return;

            SyncedIsOn = toggle.isOn;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }
    }
}