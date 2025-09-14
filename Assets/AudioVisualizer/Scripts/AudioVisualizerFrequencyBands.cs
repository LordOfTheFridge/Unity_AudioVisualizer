using UnityEngine;
using UnityEngine.UI;

namespace Utilities.UI.AudioVisualizer
{
    public class AudioVisualizerFrequencyBands : MonoBehaviour
    {
        [SerializeField] private AudioSource AudioSource;

        [Space(15)]
        [SerializeField] private GameObject BandPrefab;

        [Space(15)]
        [SerializeField] private Color[] GradientColors;

        [Space(15)]
        [SerializeField] private float MinHeight = 15.0f;
        [SerializeField] private float MaxHeight = 425.0f;
        [SerializeField] private float UpdateSentivity = 0.2f;
        [SerializeField] private float SizeMultiplier = 1000f;
        [SerializeField] private int FrequencyBandsCount = 8;
        [SerializeField] [Range(64, 8192)] private int SamplesCount = 64;
        [SerializeField] private FFTWindow SpectrumAnalysisType = FFTWindow.Triangle;

        private float[] frequencyBands;
        private float[] spectrumData;
        private GameObject[] bandObjects;
        private RectTransform[] transforms;

        void Start()
        {
            if (BandPrefab == null || FrequencyBandsCount < 1 || AudioSource == null) {
                return;
            }

            InstantiateBands();

            if (GradientColors != null && GradientColors.Length != 0) {
                SetColorsFromGradient();
            }

            frequencyBands = new float[FrequencyBandsCount];
            spectrumData = new float[SamplesCount];
        }

        void Update()
        {
            if (AudioSource != null && AudioSource.isPlaying) {
                AudioSource.GetSpectrumData(spectrumData, 0, SpectrumAnalysisType);
                CalculateFrequencyBands(spectrumData);
                ChangeBandHeights(frequencyBands);
            }
        }

        private void InstantiateBands()
        {
            transforms = new RectTransform[FrequencyBandsCount];
            bandObjects = new GameObject[FrequencyBandsCount];
            for (var i = 0; i < FrequencyBandsCount; i++) {
                bandObjects[i] = Instantiate(BandPrefab, transform);
                transforms[i] = bandObjects[i].GetComponent<RectTransform>();
            }
        }

        private void CalculateFrequencyBands(float[] spectrumData)
        {
            // get maximum frequency (Nyquist frequency)
            if (AudioSource.clip == null) {
                Debug.LogWarning("AudioSource.clip is null. Cannot calculate sampleRate.");
                return;
            }

            float maxHz = AudioSource.clip.frequency / 2f; // Typically 22050 Hz at 44100 Hz

            // Clearing frequency band array
            for (int i = 0; i < FrequencyBandsCount; i++) {
                frequencyBands[i] = 0f;
            }

            // For each band we calculate frequency range and average amplitudes
            for (int i = 0; i < FrequencyBandsCount; i++) {
                // Logarithmic frequency limits for band i
                float lowHz = Mathf.Pow(2, i * Mathf.Log(maxHz, 2) / FrequencyBandsCount);
                float highHz = Mathf.Pow(2, (i + 1) * Mathf.Log(maxHz, 2) / FrequencyBandsCount);

                // Convert frequencies into indices of the spectrumData
                int lowIndex = Mathf.FloorToInt(lowHz / (maxHz / spectrumData.Length));
                int highIndex = Mathf.FloorToInt(highHz / (maxHz / spectrumData.Length));

                // adjust boundaries so as not to go beyond array
                lowIndex = Mathf.Clamp(lowIndex, 0, spectrumData.Length - 1);
                highIndex = Mathf.Clamp(highIndex, 0, spectrumData.Length - 1);

                // If range is too small, skip
                if (lowIndex >= highIndex)
                    continue;

                // average amplitudes in the range
                float sum = 0f;
                int count = 0;
                for (int j = lowIndex; j <= highIndex; j++) {
                    sum += spectrumData[j];
                    count++;
                }

                // Keep average value
                frequencyBands[i] = (count > 0) ? sum / count : 0f;
            }
        }

        private void ChangeBandHeights(float[] bandData)
        {
            for (var i = 0; i < FrequencyBandsCount; i++) {
                var height = Mathf.Clamp(
                    Mathf.Lerp(transforms[i].sizeDelta.y, MinHeight + (bandData[i] * (MaxHeight - MinHeight) * SizeMultiplier), UpdateSentivity),
                    MinHeight,
                    MaxHeight
                );

                transforms[i].sizeDelta = new Vector2(transforms[i].sizeDelta.x, height);
            }
        }

        public void SetAudioSource(AudioSource source)
        {
            if (source == null) {
                return;
            }

            AudioSource = source;
        }

        private void SetColorsFromGradient()
        {
            var gradient = new Gradient();
            var colorKeys = new GradientColorKey[GradientColors.Length];
            var alphaKeys = new GradientAlphaKey[GradientColors.Length];
            for (var i = 0; i < GradientColors.Length; i++) {
                var time = (float)i / GradientColors.Length;
                colorKeys[i] = new GradientColorKey(GradientColors[i], time);
                alphaKeys[i] = new GradientAlphaKey(GradientColors[i].a, time);
            }
            gradient.SetKeys(colorKeys, alphaKeys);

            for (var i = 0; i < FrequencyBandsCount; i++) {
                bandObjects[i].GetComponent<Image>().color = gradient.Evaluate((float)i / FrequencyBandsCount);
            }
        }
    }
}
