using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace DerpyNewbie.Common.UI
{
    public class SelectionButton : UdonSharpBehaviour
    {
        public Image[] images;
        public Color defaultColor;
        public Color selectedColor;
        public Color confirmedColor;

        public int selectedElement
        {
            get => _selectedElement;
            set
            {
                if (_selectedElement != value)
                {
                    _selectedElement = value;
                    Refresh();
                }
            }
        }
        public int confirmedElement
        {
            get => _confirmedElement;
            set
            {
                if (_confirmedElement != value)
                {
                    _confirmedElement = value;
                    Refresh();
                }
            }
        }

        private int _selectedElement = -1;
        private int _confirmedElement = -1;

        public void Refresh()
        {
            foreach (var image in images)
                image.color = defaultColor;

            var selected = GetImage(selectedElement);
            var confirmed = GetImage(confirmedElement);

            if (selected)
                selected.color = selectedColor;
            if (confirmed)
                confirmed.color = confirmedColor;
        }

        public void SetSelectedElementWithoutNotify(int value)
        {
            _selectedElement = value;
        }

        public void SetConfirmedElementWithoutNotify(int value)
        {
            _confirmedElement = value;
        }

        private Image GetImage(int i)
        {
            if (i >= images.Length || i < 0)
                return null;
            return images[i];
        }
    }
}