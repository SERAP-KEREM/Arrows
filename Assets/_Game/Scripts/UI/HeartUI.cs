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
        [SerializeField] private Image _heartImage;

        private bool _isActive = true;

        private void Awake()
        {
            if (_heartImage == null)
            {
                _heartImage = GetComponent<Image>();
            }

            if (_heartImage == null)
            {
                Debug.LogWarning($"{name}: Image component is not found. Please assign it in Inspector or add Image component to this GameObject.", this);
            }

            if (_redHeartSprite == null || _grayHeartSprite == null)
            {
                Debug.LogWarning($"{name}: Red or Gray heart sprite is not assigned in Inspector.", this);
            }
        }

        public void SetActive(bool active)
        {
            _isActive = active;

            if (_heartImage != null)
            {
                if (active)
                {
                    if (_redHeartSprite != null)
                    {
                        _heartImage.sprite = _redHeartSprite;
                    }
                }
                else
                {
                    if (_grayHeartSprite != null)
                    {
                        _heartImage.sprite = _grayHeartSprite;
                    }
                }
            }
        }

        public void Initialize()
        {
            SetActive(true);
        }

    }
}
