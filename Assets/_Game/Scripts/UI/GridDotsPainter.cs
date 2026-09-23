using ArrowNook.UI;
using UnityEngine;

namespace ArrowNook.UI
{
    /// <summary>
    /// Paints a subtle, evenly-spaced dot grid onto a RawImage or Camera background
    /// to match the reference app's puzzle-board aesthetic.
    ///
    /// SETUP (Inspector):
    ///   1. Create a UI > Raw Image inside your Canvas, stretch it to fill the screen.
    ///   2. Add this component to that RawImage GameObject.
    ///   3. Optionally tweak <see cref="Columns"/>, <see cref="Rows"/>, and <see cref="DotRadius"/>.
    ///
    /// The texture is generated entirely in code — no imported PNG required.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.UI.RawImage))]
    public sealed class GridDotsPainter : MonoBehaviour
    {
        [Tooltip("Number of dot columns across the full texture.")]
        [SerializeField] private int Columns = 10;

        [Tooltip("Number of dot rows in the full texture.")]
        [SerializeField] private int Rows = 18;

        [Tooltip("Dot radius in texture pixels (texture is 200×360 px internally).")]
        [SerializeField] private int DotRadius = 5;

        [Tooltip("Dot color. Defaults to ArrowNookTheme.GridDot if left at default.")]
        [SerializeField] private Color DotColor = default;

        [Tooltip("Background fill color. Defaults to ArrowNookTheme.Background if left at default.")]
        [SerializeField] private Color BackgroundColor = default;

        private UnityEngine.UI.RawImage _rawImage;
        private Texture2D _texture;

        private void Awake()
        {
            _rawImage = GetComponent<UnityEngine.UI.RawImage>();

            // Fall back to theme colors when Inspector fields are left at default (clear).
            if (DotColor == default || DotColor == Color.clear)
                DotColor = ArrowNookTheme.GridDot;
            if (BackgroundColor == default || BackgroundColor == Color.clear)
                BackgroundColor = ArrowNookTheme.Background;

            RebuildTexture();
        }

        private void OnValidate() => RebuildTexture(); // Live preview in Editor.

        private void RebuildTexture()
        {
            if (_rawImage == null) _rawImage = GetComponent<UnityEngine.UI.RawImage>();
            if (_rawImage == null) return;

            int w = Mathf.Max(2, Columns) * 20;
            int h = Mathf.Max(2, Rows) * 20;

            if (_texture != null) Destroy(_texture);
            _texture = new Texture2D(w, h, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                name = "GridDotsTex"
            };

            // Fill background.
            Color32 bg = BackgroundColor;
            Color32[] pixels = new Color32[w * h];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = bg;

            // Paint dots.
            float cellW = (float)w / Columns;
            float cellH = (float)h / Rows;
            Color32 dot = DotColor;
            float r2 = DotRadius * DotRadius;

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    int cx = Mathf.RoundToInt(col * cellW + cellW * 0.5f);
                    int cy = Mathf.RoundToInt(row * cellH + cellH * 0.5f);

                    // Anti-aliased circle.
                    for (int dy = -DotRadius - 1; dy <= DotRadius + 1; dy++)
                    {
                        for (int dx = -DotRadius - 1; dx <= DotRadius + 1; dx++)
                        {
                            int px = cx + dx;
                            int py = cy + dy;
                            if (px < 0 || px >= w || py < 0 || py >= h) continue;
                            float dist2 = dx * dx + dy * dy;
                            float alpha = Mathf.Clamp01(DotRadius + 0.5f - Mathf.Sqrt(dist2));
                            if (alpha <= 0f) continue;
                            Color32 existing = pixels[py * w + px];
                            pixels[py * w + px] = Color32.Lerp(existing, dot, alpha * dot.a / 255f);
                        }
                    }
                }
            }

            _texture.SetPixels32(pixels);
            _texture.Apply(false, false);
            _rawImage.texture = _texture;
        }

        private void OnDestroy()
        {
            if (_texture != null) Destroy(_texture);
        }
    }
}
