using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)] [DefaultExecutionOrder(0)]
    public class TranslatableMessage : UdonSharpBehaviour
    {
        [SerializeField] [InspectorName("ja-JP Message")] [TextArea(3, 10)]
        private string jaJpMessage;
        [SerializeField] [InspectorName("en-US Message")] [TextArea(3, 10)]
        private string enUsMessage;

        public string Message => NewbieUtils.IsJapaneseTimeZone() ? jaJpMessage : enUsMessage;
    }
}