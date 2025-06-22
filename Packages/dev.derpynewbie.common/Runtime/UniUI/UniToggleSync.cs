using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace DerpyNewbie.Common.UniUI
{
    [AddComponentMenu("Newbie Commons/UniUI/UniToggleSync")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class UniToggleSync : UniUI
    {
        [SerializeField] [NewbieInject(SearchScope.Parents)]
        private Toggle toggle;

        [UdonSynced]
        private bool _syncedIsOn;

        private bool _inSyncedChangeContext;

        private void Start()
        {
            // serialize the initial value when instance master has first joined
            if (!Networking.IsMaster) return;
            _syncedIsOn = toggle.isOn;
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            // apply deserialized values to UI
            _inSyncedChangeContext = true;
            toggle.isOn = _syncedIsOn;
            _inSyncedChangeContext = false;
        }

        public override void OnUniUIUpdate()
        {
            // don't resync if the event was called by OnDeserialization change
            if (_inSyncedChangeContext) return;

            _syncedIsOn = toggle.isOn;
            if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }
    }
}