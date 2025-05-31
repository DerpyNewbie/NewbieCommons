using System;
using JetBrains.Annotations;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace DerpyNewbie.Common.UniUI
{
    [RequireComponent(typeof(TMP_Text))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UniSliderView : UniUI
    {
        [SerializeField] [NewbieInject(SearchScope.Parents)]
        private Slider slider;

        [SerializeField] [NewbieInject(SearchScope.Self)]
        private TMP_Text valueText;

        [SerializeField]
        private string valueFormat = "{0:F0}%";

        [SerializeField]
        private Gradient textGradient;

        private void Start()
        {
            OnUniUIUpdate();
        }

        public override void OnUniUIUpdate()
        {
            valueText.text = string.Format(valueFormat, slider.value);
            valueText.color = textGradient.Evaluate(slider.normalizedValue);
        }
    }
}