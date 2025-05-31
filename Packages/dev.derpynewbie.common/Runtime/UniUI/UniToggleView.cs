using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace DerpyNewbie.Common.UniUI
{
    [AddComponentMenu("Newbie Commons/UniUI/UniToggleView")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UniToggleView : UniUI
    {
        [SerializeField] [NewbieInject(SearchScope.Parents)]
        private Toggle toggle;

        [SerializeField]
        private float transitionTime = 0.1F;

        [SerializeField]
        private Color onColor = new Color(0.25f, 1f, 0.3f);

        [SerializeField]
        private Color offColor = new Color(1, 0.2971698F, 0.2971698F, 1);

        [SerializeField]
        private Image nodeImage;

        [SerializeField]
        private RectTransform knobTransform;

        private Vector2 _minAnchoredPos;
        private Vector2 _maxAnchoredPos;
        private float _progress;
        private float _timer;
        private float _velocity;

        private void Start()
        {
            _minAnchoredPos = knobTransform.anchoredPosition;
            _maxAnchoredPos = new Vector2(_minAnchoredPos.x * -1, _minAnchoredPos.y);
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            _progress = Mathf.Clamp01(_timer / transitionTime);

            var nextColor = toggle.isOn ? onColor : offColor;
            var prevColor = toggle.isOn ? offColor : onColor;

            var nextPos = toggle.isOn ? _minAnchoredPos : _maxAnchoredPos;
            var prevPos = toggle.isOn ? _maxAnchoredPos : _minAnchoredPos;

            var lerpColor = Color.Lerp(prevColor, nextColor, _progress);
            var lerpPos = Vector2.Lerp(prevPos, nextPos, _progress);

            knobTransform.anchoredPosition = lerpPos;
            nodeImage.color = lerpColor;

            if (Mathf.Approximately(_progress, 1))
            {
                gameObject.SetActive(false);
            }
        }

        public override void OnUniUIUpdate()
        {
            _timer = 0;
            gameObject.SetActive(true);
        }
    }
}