using System;
using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common
{
    [AddComponentMenu("Newbie Commons/Utils/Translatable Message")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)] 
    [DefaultExecutionOrder(0)]
    [Obsolete("Obsolete from 0.6.0. Use NewbieI18n package instead")]
    public class TranslatableMessage : UdonSharpBehaviour
    {
        [SerializeField] [InspectorName("ja-JP Message")] [TextArea(3, 10)]
        private string jaJpMessage;
        [SerializeField] [InspectorName("en-US Message")] [TextArea(3, 10)]
        private string enUsMessage;

        public string Message => NewbieUtils.IsJapaneseTimeZone() ? jaJpMessage : enUsMessage;
    }
}