using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace DerpyNewbie.Common.UI
{
    [AddComponentMenu("Newbie Commons/UI/Double Click Button")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DoubleClickButton : UdonSharpBehaviour
    {
        [SerializeField]
        private UdonSharpBehaviour callback;

        [SerializeField]
        private string callbackMethod;

        [SerializeField]
        private Text text;

        [SerializeField]
        private TMP_Text tmpText;

        [SerializeField]
        private float confirmDuration = 5F;

        [SerializeField]
        private string confirmTextMessage = "Are you sure?";

        private string _defaultTextMessage;
        private string _defaultTmpTextMessage;

        private bool _isInConfirmState;
        private DateTime _lastClickedTime;

        private void Start()
        {
            if (text) _defaultTextMessage = text.text;
            if (tmpText) _defaultTmpTextMessage = tmpText.text;
        }

        public void OnClick()
        {
            var now = DateTime.Now;
            var diff = now.Subtract(_lastClickedTime).TotalSeconds;
            Debug.Log($"[DoubleClickButton-{name}] OnClick: {diff:F2}");

            if (_isInConfirmState && diff < confirmDuration)
            {
                InvokeCallback();
                ToDefaultState();
            }
            else
            {
                ToConfirmState();
            }

            _lastClickedTime = now;
            SendCustomEventDelayedSeconds(nameof(CheckStateAndSchedule), confirmDuration + 0.1F);
        }

        public void CheckStateAndSchedule()
        {
            var diff = DateTime.Now.Subtract(_lastClickedTime).TotalSeconds;
            Debug.Log($"[DoubleClickButton-{name}] CheckStateAndSchedule: {_isInConfirmState}, {diff:F2}");
            if (!_isInConfirmState || diff > confirmDuration)
            {
                ToDefaultState();
                return;
            }

            Debug.Log($"[DoubleClickButton-{name}] CheckStateAndSchedule: scheduling again because not yet confirmed");
            SendCustomEventDelayedSeconds(nameof(CheckStateAndSchedule), 0.5F);
        }

        private void InvokeCallback()
        {
            callback.SendCustomEvent(callbackMethod);
        }

        private void ToDefaultState()
        {
            if (text) text.text = _defaultTextMessage;
            if (tmpText) tmpText.text = _defaultTmpTextMessage;
            _isInConfirmState = false;
        }

        private void ToConfirmState()
        {
            if (text) text.text = confirmTextMessage;
            if (tmpText) tmpText.text = confirmTextMessage;
            _isInConfirmState = true;
        }
    }
}