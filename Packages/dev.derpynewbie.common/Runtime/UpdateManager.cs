using System;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common
{
    [AddComponentMenu("Newbie Commons/Utils/UpdateManager")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [DefaultExecutionOrder(-100000)]
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

        #region EventSource

        private void Update()
        {
            for (var i = 0; i < _updateBehavioursCount; i++)
            {
                var b = _updateBehaviours[i];

#if NEWBIECOMMONS_ENABLE_UPDATER_NULL_CHECK
                if (b == null)
                {
                    Debug.Log($"[UpdateManager] Update {i} is null");
                    continue;
                }
#endif

                b.SendCustomEvent("_Update");
            }

            if (_slowUpdateBehavioursCount != 0)
            {
                ++_lastCalledSlowUpdateIndex;
                if (_lastCalledSlowUpdateIndex >= _slowUpdateBehavioursCount)
                    _lastCalledSlowUpdateIndex = 0;

                var b = _slowUpdateBehaviours[_lastCalledSlowUpdateIndex];
#if NEWBIECOMMONS_ENABLE_UPDATER_NULL_CHECK
                if (b == null)
                {
                    Debug.Log($"[UpdateManager] SlowUpdate {_lastCalledSlowUpdateIndex} is null");
                    return;
                }
#endif

                b.SendCustomEvent("_SlowUpdate");
            }
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < _fixedUpdateBehavioursCount; i++)
            {
                var b = _fixedUpdateBehaviours[i];
#if NEWBIECOMMONS_ENABLE_UPDATER_NULL_CHECK
                if (b == null)
                {
                    Debug.Log($"[UpdateManager] FixedUpdate {i} is null");
                    continue;
                }
#endif

                b.SendCustomEvent("_FixedUpdate");
            }

            if (_slowFixedUpdateBehavioursCount != 0)
            {
                ++_lastCalledSlowFixedUpdateIndex;
                if (_lastCalledSlowFixedUpdateIndex >= _slowFixedUpdateBehavioursCount)
                    _lastCalledSlowFixedUpdateIndex = 0;

                var b = _slowFixedUpdateBehaviours[_lastCalledSlowFixedUpdateIndex];
#if NEWBIECOMMONS_ENABLE_UPDATER_NULL_CHECK
                if (b == null)
                {
                    Debug.Log($"[UpdateManager] SlowFixedUpdate {_lastCalledSlowFixedUpdateIndex} is null");
                    return;
                }
#endif

                b.SendCustomEvent("_SlowFixedUpdate");
            }
        }

        public override void PostLateUpdate()
        {
            for (var i = 0; i < _postLateUpdateBehavioursCount; i++)
            {
                var b = _postLateUpdateBehaviours[i];
#if NEWBIECOMMONS_ENABLE_UPDATER_NULL_CHECK
                if (b == null)
                {
                    Debug.Log($"[UpdateManager] PostLateUpdate {i} is null");
                    continue;
                }
#endif

                b.SendCustomEvent("_PostLateUpdate");
            }

            if (_slowPostLateUpdateBehavioursCount != 0)
            {
                ++_lastCalledSlowPostLateUpdateIndex;
                if (_lastCalledSlowPostLateUpdateIndex >= _slowPostLateUpdateBehavioursCount)
                    _lastCalledSlowPostLateUpdateIndex = 0;

                var b = _slowPostLateUpdateBehaviours[_lastCalledSlowPostLateUpdateIndex];
#if NEWBIECOMMONS_ENABLE_UPDATER_NULL_CHECK
                if (b == null)
                {
                    Debug.Log($"[UpdateManager] SlowPostLateUpdate {_lastCalledSlowPostLateUpdateIndex} is null");
                    return;
                }
#endif

                b.SendCustomEvent("_SlowPostLateUpdate");
            }
        }

        #endregion

        #region Internals

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

        #endregion

        #region PublicAPI

        /// <summary>
        /// Subscribes <paramref name="behaviour"/> to <c>_Update</c> event call
        /// </summary>
        /// <param name="behaviour">A behaviour which will subscribe to the event</param>
        [PublicAPI]
        public void SubscribeUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            _AddBehaviour(_updateBehavioursCount++, behaviour, _updateBehaviours, out _updateBehaviours);
        }

        /// <summary>
        /// Unsubscribes <paramref name="behaviour"/> from <c>_Update</c> event call
        /// </summary>
        /// <param name="behaviour">A behaviour which will unsubscribe from the event</param>
        [PublicAPI]
        public void UnsubscribeUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            if (_RemoveBehaviour(behaviour, _updateBehaviours, out _updateBehaviours))
                --_updateBehavioursCount;
        }

        /// <summary>
        /// Subscribes <paramref name="behaviour"/> to <c>_SlowUpdate</c> event call
        /// </summary>
        /// <remarks>
        /// This event will called one-by-one every <c>_Update</c> call.
        /// meaning <see cref="Time.deltaTime"/> will not work. 
        /// </remarks>
        /// <param name="behaviour">A behaviour which will subscribe to the event</param>
        [PublicAPI]
        public void SubscribeSlowUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            _AddBehaviour(_slowUpdateBehavioursCount++, behaviour, _slowUpdateBehaviours, out _slowUpdateBehaviours);
        }

        /// <summary>
        /// Unsubscribes <paramref name="behaviour"/> from <c>_SlowUpdate</c> event call
        /// </summary>
        /// <param name="behaviour">A behaviour which will unsubscribe from the event</param>
        [PublicAPI]
        public void UnsubscribeSlowUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            if (_RemoveBehaviour(behaviour, _slowUpdateBehaviours, out _slowUpdateBehaviours))
                --_slowUpdateBehavioursCount;
        }

        /// <summary>
        /// Subscribes <paramref name="behaviour"/> to <c>_PostLateUpdate</c> event call
        /// </summary>
        /// <param name="behaviour">A behaviour which will subscribe to the event</param>
        [PublicAPI]
        public void SubscribePostLateUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            _AddBehaviour(_postLateUpdateBehavioursCount++, behaviour, _postLateUpdateBehaviours,
                out _postLateUpdateBehaviours);
        }

        /// <summary>
        /// Unsubscribes <paramref name="behaviour"/> from <c>_PostLateUpdate</c> event call
        /// </summary>
        /// <param name="behaviour">A behaviour which will unsubscribe from the event</param>
        [PublicAPI]
        public void UnsubscribePostLateUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            if (_RemoveBehaviour(behaviour, _postLateUpdateBehaviours, out _postLateUpdateBehaviours))
                --_postLateUpdateBehavioursCount;
        }

        /// <summary>
        /// Subscribes <paramref name="behaviour"/> to <c>_PostLateUpdate</c> event call
        /// </summary>
        /// <remarks>
        /// This event will called one-by-one every <c>_PostLateUpdate</c> call.
        /// meaning <see cref="Time.deltaTime"/> will not work. 
        /// </remarks>
        /// <param name="behaviour">A behaviour which will subscribe to the event</param>
        [PublicAPI]
        public void SubscribeSlowPostLateUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            _AddBehaviour(_slowPostLateUpdateBehavioursCount++, behaviour, _slowPostLateUpdateBehaviours,
                out _slowPostLateUpdateBehaviours);
        }

        /// <summary>
        /// Unsubscribes <paramref name="behaviour"/> from <c>_SlowLateUpdate</c> event call
        /// </summary>
        /// <param name="behaviour">A behaviour which will unsubscribe from the event</param>
        [PublicAPI]
        public void UnsubscribeSlowLateUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            if (_RemoveBehaviour(behaviour, _slowPostLateUpdateBehaviours, out _slowPostLateUpdateBehaviours))
                --_slowPostLateUpdateBehavioursCount;
        }

        /// <summary>
        /// Subscribes <paramref name="behaviour"/> to <c>_FixedUpdate</c> event call
        /// </summary>
        /// <param name="behaviour">A behaviour which will subscribe to the event</param>
        [PublicAPI]
        public void SubscribeFixedUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            _AddBehaviour(_fixedUpdateBehavioursCount++, behaviour, _fixedUpdateBehaviours, out _fixedUpdateBehaviours);
        }

        /// <summary>
        /// Unsubscribes <paramref name="behaviour"/> from <c>_FixedUpdate</c> event call
        /// </summary>
        /// <param name="behaviour">A behaviour which will unsubscribe from the event</param>
        [PublicAPI]
        public void UnsubscribeFixedUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            if (_RemoveBehaviour(behaviour, _fixedUpdateBehaviours, out _fixedUpdateBehaviours))
                --_fixedUpdateBehavioursCount;
        }

        /// <summary>
        /// Subscribes <paramref name="behaviour"/> to <c>_SlowFixedUpdate</c> event call
        /// </summary>
        /// <remarks>
        /// This event will called one-by-one every <c>_SlowFixedUpdate</c> call.
        /// meaning <see cref="Time.deltaTime"/> will not work. 
        /// </remarks>
        /// <param name="behaviour">A behaviour which will subscribe to the event</param>
        [PublicAPI]
        public void SubscribeSlowFixedUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            _AddBehaviour(_slowFixedUpdateBehavioursCount++, behaviour, _slowFixedUpdateBehaviours,
                out _slowFixedUpdateBehaviours);
        }

        /// <summary>
        /// Unsubscribes <paramref name="behaviour"/> from <c>_SlowFixedUpdate</c> event call
        /// </summary>
        /// <param name="behaviour">A behaviour which will unsubscribe from the event</param>
        [PublicAPI]
        public void UnsubscribeSlowFixedUpdate(UdonSharpBehaviour behaviour)
        {
            _EnsureInit();
            if (behaviour == null)
                return;

            if (_RemoveBehaviour(behaviour, _slowFixedUpdateBehaviours, out _slowFixedUpdateBehaviours))
                --_slowFixedUpdateBehavioursCount;
        }

        #endregion
    }
}