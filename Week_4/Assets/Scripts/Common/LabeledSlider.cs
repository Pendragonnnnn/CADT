using UnityEngine;
using UnityEngine.UI;

namespace PhysicsDemo
{
    /// <summary>Keeps a slider's "Label: value" caption up to date. Created by <see cref="UIFactory"/>.</summary>
    public class LabeledSlider : MonoBehaviour
    {
        public Slider Slider { get; private set; }
        Text caption;
        string label;
        string format;

        /// <summary>Called once by the factory.</summary>
        public void Init(Slider slider, Text captionText, string labelText, string valueFormat)
        {
            Slider = slider;
            caption = captionText;
            label = labelText;
            format = valueFormat;
            RefreshCaption();
        }

        /// <summary>Changes the value without firing the change callback (used to sync UI to data).</summary>
        public void SetWithoutNotify(float value)
        {
            Slider.SetValueWithoutNotify(value);
            RefreshCaption();
        }

        /// <summary>Rewrites the caption from the current slider value.</summary>
        public void RefreshCaption()
        {
            caption.text = $"{label}: <b>{Slider.value.ToString(format)}</b>";
        }
    }
}
