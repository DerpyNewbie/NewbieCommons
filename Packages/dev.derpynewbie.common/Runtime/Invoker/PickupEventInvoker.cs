﻿using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace DerpyNewbie.Common.Invoker
{
    [AddComponentMenu("Newbie Commons/Invoker/Pickup Event")]
    public class PickupEventInvoker : CommonInvokerBase
    {
        [SerializeField] private PickupEventType eventType = PickupEventType.All;
        [SerializeField] private bool overrideEventAsActualPickupEvent = true;

        private PickupEventType _lastEventType;

        public override void OnPickup()
        {
            if (eventType != PickupEventType.OnPickup && eventType != PickupEventType.All) return;
            
            _lastEventType = PickupEventType.OnPickup;
            Invoke();
        }

        public override void OnPickupUseDown()
        {
            if (eventType != PickupEventType.OnPickupUseDown && eventType != PickupEventType.All) return;
            
            _lastEventType = PickupEventType.OnPickupUseDown;
            Invoke();
        }

        public override void OnPickupUseUp()
        {
            if (eventType != PickupEventType.OnPickupUseUp && eventType != PickupEventType.All) return;
            
            _lastEventType = PickupEventType.OnPickupUseUp;
            Invoke();
        }

        public override void OnDrop()
        {
            if (eventType != PickupEventType.OnDrop && eventType != PickupEventType.All) return;
            
            _lastEventType = PickupEventType.OnDrop;
            Invoke();
        }

        protected override void InternalInvoke()
        {
            if (overrideEventAsActualPickupEvent)
            {
                foreach (var listener in listeners)
                    _SendCallbackEvent(listener, _lastEventType);
            }
            else
            {
                foreach (var listener in listeners)
                    _SendCallbackEvent(listener, eventName);
            }
        }

        private static void _SendCallbackEvent(UdonSharpBehaviour eventCallback, string method)
        {
            if (!Utilities.IsValid(eventCallback))
                return;

            eventCallback.SendCustomEvent(method);
        }

        private static void _SendCallbackEvent(UdonSharpBehaviour eventCallback, PickupEventType type)
        {
            if (!Utilities.IsValid(eventCallback))
                return;

            switch (type)
            {
                case PickupEventType.OnPickup:
                    eventCallback.SendCustomEvent("_onPickup");
                    break;
                case PickupEventType.OnPickupUseDown:
                    eventCallback.SendCustomEvent("_onPickupUseDown");
                    break;
                case PickupEventType.OnPickupUseUp:
                    eventCallback.SendCustomEvent("_onPickupUseUp");
                    break;
                case PickupEventType.OnDrop:
                    eventCallback.SendCustomEvent("_onDrop");
                    break;
                default:
                    Debug.LogError($"[PickupEventSender] Unknown PickupEventType {type}");
                    break;
            }
        }
    }

    public abstract class PickupEventSenderCallback : UdonSharpBehaviour
    {
        public override void OnPickup()
        {
            OnPickupRelayed();
        }

        public override void OnPickupUseDown()
        {
            OnPickupUseDownRelayed();
        }

        public override void OnPickupUseUp()
        {
            OnPickupUseUpRelayed();
        }

        public override void OnDrop()
        {
            OnDropRelayed();
        }

        [PublicAPI]
        public virtual void OnPickupRelayed()
        {
        }

        [PublicAPI]
        public virtual void OnPickupUseDownRelayed()
        {
        }

        [PublicAPI]
        public virtual void OnPickupUseUpRelayed()
        {
        }

        [PublicAPI]
        public virtual void OnDropRelayed()
        {
        }
    }

    public enum PickupEventType
    {
        OnPickup,
        OnPickupUseDown,
        OnPickupUseUp,
        OnDrop,
        All,
    }
}