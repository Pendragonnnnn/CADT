using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PhysicsDemo
{
    /// <summary>
    /// The on-screen info panel shared by every demo scene.
    /// Left panel: title, concept, controls and live values. Right panel: sliders/toggles that
    /// demo scripts add from their Start() method. The whole UI is built from code in Awake.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class DemoHUD : MonoBehaviour
    {
        /// <summary>The HUD of the current scene (set in Awake, before any Start runs).</summary>
        public static DemoHUD Instance { get; private set; }

        [Header("Info panel text")]
        public string title = "Demo";
        [TextArea(2, 8)] public string concept = "";
        [TextArea(2, 8)] public string controls = "";
        [Tooltip("Inspector Labs only: which objects and fields to try editing. Shown at the top of the right panel.")]
        [TextArea(2, 10)] public string tryInInspector = "";

        [Tooltip("Show the Reset / Back buttons and the slow-mo / gizmo status line.")]
        public bool showNavigation = true;

        [Tooltip("Label of the Reset button (it reloads the scene).")]
        public string resetLabel = "Reset (R)";

        Canvas canvas;
        RectTransform infoPanel;
        RectTransform controlPanel;
        Text liveText;
        Text statusText;
        Text hintText;
        readonly List<Func<string>> liveSources = new List<Func<string>>();
        float nextRefresh;
        bool panelsVisible = true;

        /// <summary>Canvas for the HUD panels.</summary>
        public Canvas Canvas => canvas;

        /// <summary>Lower canvas used for world-space labels that follow objects.</summary>
        public Transform LabelLayer { get; private set; }

        void Awake()
        {
            Instance = this;
            Build();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void Build()
        {
            LabelLayer = UIFactory.CreateCanvas("HUD Labels", 0).transform;
            canvas = UIFactory.CreateCanvas("HUD", 10);

            infoPanel = UIFactory.CreatePanel(canvas.transform, "InfoPanel", new Vector2(0, 1), new Vector2(10, -10), 400);
            UIFactory.CreateText(infoPanel, title, 22, UIFactory.AccentColor, FontStyle.Bold);
            if (!string.IsNullOrEmpty(concept)) UIFactory.CreateText(infoPanel, concept, 15, Color.white);
            if (!string.IsNullOrEmpty(controls))
            {
                UIFactory.CreateText(infoPanel, "CONTROLS", 13, UIFactory.MutedColor, FontStyle.Bold);
                UIFactory.CreateText(infoPanel, controls, 13, UIFactory.MutedColor);
            }
            liveText = UIFactory.CreateText(infoPanel, "", 14, UIFactory.LiveColor);
            liveText.gameObject.SetActive(false);

            if (showNavigation)
            {
                statusText = UIFactory.CreateText(infoPanel, "", 13, UIFactory.MutedColor);
                var row = UIFactory.CreateRow(infoPanel, 30);
                UIFactory.CreateButton(row, resetLabel, SceneLoader.Reload).name = "Nav_Reset";
            }

            controlPanel = UIFactory.CreatePanel(canvas.transform, "ControlPanel", new Vector2(1, 1), new Vector2(-10, -10), 300);
            if (!string.IsNullOrEmpty(tryInInspector))
            {
                controlPanel.sizeDelta = new Vector2(340, 100);
                UIFactory.CreateText(controlPanel, "TRY IN THE INSPECTOR", 14, new Color(1f, 0.85f, 0.35f), FontStyle.Bold);
                UIFactory.CreateText(controlPanel, "Select the object in the Hierarchy during Play, change the value, watch the effect.", 12, UIFactory.MutedColor);
                UIFactory.CreateText(controlPanel, tryInInspector, 13, Color.white);
                UIFactory.CreateText(controlPanel, "Component changes reset when Play stops; asset changes (e.g. Physic Materials) are kept.", 12, UIFactory.MutedColor);
            }
            else UIFactory.CreateText(controlPanel, "SETTINGS", 13, UIFactory.MutedColor, FontStyle.Bold);
            controlPanel.gameObject.SetActive(!string.IsNullOrEmpty(tryInInspector));

            hintText = UIFactory.CreateText(canvas.transform, "Tab: show info panel", 13, UIFactory.MutedColor);
            var hrt = hintText.rectTransform;
            hrt.anchorMin = hrt.anchorMax = hrt.pivot = new Vector2(0, 1);
            hrt.anchoredPosition = new Vector2(12, -12);
            hrt.sizeDelta = new Vector2(300, 24);
            hintText.gameObject.SetActive(false);
        }

        /// <summary>Registers a function whose text is shown (and refreshed 10x per second) in the live-values area.</summary>
        public void AddLiveSource(Func<string> source)
        {
            liveSources.Add(source);
            liveText.gameObject.SetActive(true);
        }

        /// <summary>Shows or hides both panels (Tab key).</summary>
        public void SetPanelsVisible(bool visible)
        {
            panelsVisible = visible;
            infoPanel.gameObject.SetActive(visible);
            controlPanel.gameObject.SetActive(visible && controlPanel.childCount > 1);
            hintText.gameObject.SetActive(!visible);
        }

        public void TogglePanels() => SetPanelsVisible(!panelsVisible);

        void Update()
        {
            if (Time.unscaledTime < nextRefresh) return;
            nextRefresh = Time.unscaledTime + 0.1f;

            if (liveSources.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var source in liveSources)
                {
                    string s = source();
                    if (string.IsNullOrEmpty(s)) continue;
                    if (sb.Length > 0) sb.Append('\n');
                    sb.Append(s);
                }
                liveText.text = sb.ToString();
            }

            if (statusText != null)
            {
                statusText.text = $"T slow-mo: {(TimeControl.IsSlowMotion ? "<color=#ffd75e>ON x0.2</color>" : "off")}   " +
                                  $"G gizmos: {(DebugLines.GizmosVisible ? "on" : "off")}   Tab: hide panel";
            }
        }

        // ---------- Controls (right panel) ----------

        Transform Controls()
        {
            if (!controlPanel.gameObject.activeSelf && panelsVisible) controlPanel.gameObject.SetActive(true);
            return controlPanel;
        }

        /// <summary>Bold section caption in the settings panel.</summary>
        public Text AddHeader(string text) =>
            UIFactory.CreateText(Controls(), text, 14, UIFactory.AccentColor, FontStyle.Bold);

        /// <summary>Small explanatory text in the settings panel.</summary>
        public Text AddNote(string text) => UIFactory.CreateText(Controls(), text, 13, UIFactory.MutedColor);

        public LabeledSlider AddSlider(string label, float min, float max, float value, Action<float> onChange, string format = "0.00") =>
            UIFactory.CreateSlider(Controls(), label, min, max, value, onChange, format);

        public Toggle AddToggle(string label, bool value, Action<bool> onChange) =>
            UIFactory.CreateToggle(Controls(), label, value, onChange);

        public Dropdown AddDropdown(string label, string[] options, int value, Action<int> onChange) =>
            UIFactory.CreateDropdown(Controls(), label, options, value, onChange);

        public Button AddButton(string label, Action onClick) =>
            UIFactory.CreateButton(Controls(), label, onClick);
    }
}
