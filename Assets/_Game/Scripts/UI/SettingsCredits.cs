using ArrowNook.Ads;
using SerapKeremGameKit._UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArrowNook.UI
{
    public sealed class SettingsCredits : MonoBehaviour
    {
        private static readonly Color Ink = new Color32(30, 44, 40, 255);
        private static readonly Color ButtonColor = new Color32(222, 239, 228, 255);
        private RectTransform _panel;
        private TMP_FontAsset _font;
        private Sprite _closeIcon;
        private GameObject _document;
        private Button _privacyChoices;

        public static void Attach(SettingsPanel settings, Button closeButton)
        {
            var credits = settings.gameObject.AddComponent<SettingsCredits>();
            credits.Build(closeButton);
        }

        private void Build(Button closeButton)
        {
            _panel = transform.Find("SettingsPanel") as RectTransform;
            if (_panel == null) return;
            TMP_Text sample = GetComponentInChildren<TMP_Text>(true);
            _font = sample != null ? sample.font : TMP_Settings.defaultFontAsset;
            _closeIcon = closeButton != null ? closeButton.image.sprite : null;
            _panel.sizeDelta = new Vector2(850, 1400);
            var toggles = _panel.Find("SettingsToggles") as RectTransform;
            if (toggles != null)
            {
                toggles.anchorMin = toggles.anchorMax = new Vector2(0.5f, 0.77f);
                toggles.anchoredPosition = Vector2.zero;
                toggles.sizeDelta = new Vector2(650, 400);
            }

            RectTransform band = Rect("About ArrowNook", _panel);
            band.anchorMin = new Vector2(0.06f, 0.035f);
            band.anchorMax = new Vector2(0.94f, 0.535f);
            band.offsetMin = band.offsetMax = Vector2.zero;
            band.gameObject.AddComponent<Image>().color = Color.white;
            Label(band, "ArrowNook: Logic Puzzle", 42, 12, 72);
            Label(band, "Made with love in India\nby Anupam Pradhan, a solo developer", 30, 92, 104);
            RectTransform badge = Rect("Made with Unity", band);
            badge.anchorMin = badge.anchorMax = new Vector2(0.5f, 1f);
            badge.pivot = new Vector2(0.5f, 1f);
            badge.anchoredPosition = new Vector2(0, -245);
            badge.sizeDelta = new Vector2(415, 100);
            var logo = badge.gameObject.AddComponent<Image>();
            logo.sprite = Resources.Load<Sprite>("Branding/MadeWithUnity");
            logo.preserveAspect = true;
            logo.raycastTarget = false;

            Command(band, "Acknowledgements", 400, () => ShowDocument("Acknowledgements", "Branding/Acknowledgements"));
            Command(band, "Privacy", 492, () => ShowDocument("Privacy", "Branding/PrivacyNotice"));
            _privacyChoices = Command(band, "Privacy choices", 584, ShowPrivacyChoices);
        }

        private void LateUpdate()
        {
            if (_panel == null) return;
            var root = (RectTransform)transform;
            Rect safe = Screen.safeArea;
            float width = root.rect.width * safe.width / Mathf.Max(1, Screen.width);
            float height = root.rect.height * safe.height / Mathf.Max(1, Screen.height);
            float scale = Mathf.Min(1f, (width - 60) / 850f, (height - 260) / 1400f);
            _panel.localScale = Vector3.one * Mathf.Max(0.1f, scale);
            _panel.anchoredPosition = new Vector2(
                (safe.center.x / Mathf.Max(1, Screen.width) - 0.5f) * root.rect.width,
                (safe.center.y / Mathf.Max(1, Screen.height) - 0.5f) * root.rect.height);
            if (_privacyChoices != null)
            {
                var ads = ResultsBannerController.Instance;
                _privacyChoices.gameObject.SetActive(ads != null && ads.PrivacyOptionsRequired);
                _privacyChoices.interactable = ads != null && !ads.PrivacyBusy;
            }
        }

        private void ShowPrivacyChoices()
        {
            ResultsBannerController.Instance?.ShowPrivacyOptions(error =>
            {
                if (error != null && this != null) ShowText("Privacy choices", error);
            });
        }

        private void ShowDocument(string title, string resource)
        {
            TextAsset document = Resources.Load<TextAsset>(resource);
            ShowText(title, document != null ? document.text : "This document is unavailable.");
        }

        private void ShowText(string title, string text)
        {
            if (_document != null) Destroy(_document);
            RectTransform page = Rect(title, transform);
            _document = page.gameObject;
            Stretch(page);
            page.gameObject.AddComponent<Image>().color = Color.white;

            RectTransform safe = Rect("Safe area", page);
            safe.anchorMin = new Vector2(Screen.safeArea.xMin / Screen.width, Screen.safeArea.yMin / Screen.height);
            safe.anchorMax = new Vector2(Screen.safeArea.xMax / Screen.width, Screen.safeArea.yMax / Screen.height);
            safe.offsetMin = new Vector2(45, 45);
            safe.offsetMax = new Vector2(-45, -45);
            TMP_Text heading = Label(safe, title, 42, 8, 95);
            heading.rectTransform.offsetMax += new Vector2(-110, 0);

            RectTransform close = Rect("Close document", safe);
            close.anchorMin = close.anchorMax = close.pivot = Vector2.one;
            close.anchoredPosition = new Vector2(-8, -8);
            close.sizeDelta = new Vector2(90, 90);
            var closeImage = close.gameObject.AddComponent<Image>();
            closeImage.sprite = _closeIcon;
            closeImage.preserveAspect = true;
            var closeButton = close.gameObject.AddComponent<Button>();
            closeButton.targetGraphic = closeImage;
            closeButton.onClick.AddListener(() => { Destroy(_document); _document = null; });

            RectTransform viewport = Rect("Document viewport", safe);
            Stretch(viewport);
            viewport.offsetMin = new Vector2(0, 20);
            viewport.offsetMax = new Vector2(0, -130);
            viewport.gameObject.AddComponent<Image>().color = Color.white;
            viewport.gameObject.AddComponent<RectMask2D>();
            var scroll = viewport.gameObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.viewport = viewport;
            RectTransform content = Rect("Document text", viewport);
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = Vector2.one;
            content.pivot = new Vector2(0.5f, 1);
            content.offsetMin = content.offsetMax = Vector2.zero;
            TMP_Text body = content.gameObject.AddComponent<TextMeshProUGUI>();
            Style(body, 30);
            body.alignment = TextAlignmentOptions.TopLeft;
            body.richText = false;
            body.text = text;
            body.margin = new Vector4(10, 8, 18, 32);
            var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.content = content;
            scroll.verticalNormalizedPosition = 1;
        }

        private TMP_Text Label(Transform parent, string text, int size, float top, float height)
        {
            RectTransform rect = Rect(text, parent);
            TopRow(rect, top, height);
            TMP_Text label = rect.gameObject.AddComponent<TextMeshProUGUI>();
            Style(label, size);
            label.text = text;
            return label;
        }

        private void Style(TMP_Text label, int size)
        {
            label.font = _font;
            label.fontSize = size;
            label.color = Ink;
            label.characterSpacing = 0;
            label.alignment = TextAlignmentOptions.Center;
            label.raycastTarget = false;
            label.textWrappingMode = TextWrappingModes.Normal;
        }

        private Button Command(Transform parent, string title, float top, UnityEngine.Events.UnityAction action)
        {
            RectTransform rect = Rect(title, parent);
            TopRow(rect, top, 76);
            var background = rect.gameObject.AddComponent<Image>();
            background.color = ButtonColor;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = background;
            button.onClick.AddListener(action);
            TMP_Text label = Label(rect, title, 32, 0, 76);
            label.margin = new Vector4(16, 0, 16, 0);
            return button;
        }

        private static RectTransform Rect(string name, Transform parent)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            return rect;
        }

        private static void TopRow(RectTransform rect, float top, float height)
        {
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 1);
            rect.offsetMin = new Vector2(24, -top - height);
            rect.offsetMax = new Vector2(-24, -top);
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }

        private void OnDisable()
        {
            if (_document != null) Destroy(_document);
            _document = null;
        }
    }
}
