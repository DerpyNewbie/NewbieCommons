using System;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ProximityUpdateManager : UdonSharpBehaviour
    {
        [SerializeField]
        private float maxUpdateProximity = 30F;
        [SerializeField] [Range(1, 30)]
        private int maxPartialSortCount = 4;
        [SerializeField] [ItemNotNull]
        private ProximityUpdateReceiver[] receivers;
        private float[] _cachedDistances;

        private VRCPlayerApi _localPlayer;

        private float _partialSortPreviousDistance;
        private int _partialSortIndex;

        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;
            _cachedDistances = new float[receivers.Length];
            for (int i = 0; i < _cachedDistances.Length; i++)
                _cachedDistances[i] = float.PositiveInfinity;
        }

        #region PublicAPI

        /// <summary>
        /// Subscribes to ProximityUpdateManager updates.
        /// </summary>
        /// <param name="receiver">Receiver to subscribe</param>
        /// <returns>true if subscribed, false if already subscribed.</returns>
        [PublicAPI]
        public bool SubscribeReceiver(ProximityUpdateReceiver receiver)
        {
            receivers = receivers.AddAsSet(receiver, out var result);
            if (receivers.Length > _cachedDistances.Length)
            {
                var arr = new float[_cachedDistances.Length + 64];
                Array.Copy(_cachedDistances, arr, _cachedDistances.Length);
                _cachedDistances = arr;
                for (int i = _cachedDistances.Length - 64; i < _cachedDistances.Length; i++)
                    _cachedDistances[i] = float.PositiveInfinity;
            }

            return result;
        }

        /// <summary>
        /// Unsubscribes from ProximityUpdateManager updates.
        /// </summary>
        /// <param name="receiver">Receiver to unsubscribe</param>
        /// <returns>true if unsubscribed, false if already unsubscribed.</returns>
        [PublicAPI]
        public bool UnsubscribeReceiver(ProximityUpdateReceiver receiver)
        {
            receivers = receivers.RemoveItem(receiver, out var result);
            return result;
        }

        /// <summary>
        /// Calculates distance between local player to <paramref name="receiver"/>.
        /// </summary>
        /// <param name="receiver">Receiver to calculate distance</param>
        /// <param name="applyResultImmediately">If true, result will be applied immediately to cache</param>
        /// <returns></returns>
        [PublicAPI]
        public float CalculateDistance(ProximityUpdateReceiver receiver, bool applyResultImmediately = false)
        {
            var distance = Vector3.Distance(receiver.Position, _localPlayer.GetPosition());
            if (applyResultImmediately)
            {
                var index = receivers.FindItem(receiver);
                if (index != -1)
                    _cachedDistances[index] = distance;
            }

            return distance;
        }

        /// <summary>
        /// Retrieves cached distance for <paramref name="receiver"/>.
        /// </summary>
        /// <returns><c>true</c> if retrieved, <c>false</c> otherwise.</returns>
        [PublicAPI]
        public bool CachedDistance(ProximityUpdateReceiver receiver, out float cachedDistance)
        {
            if (receiver == null)
            {
                cachedDistance = float.NaN;
                return false;
            }

            var index = receivers.FindItem(receiver);
            if (index == -1)
            {
                cachedDistance = float.NaN;
                return false;
            }

            cachedDistance = _cachedDistances[index];
            return true;
        }

        #endregion

        #region UpdateCalls

        private void Update()
        {
            DoPartialSort();
            SendAllUpdate(UpdateTiming.Update);
        }

        private void FixedUpdate()
        {
            SendAllUpdate(UpdateTiming.FixedUpdate);
        }

        private void LateUpdate()
        {
            SendAllUpdate(UpdateTiming.LateUpdate);
        }

        public override void PostLateUpdate()
        {
            SendAllUpdate(UpdateTiming.PostLateUpdate);
        }

        #endregion

        #region Internals

        private void DoPartialSort()
        {
            var localPlayerPos = _localPlayer.GetPosition();
            if (receivers.Length == 0)
                return;

            for (int i = 0; i < maxPartialSortCount; i++)
            {
                var receiver = receivers[_partialSortIndex];
                var lastDistance = _cachedDistances[_partialSortIndex];
                var distance = Vector3.Distance(localPlayerPos, receiver.Position);
                var previousIndex = _partialSortIndex + 1;
                if (distance > _partialSortPreviousDistance && previousIndex < receivers.Length)
                {
                    receivers[_partialSortIndex] = receivers[previousIndex];
                    _cachedDistances[_partialSortIndex] = _cachedDistances[previousIndex];
                    receivers[previousIndex] = receiver;
                    _cachedDistances[previousIndex] = distance;
                }
                else
                {
                    _cachedDistances[_partialSortIndex] = distance;
                }

                if (lastDistance < maxUpdateProximity && distance > maxUpdateProximity)
                {
                    receiver.ProximityExit();
                }
                else if (lastDistance > maxUpdateProximity && distance < maxUpdateProximity)
                {
                    receiver.ProximityEnter();
                }

                _partialSortPreviousDistance = distance;
                --_partialSortIndex;

                // TODO: possibly breaking last element of receivers array, ProximityEnter/Exit may not be called?
                if (_partialSortIndex < 0)
                {
                    _partialSortIndex = receivers.Length - 1;
                    _partialSortPreviousDistance = _cachedDistances[_partialSortIndex];
                }
            }
        }

        private void SendAllUpdate(UpdateTiming timing)
        {
            for (int i = 0; i < receivers.Length; i++)
            {
                var distance = _cachedDistances[i];
                if (distance > maxUpdateProximity)
                    break;
                SendUpdate(receivers[i], distance, timing);
            }
        }

        private static void SendUpdate([NotNull] ProximityUpdateReceiver receiver, float distance, UpdateTiming timing)
        {
            switch (timing)
            {
                case UpdateTiming.Update:
                    receiver.ProximityUpdate(distance);
                    break;
                case UpdateTiming.LateUpdate:
                    receiver.ProximityLateUpdate(distance);
                    break;
                case UpdateTiming.PostLateUpdate:
                    receiver.ProximityPostLateUpdate(distance);
                    break;
                case UpdateTiming.FixedUpdate:
                    receiver.ProximityFixedUpdate(distance);
                    break;
            }
        }

        #endregion
    }

    public enum UpdateTiming
    {
        Update,
        LateUpdate,
        PostLateUpdate,
        FixedUpdate
    }

    public abstract class ProximityUpdateReceiver : UdonSharpBehaviour
    {
        [SerializeField] [HideInInspector] [NewbieInject]
        protected ProximityUpdateManager proximityUpdateManager;

        /// <summary>
        /// The current position of this receiver
        /// This should be calculated every call, Should not be cached.
        /// </summary>
        public abstract Vector3 Position { get; }

        /// <summary>
        /// Called when this receiver has entered proximity range.
        /// </summary>
        /// <remarks>
        /// Call timing is Unity's Update time frame.
        /// </remarks>
        public virtual void ProximityEnter()
        {
        }

        /// <summary>
        /// Called when this receiver has exited proximity range.
        /// </summary>
        /// <remarks>
        /// Call timing is Unity's Update time frame.
        /// </remarks>
        public virtual void ProximityExit()
        {
        }

        /// <summary>
        /// Called every frame while within range of <see cref="ProximityUpdateManager.maxUpdateProximity"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="cachedDistance"/> may not equal calculated distance, due to cached result.
        /// <paramref name="cachedDistance"/> will not exceed <see cref="ProximityUpdateManager.maxUpdateProximity"/>.
        /// </remarks>
        /// <param name="cachedDistance">Cached distance from local player</param>
        public virtual void ProximityUpdate(float cachedDistance)
        {
        }

        /// <summary>
        /// Called every frame later than <see cref="ProximityUpdate"/> while within range of <see cref="ProximityUpdateManager.maxUpdateProximity"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="cachedDistance"/> may not equal calculated distance, due to cached result.
        /// <paramref name="cachedDistance"/> will not exceed <see cref="ProximityUpdateManager.maxUpdateProximity"/>.
        /// </remarks>
        /// <param name="cachedDistance">Cached distance from local player</param>
        public virtual void ProximityLateUpdate(float cachedDistance)
        {
        }

        /// <summary>
        /// Called every frame later than <see cref="ProximityLateUpdate"/> while within range of <see cref="ProximityUpdateManager.maxUpdateProximity"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="cachedDistance"/> may not equal calculated distance, due to cached result.
        /// <paramref name="cachedDistance"/> will not exceed <see cref="ProximityUpdateManager.maxUpdateProximity"/>.
        /// </remarks>
        /// <param name="cachedDistance">Cached distance from local player</param>
        public virtual void ProximityPostLateUpdate(float cachedDistance)
        {
        }

        /// <summary>
        /// Called when physics update while within range of <see cref="ProximityUpdateManager.maxUpdateProximity"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="cachedDistance"/> may not equal calculated distance, due to cached result.
        /// <paramref name="cachedDistance"/> can exceed <see cref="ProximityUpdateManager.maxUpdateProximity"/>.
        /// </remarks>
        /// <param name="cachedDistance">Cached distance from local player</param>
        public virtual void ProximityFixedUpdate(float cachedDistance)
        {
        }

        protected virtual void OnDestroy()
        {
            proximityUpdateManager.UnsubscribeReceiver(this);
        }
    }
}