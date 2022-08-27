using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MirrorHandler : UdonSharpBehaviour
    {
        public GameObject highMirror;
        public GameObject lowMirror;
        private bool _isLow;
        private bool _isOn;

        private void Start()
        {
            if (!highMirror || !lowMirror)
                Debug.LogError("Mirror doesn't exist! oh no!");
        }

        public void ToggleMirror()
        {
            _isOn = !_isOn;
            SetMirrorOnOff();
        }

        public void ToggleLowQuality()
        {
            _isLow = !_isLow;
            SetMirrorOnOff();
        }

        public void SetLowQuality(bool isLowQuality)
        {
            _isLow = isLowQuality;
            SetMirrorOnOff();
        }

        public void SetMirrorOn(bool isMirrorOn)
        {
            _isOn = isMirrorOn;
            SetMirrorOnOff();
        }

        public bool IsOn()
        {
            return _isOn;
        }

        public bool IsLow()
        {
            return _isLow;
        }

        private void SetMirrorOnOff()
        {
            highMirror.SetActive(_isOn && !_isLow);
            lowMirror.SetActive(_isOn && _isLow);
        }
    }
}