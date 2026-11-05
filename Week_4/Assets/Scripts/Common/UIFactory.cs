using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace PhysicsDemo
{
    /// <summary>
    /// Builds UGUI elements from code (canvas, panels, text, buttons, sliders, toggles, dropdowns)
    /// so the demo scenes need no hand-made UI prefabs. Uses Unity's built-in LegacyRuntime font.
    /// </summary>
    public static class UIFactory
    {
        public static readonly Color PanelColor = new Color(0.07f, 0.08f, 0.11f, 0.88f);
        public static readonly Color AccentColor = new Color(0.40f, 0.78f, 1f);
        public static readonly Color MutedColor = new Color(0.70f, 0.74f, 0.80f);
        public static readonly Color LiveColor = new Color(0.65f, 1f, 0.65f);
        static readonly Color ButtonColor = new Color(0.20f, 0.25f, 0.34f);

        static Font s_Font;

        /// <summary>Unity's built-in runtime font (always available, no TMP essentials needed).</summary>
        public static Font DefaultFont => s_Font != null ? s_Font : (s_Font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        /// <summary>Screen-space overlay canvas that scales with resolution.</summary>
        public static Canvas CreateCanvas(string name, int sortOrder)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            // Snap UI to whole pixels: text stays sharp instead of being sampled between pixels.
            canvas.pixelPerfect = true;
            // 1280x720 reference: in a small Game view the UI shrinks less, so small text stays readable.
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = 0.5f;
            EnsureEventSystem();
            return canvas;
        }

        /// <summary>UGUI needs one EventSystem; with the new Input System it uses InputSystemUIInputModule.</summary>
        public static void EnsureEventSystem()
        {
            if (UnityEngine.Object.FindAnyObjectByType<EventSystem>() != null) return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        }

        /// <summary>Dark panel that stacks its children vertically and grows to fit them.</summary>
        public static RectTransform CreatePanel(Transform parent, string name, Vector2 anchor, Vector2 anchoredPos, float width)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = new Vector2(width, 100);
            go.GetComponent<Image>().color = PanelColor;
            var layout = go.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 12, 12);
            layout.spacing = 6;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            go.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return rt;
        }

        /// <summary>Invisible container that lays its children out horizontally.</summary>
        public static RectTransform CreateRow(Transform parent, float height, float spacing = 6)
        {
            var go = new GameObject("Row", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            var layout = go.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            go.GetComponent<LayoutElement>().preferredHeight = height;
            return rt;
        }

        /// <summary>Wrapped, rich-text enabled label.</summary>
        public static Text CreateText(Transform parent, string content, int size, Color color,
            FontStyle style = FontStyle.Normal, TextAnchor align = TextAnchor.UpperLeft)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var t = go.GetComponent<Text>();
            t.font = DefaultFont;
            t.fontSize = size;
            t.color = color;
            t.fontStyle = style;
            t.alignment = align;
            t.text = content;
            t.supportRichText = true;
            t.raycastTarget = false;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            return t;
        }

        /// <summary>Flat button with a centred label.</summary>
        public static Button CreateButton(Transform parent, string label, Action onClick, float height = 30, int fontSize = 15)
        {
            var go = new GameObject("Btn_" + label, typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            var img = go.GetComponent<Image>();
            img.color = Color.white;
            var btn = go.GetComponent<Button>();
            btn.targetGraphic = img;
            btn.colors = MakeColors(ButtonColor);
            go.GetComponent<LayoutElement>().preferredHeight = height;
            var t = CreateText(go.transform, label, fontSize, Color.white, FontStyle.Bold, TextAnchor.MiddleCenter);
            Stretch(t.rectTransform);
            if (onClick != null) btn.onClick.AddListener(() => onClick());
            return btn;
        }

        /// <summary>Slider with a "Label: value" caption above it.</summary>
        public static LabeledSlider CreateSlider(Transform parent, string label, float min, float max, float value,
            Action<float> onChange, string format = "0.00")
        {
            var row = new GameObject("Slider_" + label, typeof(RectTransform), typeof(VerticalLayoutGroup));
            row.transform.SetParent(parent, false);
            var vl = row.GetComponent<VerticalLayoutGroup>();
            vl.spacing = 2;
            vl.childControlWidth = vl.childControlHeight = true;
            vl.childForceExpandWidth = true;
            vl.childForceExpandHeight = false;
            var caption = CreateText(row.transform, "", 14, Color.white);

            var sliderGo = new GameObject("Slider", typeof(RectTransform), typeof(LayoutElement));
            sliderGo.transform.SetParent(row.transform, false);
            sliderGo.GetComponent<LayoutElement>().preferredHeight = 16;

            var bg = CreateImage(sliderGo.transform, "Background", new Color(0.18f, 0.2f, 0.25f));
            Stretch(bg.rectTransform);
            var fillArea = new GameObject("Fill Area", typeof(RectTransform)).GetComponent<RectTransform>();
            fillArea.SetParent(sliderGo.transform, false);
            Stretch(fillArea);
            var fill = CreateImage(fillArea, "Fill", AccentColor * 0.8f);
            fill.rectTransform.anchorMin = Vector2.zero;
            fill.rectTransform.anchorMax = new Vector2(0, 1);
            fill.rectTransform.sizeDelta = Vector2.zero;
            var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform)).GetComponent<RectTransform>();
            handleArea.SetParent(sliderGo.transform, false);
            Stretch(handleArea);
            handleArea.offsetMin = new Vector2(7, 0);
            handleArea.offsetMax = new Vector2(-7, 0);
            var handle = CreateImage(handleArea, "Handle", Color.white);
            handle.rectTransform.anchorMin = Vector2.zero;
            handle.rectTransform.anchorMax = new Vector2(0, 1);
            handle.rectTransform.sizeDelta = new Vector2(14, 6);

            var slider = sliderGo.AddComponent<Slider>();
            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = min;
            slider.maxValue = max;
            slider.SetValueWithoutNotify(value);

            var labeled = row.AddComponent<LabeledSlider>();
            labeled.Init(slider, caption, label, format);
            slider.onValueChanged.AddListener(v =>
            {
                labeled.RefreshCaption();
                onChange?.Invoke(v);
            });
            return labeled;
        }

        /// <summary>Checkbox with a label to its right.</summary>
        public static Toggle CreateToggle(Transform parent, string label, bool value, Action<bool> onChange)
        {
            var go = new GameObject("Toggle_" + label, typeof(RectTransform), typeof(Toggle), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            go.GetComponent<LayoutElement>().preferredHeight = 22;
            var box = CreateImage(go.transform, "Box", new Color(0.18f, 0.2f, 0.25f));
            var boxRt = box.rectTransform;
            boxRt.anchorMin = boxRt.anchorMax = new Vector2(0, 0.5f);
            boxRt.pivot = new Vector2(0, 0.5f);
            boxRt.sizeDelta = new Vector2(20, 20);
            var check = CreateImage(boxRt, "Check", AccentColor);
            check.rectTransform.anchorMin = check.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            check.rectTransform.sizeDelta = new Vector2(12, 12);
            var text = CreateText(go.transform, label, 14, Color.white, FontStyle.Normal, TextAnchor.MiddleLeft);
            Stretch(text.rectTransform);
            text.rectTransform.offsetMin = new Vector2(28, 0);

            var toggle = go.GetComponent<Toggle>();
            toggle.targetGraphic = box;
            toggle.graphic = check;
            toggle.SetIsOnWithoutNotify(value);
            if (onChange != null) toggle.onValueChanged.AddListener(v => onChange(v));
            return toggle;
        }

        /// <summary>Dropdown (built with Unity's DefaultControls) with a caption above it.</summary>
        public static Dropdown CreateDropdown(Transform parent, string label, string[] options, int value, Action<int> onChange)
        {
            if (!string.IsNullOrEmpty(label)) CreateText(parent, label, 14, Color.white);
            var go = DefaultControls.CreateDropdown(new DefaultControls.Resources());
            go.name = "Dropdown_" + label;
            go.transform.SetParent(parent, false);
            go.AddComponent<LayoutElement>().preferredHeight = 26;
            foreach (var t in go.GetComponentsInChildren<Text>(true))
            {
                t.font = DefaultFont;
                t.fontSize = 14;
                t.color = new Color(0.1f, 0.1f, 0.12f);
                t.alignment = TextAnchor.MiddleLeft;
                t.verticalOverflow = VerticalWrapMode.Overflow;   // the default rect is too short for 14 px text
            }
            foreach (var img in go.GetComponentsInChildren<Image>(true))
                if (img.name == "Arrow") img.color = new Color(0.35f, 0.4f, 0.5f);
            var dd = go.GetComponent<Dropdown>();
            dd.ClearOptions();
            dd.AddOptions(new System.Collections.Generic.List<string>(options));
            dd.SetValueWithoutNotify(value);
            dd.RefreshShownValue();
            if (onChange != null) dd.onValueChanged.AddListener(v => onChange(v));
            return dd;
        }

        /// <summary>Plain coloured image (no sprite = solid rectangle).</summary>
        public static Image CreateImage(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var img = go.GetComponent<Image>();
            img.color = color;
            return img;
        }

        /// <summary>Makes a RectTransform fill its parent.</summary>
        public static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        static ColorBlock MakeColors(Color baseColor)
        {
            var cb = ColorBlock.defaultColorBlock;
            cb.normalColor = baseColor;
            cb.highlightedColor = Color.Lerp(baseColor, Color.white, 0.25f);
            cb.selectedColor = baseColor;
            cb.pressedColor = Color.Lerp(baseColor, AccentColor, 0.5f);
            return cb;
        }
    }
}
