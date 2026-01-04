using DG.Tweening;
using TMPro;
using UnityEngine;
using SerapKeremGameKit._Audio;
using SerapKeremGameKit._Haptics;

namespace SerapKeremGameKit._UI
{
    public sealed class CoinFlyAnimator : MonoBehaviour
    {
        [SerializeField] private float _spawnDuration = 0.18f;
        [SerializeField] private float _moveDuration = 0.35f;
        [SerializeField] private float _spawnRadius = 90f;
        [SerializeField] private float _delayStep = 0.03f;

		[Header("Audio/Haptics Keys")]
		[SerializeField] private string _coinTickKey = "coin_tick";
		[SerializeField] private HapticType _coinTickHaptic = HapticType.Light;

        public Sequence AnimateAdd(TextMeshProUGUI totalText, RectTransform source, RectTransform target, long startAmount, long addAmount)
        {
            if (totalText == null || source == null || target == null || CoinPool.Instance == null)
                return null;

            int iconCount = Mathf.Clamp((int)addAmount, 2, 12);
            long end = startAmount + addAmount;

			var master = DOTween.Sequence()
				.SetAutoKill(true)
				.SetLink(gameObject, LinkBehaviour.KillOnDestroy);

            Vector3 originalScale = target.localScale;

			for (int i = 0; i < iconCount; i++)
            {
                RectTransform icon = CoinPool.Instance.Spawn();
                icon.SetParent(target.parent, worldPositionStays: false);
                icon.position = source.position;

                float t = i / (float)(iconCount - 1);
                Vector3 burstOffset = (Vector3)(Random.insideUnitCircle * _spawnRadius);

                // Burst out
				var s1 = icon.DOMove(source.position + burstOffset, _spawnDuration)
					.SetEase(Ease.InOutSine)
					.SetDelay(i * _delayStep)
					.SetAutoKill(true)
					.SetLink(icon.gameObject, LinkBehaviour.KillOnDisable | LinkBehaviour.KillOnDestroy);

                // Fly to target
				var s2 = icon.DOMove(target.position, _moveDuration)
					.SetEase(Ease.InBack)
					.SetAutoKill(true)
					.SetLink(icon.gameObject, LinkBehaviour.KillOnDisable | LinkBehaviour.KillOnDestroy)
                    .OnComplete(() =>
                    {
                        long stepValue = (long)Mathf.Ceil(Mathf.Lerp(startAmount, end, t));
                        totalText.text = stepValue.ToString();
                        target.DOKill();
                        target.localScale = originalScale;
						target.DOScale(originalScale * 1.04f, 0.08f)
							.SetEase(Ease.OutQuad)
							.SetLoops(2, LoopType.Yoyo)
							.SetAutoKill(true)
							.SetLink(target.gameObject, LinkBehaviour.KillOnDestroy)
							.OnComplete(() =>
                        {
                            target.localScale = originalScale;
                        });
						if (AudioManager.IsInitialized && !string.IsNullOrEmpty(_coinTickKey)) AudioManager.Instance.Play(_coinTickKey);
						if (HapticManager.IsInitialized && _coinTickHaptic != HapticType.None) HapticManager.Instance.Play(_coinTickHaptic);
                        CoinPool.Instance.Despawn(icon);
                    });

				var coinSeq = DOTween.Sequence()
					.SetAutoKill(true)
					.SetLink(icon.gameObject, LinkBehaviour.KillOnDisable | LinkBehaviour.KillOnDestroy);
                coinSeq.Append(s1);
                coinSeq.Append(s2);
                master.Insert(0f, coinSeq);
            }

			master.Append(DOVirtual.DelayedCall(iconCount * _delayStep + _spawnDuration + _moveDuration, () =>
            {
                totalText.text = end.ToString();
                target.localScale = originalScale;
			}).SetAutoKill(true).SetLink(gameObject, LinkBehaviour.KillOnDestroy));

            return master;
        }
    }
}


