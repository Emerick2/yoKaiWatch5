using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class VolumeMixerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private Slider slider;

    private AudioMixer mixer;
    private string parameterName;

    public void Initialize(
        AudioMixer audioMixer,
        string exposedParameter,
        string displayName
    )
    {
        mixer = audioMixer;
        parameterName = exposedParameter;

        title.text = displayName;

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;

        if (mixer.GetFloat(parameterName, out float db))
        {
            float volume = db <= -80f
                ? 0f
                : Mathf.Pow(10f, db / 20f);

            slider.SetValueWithoutNotify(
                Mathf.Clamp01(volume)
            );
        }

        slider.onValueChanged.RemoveListener(OnVolumeChanged);
        slider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        if (mixer == null)
            return;

        // Conversion volume linéaire -> décibels
        float db = value <= 0.0001f
            ? -80f
            : 20f * Mathf.Log10(value);

        mixer.SetFloat(parameterName, db);
    }

    private void OnDestroy()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(OnVolumeChanged);
    }
}