using ArrowNook.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI
{
    public class HeartUI : MonoBehaviour
    {
        [Header("Heart Sprites")]
        [SerializeField] private Sprite _redHeartSprite;
        [SerializeField] private Sprite _grayHeartSprite;

        [Header("Image Component")]
        private Image _heartImage;

        private bool _isActive = true;
        private bool _isInitialized = false;

        public void SetActive(bool active)
        {
            _isActive = active;

            if (_heartImage == null) return;

            if (active)
            {
                if (_redHeartSprite != null)
                    _heartImage.sprite = _redHeartSprite;
                // Tint with theme color so a single white-heart sprite also works.
                _heartImage.color = ArrowNookTheme.HeartActive;
            }
            else
            {
                if (_grayHeartSprite != null)
                    _heartImage.sprite = _grayHeartSprite;
                _heartImage.color = ArrowNookTheme.HeartEmpty;
            }
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            _heartImage = gameObject.GetComponent<Image>();

            if (_heartImage == null)
            {
                Debug.LogWarning($"{name}: Image component is not found. Please assign it in Inspector.", this);
            }

            if (_redHeartSprite == null && _grayHeartSprite == null)
            {
                Debug.LogWarning($"{name}: No heart sprites assigned — ArrowNookTheme tint will still apply.", this);
            }

            SetActive(true);
            _isInitialized = true;
        }
    }
}

