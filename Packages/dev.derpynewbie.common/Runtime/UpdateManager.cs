using System;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)] [DefaultExecutionOrder(-100000)]
    public class UpdateManager : UdonSharpBehaviour
    {
        private const int InitialPoolSize = 256;
        private UdonSharpBehaviour[] _fixedUpdateBehaviours;
        private int _fixedUpdateBehavioursCount;

        private bool _isInitialized;
        private int _lastCalledSlowFixedUpdateIndex;
        private int _lastCalledSlowPostLateUpdateIndex;
        private int _lastCalledSlowUpdateIndex;

        private UdonSharpBehaviour[] _postLateUpdateBehaviours;
        private int _postLateUpdateBehavioursCount;
        private UdonSharpBehaviour[] _slowFixedUpdateBehaviours;
        private int _slowFixedUpdateBehavioursCount;
        private UdonSharpBehaviour[] _slowPostLateUpdateBehaviours;
        private int _slowPostLateUpdateBehavioursCount;
        private UdonSharpBehaviour[] _slowUpdateBehaviours;
        private int _slowUpdateBehavioursCount;
        private UdonSharpBehaviour[] _updateBehaviours;
        private int _updateBehavioursCount;

        private void Start()
        {
            _EnsureInit();
        }

        private void Update()
        {
            for (var i = 0; i < _updateBehavioursCount; i++)
            {
                var b = _updateBehaviours[i];
                b.SendCustomEvent("_Update");
            }

            if (_slowUpdateBehavioursCount != 0)
            {
                ++_lastCalledSlowUpdateIndex;
                if (_lastCalledSlowUpdateIndex >= _slowUpdateBehavioursCount)
                    _lastCalledSlowUpdateIndex = 0;

                var b = _slowUpdateBehaviours[_lastCalledSlowUpdateIndex];
                b.SendCustomEvent("_SlowUpdate");
            }
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < _fixedUpdateBehavioursCount; i++)
            {
                var b = _fixedUpdateBehaviours[i];
                b.SendCustomEvent("_FixedUpdate");
            }

            if (_slowFixedUpdateBehavioursCount != 0)
            {
                ++_lastCalledSlowFixedUpdateIndex;
                if (_lastCalledSlowFixedUpdateIndex >= _slowFixedUpdateBehavioursCount)
                    _lastCalledSlowFixedUpdateIndex = 0;

                var b = _slowFixedUpdateBehaviours[_lastCalledSlowFixedUpdateIndex];
                b.SendCustomEvent("_SlowFixedUpdate");
            }
        }

        public override void PostLateUpdate()
        {
            for (var i = 0; i < _postLateUpdateBehavioursCount; i++)
            {
                var b = _postLateUpdateBehaviours[i];
                b.SendCustomEvent("_PostLateUpdate");
            }

            if (_slowPostLateUpdateBehavioursCount != 0)
            {
                ++_lastCalledSlowPostLateUpdateIndex;
                if (_lastCalledSlowPostLateUpdateIndex >= _slowPostLateUpdateBehavioursCount)
                    _lastCalledSlowPostLateUpdateIndex = 0;

                var b = _slowPostLateUpdateBehaviours[_lastCalledSlowPostLateUpdateIndex];
                b.SendCustomEvent("_SlowPostLateUpdate");
            }
        }

        private void _EnsureInit()
        {
            if (_isInitialized) return;

            _fixedUpdateBehavioursCount = 0;
            _fixedUpdateBehaviours = new UdonSharpBehaviour[InitialPoolSize];
            _slowFixedUpdateBehavioursCount = 0;
            _slowFixedUpdateBehaviours = new UdonSharpBehaviour[InitialPoolSize];
            _updateBehavioursCount = 0;
            _updateBehaviours = new UdonSharpBehaviour[InitialPoolSize];
            _slowUpdateBehavioursCount = 0;
            _slowUpdateBehaviours = new UdonSharpBehaviour[InitialPoolSize];
            _postLateUpdateBehavioursCount = 0;
            _postLateUpdateBehaviours = new UdonSharpBehaviour[InitialPoolSize];
            _slowPostLateUpdateBehavioursCount = 0;
            _slowPostLateUpdateBehaviours = new UdonSharpBehaviour[InitialPoolSize];
            _isInitialized = true;
        }

        [Pure]
        private static void _AddBehaviour(int index, UdonSharpBehaviour item, UdonSharpBehaviour[] arr,
            out UdonSharpBehaviour[] newArr)
        {
            if (arr.Length <= index)
            {
                var upScaledArr = new UdonSharpBehaviour[arr.Length + InitialPoolSize];
                Array.Copy(arr, upScaledArr, arr.Length);
                arr = upScaledArr;
            }

#if UNITY_EDITOR
            Debug.Log($"[UpdateManager] add behaviour at {index} {item.name}");
#endif

            arr[index] = item;
            newArr = arr;
        }

        private static bool _RemoveBehaviour(UdonSharpBehaviour item, UdonSharpBehaviour[] arr,
            out UdonSharpBehaviour[] newArr)
        {
            var index = Array.IndexOf(arr, item);
            if (index == -1)
            {
                newArr = arr;
                return false;
            }

            Array.ConstrainedCopy(arr, index + 1, arr, index, arr.Length - 1 - index);

#if UNITY_EDITOR
            Debug.Log($"[UpdateManager] remove behaviour at {index} {item.name}");
#endif

            newArr = arr;
            return true;
        }

        public void SubscribeUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            _AddBehaviour(_updateBehavioursCount++, behaviour, _updateBehaviours, out _updateBehaviours);
        }

        public void UnsubscribeUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            if (_RemoveBehaviour(behaviour, _updateBehaviours, out _updateBehaviours))
                --_updateBehavioursCount;
        }

        public void SubscribeSlowUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            _AddBehaviour(_slowUpdateBehavioursCount++, behaviour, _slowUpdateBehaviours, out _slowUpdateBehaviours);
        }

        public void UnsubscribeSlowUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            if (_RemoveBehaviour(behaviour, _slowUpdateBehaviours, out _slowUpdateBehaviours))
                --_slowUpdateBehavioursCount;
        }

        public void SubscribePostLateUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            _AddBehaviour(_postLateUpdateBehavioursCount++, behaviour, _postLateUpdateBehaviours,
                out _postLateUpdateBehaviours);
        }

        public void UnsubscribePostLateUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            if (_RemoveBehaviour(behaviour, _postLateUpdateBehaviours, out _postLateUpdateBehaviours))
                --_postLateUpdateBehavioursCount;
        }

        public void SubscribeSlowPostLateUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            _AddBehaviour(_slowPostLateUpdateBehavioursCount++, behaviour, _slowPostLateUpdateBehaviours,
                out _slowPostLateUpdateBehaviours);
        }

        public void UnsubscribeSlowLateUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            if (_RemoveBehaviour(behaviour, _slowPostLateUpdateBehaviours, out _slowPostLateUpdateBehaviours))
                --_slowPostLateUpdateBehavioursCount;
        }

        public void SubscribeFixedUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            _AddBehaviour(_fixedUpdateBehavioursCount++, behaviour, _fixedUpdateBehaviours, out _fixedUpdateBehaviours);
        }

        public void UnsubscribeFixedUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            if (_RemoveBehaviour(behaviour, _fixedUpdateBehaviours, out _fixedUpdateBehaviours))
                --_fixedUpdateBehavioursCount;
        }

        public void SubscribeSlowFixedUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            _AddBehaviour(_slowFixedUpdateBehavioursCount++, behaviour, _slowFixedUpdateBehaviours,
                out _slowFixedUpdateBehaviours);
        }

        public void UnsubscribeSlowFixedUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            if (_RemoveBehaviour(behaviour, _slowFixedUpdateBehaviours, out _slowFixedUpdateBehaviours))
                --_slowFixedUpdateBehavioursCount;
        }
    }
}