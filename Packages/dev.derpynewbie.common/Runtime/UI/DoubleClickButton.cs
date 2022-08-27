using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace DerpyNewbie.Common.UI
{
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
        private float confirmDuration = 5F;
        [SerializeField]
        private string confirmTextMessage = "Are you sure?";

        private string _defaultTextMessage;

        private bool _isInConfirmState;
        private DateTime _lastClickedTime;

        private void Start()
        {
            _defaultTextMessage = text.text;
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
            text.text = _defaultTextMessage;
            _isInConfirmState = false;
        }

        private void ToConfirmState()
        {
            text.text = confirmTextMessage;
            _isInConfirmState = true;
        }
    }
}