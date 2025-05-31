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

        [UdonSynced] [FieldChangeCallback(nameof(SyncedValue))]
        private float _syncedValue;

        private bool _inSyncedChangeContext;

        public float SyncedValue
        {
            get => _syncedValue;
            set
            {
                _syncedValue = value;
                if (_inSyncedChangeContext) return;

                _inSyncedChangeContext = true;
                slider.value = value;
                _inSyncedChangeContext = false;
            }
        }

        private void Start()
        {
            if (!Networking.IsMaster) return;
            _syncedValue = slider.value;
            RequestSerialization();
        }

        public override void OnUniUIUpdate()
        {
            if (_inSyncedChangeContext || Mathf.Approximately(slider.value, SyncedValue)) return;

            SyncedValue = slider.value;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }
    }
}