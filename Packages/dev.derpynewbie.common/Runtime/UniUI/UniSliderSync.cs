using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace DerpyNewbie.Common.UniUI
{
    [AddComponentMenu("Newbie Commons/UniUI/UniSliderSync")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class UniSliderSync : UniUI
    {
        [SerializeField] [NewbieInject(SearchScope.Parents)]
        private Slider slider;

        [UdonSynced]
        private float _syncedValue;

        private bool _inSyncedChangeContext;

        private void Start()
        {
            // serialize the initial value when instance master has first joined
            if (!Networking.IsMaster) return;
            _syncedValue = slider.value;
            RequestSerialization();
        }

        public override void OnDeserialization()
        {
            // apply deserialized values to UI
            _inSyncedChangeContext = true;
            slider.value = _syncedValue;
            _inSyncedChangeContext = false;
        }

        public override void OnUniUIUpdate()
        {
            // don't resync if the event was called by OnDeserialization change
            if (_inSyncedChangeContext) return;

            _syncedValue = slider.value;
            if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }
    }
}