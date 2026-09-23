using System.Collections;
using System.Collections.Generic;
using ArrowNook.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ArrowNook.UI
{
    /// <summary>
    /// Applies the reference-app win-screen aesthetic to the existing win/results panel:
    /// • Blue radial-sunburst gradient background
    /// • Colorful confetti particles
    /// • "Next Level" → white pill button with blue text
    /// • "Main Menu" → plain white underlined text
    ///
    /// SETUP (Inspector):
    ///   1. Add this component to your win-panel root (the GameObject that becomes active on win).
    ///   2. Assign <see cref="NextLevelButton"/> and <see cref="MainMenuButton"/>.
    ///   3. Assign <see cref="TitleLabel"/> (shows "Level Completed!").
    ///   4. (Optional) Assign <see cref="ConfettiRoot"/> — a child RectTransform to spawn confetti in.
    ///
    /// The sunburst is drawn by creating a full-screen Image child using a radial gradient texture
    /// generated at runtime — no extra PNG needed.
    /// </summary>
    public sealed class WinScreenTheme : MonoBehaviour
    {
        [Header("Required References")]
        [SerializeField] private Button NextLevelButton;
        [SerializeField] private Button MainMenuButton;
        [SerializeField] private TMP_Text TitleLabel;

        [Header("Optional")]
        [Tooltip("Parent RectTransform for confetti pieces. Leave null to use this transform.")]
        [SerializeField] private RectTransform ConfettiRoot;
        [SerializeField] private int ConfettiCount = 40;
        [SerializeField] private float ConfettiFallDuration = 2.5f;

        // ── State ──────────────────────────────────────────────────────────────
        private readonly List<GameObject> _confettiPieces = new List<GameObject>();
        private Image _backgroundImage;
        private Texture2D _sunburstTexture;

        // ── Unity lifecycle ────────────────────────────────────────────────────

        private void Awake()
        {
            BuildBackground();
            StyleButtons();
            StyleTitle();
        }

        private void OnEnable()
        {
            StartCoroutine(SpawnConfetti());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            ClearConfetti();
        }

        private void OnDestroy()
        {
            if (_sunburstTexture != null) Destroy(_sunburstTexture);
        }

        // ── Background ─────────────────────────────────────────────────────────

        private void BuildBackground()
        {
            // Insert a full-stretch Image as the first child so it sits behind everything.
            var bgGO = new GameObject("WinBackground", typeof(RectTransform), typeof(Image));
            bgGO.transform.SetParent(transform, false);
            bgGO.transform.SetAsFirstSibling();

            var rt = bgGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            _backgroundImage = bgGO.GetComponent<Image>();
            _backgroundImage.color = ArrowNookTheme.WinBackground;
            _backgroundImage.raycastTarget = false;

            // Generate a simple radial sunburst texture.
            _sunburstTexture = BuildSunburstTexture(256, 256, 16);
            _backgroundImage.sprite = Sprite.Create(
                _sunburstTexture,
                new Rect(0, 0, 256, 256),
                new Vector2(0.5f, 0.5f));
            _backgroundImage.type = Image.Type.Simple;
            _backgroundImage.preserveAspect = false;
        }

        private Texture2D BuildSunburstTexture(int w, int h, int rays)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                name = "WinSunburst"
            };

            Color32 center = ArrowNookTheme.WinBackground;
            Color32 ray = ArrowNookTheme.WinRay;
            Color32[] pixels = new Color32[w * h];
            float cx = w * 0.5f, cy = h * 0.5f;
            float twoPi = Mathf.PI * 2f;
            float sliceAngle = twoPi / (rays * 2f);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float angle = Mathf.Atan2(y - cy, x - cx); // -π .. π
                    angle = (angle + twoPi) % twoPi;            // 0 .. 2π
                    float slice = angle / sliceAngle;
                    bool inRay = ((int)slice % 2) == 0;
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    float falloff = Mathf.Clamp01(dist / (w * 0.5f));
                    // Blend ray color slightly, fading toward edges.
                    pixels[y * w + x] = inRay
                        ? Color32.Lerp(ray, center, falloff * 0.4f)
                        : Color32.Lerp(center, center, falloff);
                }
            }

            tex.SetPixels32(pixels);
            tex.Apply(false, true);
            return tex;
        }

        // ── Buttons ────────────────────────────────────────────────────────────

        private void StyleButtons()
        {
            if (NextLevelButton != null) StylePillButton(NextLevelButton,
                ArrowNookTheme.WinButton, ArrowNookTheme.WinButtonText);

            if (MainMenuButton != null) StyleTextButton(MainMenuButton);
        }

        private static void StylePillButton(Button button, Color bg, Color fg)
        {
            var image = button.GetComponent<Image>();
            if (image != null) image.color = bg;

            var label = button.GetComponentInChildren<TMP_Text>();
            if (label != null)
            {
                label.color = fg;
                label.fontStyle = FontStyles.Bold;
            }
        }

        private static void StyleTextButton(Button button)
        {
            var image = button.GetComponent<Image>();
            if (image != null) image.color = Color.clear; // transparent background

            var label = button.GetComponentInChildren<TMP_Text>();
            if (label != null)
            {
                label.color = Color.white;
                label.fontStyle = FontStyles.Underline | FontStyles.Bold;
            }
        }

        // ── Title label ────────────────────────────────────────────────────────

        private void StyleTitle()
        {
            if (TitleLabel == null) return;
            TitleLabel.color = Color.white;
            TitleLabel.fontStyle = FontStyles.Bold;
        }

        // ── Confetti ───────────────────────────────────────────────────────────

        private IEnumerator SpawnConfetti()
        {
            yield return null; // Wait one frame for layout.
            ClearConfetti();

            RectTransform root = ConfettiRoot != null
                ? ConfettiRoot
                : (RectTransform)transform;

            Rect rootRect = root.rect;

            for (int i = 0; i < ConfettiCount; i++)
            {
                SpawnPiece(root, rootRect);
                yield return new WaitForSecondsRealtime(ConfettiFallDuration / ConfettiCount);
            }
        }

        private void SpawnPiece(RectTransform root, Rect rootRect)
        {
            var go = new GameObject("Confetti", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(root, false);

            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(
                Random.Range(8f, 20f),
                Random.Range(5f, 14f));
            rt.anchorMin = rt.anchorMax = new Vector2(Random.value, 1.05f);
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

            var img = go.GetComponent<Image>();
            img.color = ArrowNookTheme.ConfettiColors[Random.Range(0, ArrowNookTheme.ConfettiColors.Length)];
            img.raycastTarget = false;

            _confettiPieces.Add(go);
            StartCoroutine(AnimatePiece(rt, rootRect));
        }

        private IEnumerator AnimatePiece(RectTransform rt, Rect rootRect)
        {
            float elapsed = 0f;
            float duration = Random.Range(ConfettiFallDuration * 0.7f, ConfettiFallDuration * 1.3f);
            float startY = rootRect.height * 0.05f;
            float endY = -rootRect.height * 1.1f;
            float startX = rt.anchoredPosition.x;
            float swayAmp = Random.Range(20f, 60f);
            float swayFreq = Random.Range(1.5f, 3f);
            float startAngle = rt.localRotation.eulerAngles.z;
            float spinSpeed = Random.Range(-180f, 180f);

            while (elapsed < duration)
            {
                if (rt == null) yield break;
                float t = elapsed / duration;
                float y = Mathf.Lerp(startY, endY, t);
                float x = startX + Mathf.Sin(elapsed * swayFreq) * swayAmp;
                rt.anchoredPosition = new Vector2(x, y);
                rt.localRotation = Quaternion.Euler(0, 0, startAngle + spinSpeed * elapsed);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        private void ClearConfetti()
        {
            foreach (GameObject p in _confettiPieces)
                if (p != null) Destroy(p);
            _confettiPieces.Clear();
        }
    }
}
