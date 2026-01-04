using DG.Tweening;
using UnityEngine;
using SerapKeremGameKit._Audio;
using SerapKeremGameKit._Haptics;

namespace SerapKeremGameKit._UI
{
    public abstract class UIPanel : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected float showDuration = 0.2f;
        [SerializeField] protected float hideDuration = 0.15f;
        [SerializeField] protected Ease showEase = Ease.OutQuad;
        [SerializeField] protected Ease hideEase = Ease.OutQuad;
        [Header("Audio/Haptics Keys")]
        [SerializeField] private string _showKey = "ui_open";
        [SerializeField] private string _hideKey = "ui_close";
        [SerializeField] private HapticType _showHaptic = HapticType.Light;

        private Tween _fadeTween;

        public virtual void Show(bool playSound = true)
        {
            gameObject.SetActive(true);
            _fadeTween?.Kill();
            if (canvasGroup == null) return;
            canvasGroup.alpha = 0f;
            if (playSound && AudioManager.IsInitialized && !string.IsNullOrEmpty(_showKey)) AudioManager.Instance.Play(_showKey);
            if (HapticManager.IsInitialized && _showHaptic != HapticType.None) HapticManager.Instance.Play(_showHaptic);
            _fadeTween = canvasGroup
                .DOFade(1f, showDuration)
                .SetEase(showEase)
                .SetUpdate(true)
                .SetAutoKill(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        public virtual void Hide(bool playSound = true)
        {
            _fadeTween?.Kill();
            if (canvasGroup == null)
            {
                gameObject.SetActive(false);
                return;
            }
            if (playSound && AudioManager.IsInitialized && !string.IsNullOrEmpty(_hideKey)) AudioManager.Instance.Play(_hideKey);
            _fadeTween = canvasGroup
                .DOFade(0f, hideDuration)
                .SetEase(hideEase)
                .SetUpdate(true)
                .SetAutoKill(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }

        protected virtual void OnDestroy()
        {
            _fadeTween?.Kill();
        }

        public void HideImmediate()
        {
            _fadeTween?.Kill();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
            gameObject.SetActive(false);
        }
    }
}
