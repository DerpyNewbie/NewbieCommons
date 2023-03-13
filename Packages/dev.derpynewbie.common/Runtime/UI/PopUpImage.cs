using System;
using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common.UI
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PopUpImage : UdonSharpBehaviour
    {
        [SerializeField]
        private bool allowOverridePlay;
        [SerializeField]
        private float playTime = 0.5F;
        private float _currentTime;
        private DateTime _hidingTime;

        private Vector3 _initialScale;

        private bool _isHidingLater;
        private bool _isPlaying;
        private bool _isShowing;

        private void Start()
        {
            _initialScale = transform.localScale;
            Show();
            Hide();
        }

        private void FixedUpdate()
        {
            if (_isHidingLater && DateTime.Now > _hidingTime)
            {
                Hide();
                _isHidingLater = false;
            }

            if (!_isPlaying)
                return;

            _currentTime += Time.deltaTime;
            var progress = _currentTime / playTime;
            if (progress < 1)
            {
                transform.localScale = _isShowing
                    ? Vector3.Slerp(Vector3.zero, _initialScale, progress)
                    : Vector3.Slerp(_initialScale, Vector3.zero, progress);
            }
            else
            {
                transform.localScale = _isShowing ? _initialScale : Vector3.zero;
                _currentTime = 0;
                _isPlaying = false;
                gameObject.SetActive(_isShowing);
            }
        }

        public void Show()
        {
            if (_isPlaying && !allowOverridePlay)
                return;

            _currentTime = 0;
            _isPlaying = true;
            _isShowing = true;

            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (!_isShowing)
                return;

            _currentTime = 0;
            _isPlaying = true;
            _isShowing = false;

            transform.localScale = _initialScale;
        }

        public void ShowAndHideLater(float timeDiffInSeconds)
        {
            Show();
            _isHidingLater = true;
            _hidingTime = DateTime.Now.AddSeconds(timeDiffInSeconds);
        }
    }
}