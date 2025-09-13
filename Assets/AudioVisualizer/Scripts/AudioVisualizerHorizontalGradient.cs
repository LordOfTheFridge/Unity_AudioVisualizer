using UnityEngine;
using UnityEngine.UI;

namespace Utilities.UI.AudioVisualizer
{
    public class AudioVisualizerHorizontalGradient : MonoBehaviour
    {
        [SerializeField] private AudioSource AudioSource;
        [Space(15)]

        [SerializeField] private Color[] GradientColors;
        [Space(15)]

        [SerializeField] private float MinHeight = 15.0f;
        [SerializeField] private float MaxHeight = 425.0f;
        [SerializeField] private float UpdateSentivity = 0.2f;
        [SerializeField] private float SizeMultiplier = 100.0f;
        [SerializeField] [Range(64, 8192)] private int SamplesCount = 64;
        [SerializeField] private FFTWindow SpectrumAnalysisType = FFTWindow.Triangle;
        [Space(15)]

        [SerializeField] private GameObject SamplePrefab;

        private float[] spectrumData;
        private GameObject[] samples;
        private RectTransform[] transforms;

        void Start()
        {
            if (SamplePrefab == null) {
                return;
            }

            InstantiateSamples();

            if (GradientColors != null && GradientColors.Length != 0) {
                SetColorsFromGradient();
            }

            spectrumData = new float[SamplesCount];
        }

        void Update()
        {
            if (AudioSource != null && AudioSource.isPlaying) {
                AudioSource.GetSpectrumData(spectrumData, 0, SpectrumAnalysisType);
                ChangeSampleHeights(spectrumData);
            }
        }

        private void ChangeSampleHeights(float[] spectrumData)
        {
            for (var i = 0; i < SamplesCount; i++) {
                var height = Mathf.Clamp(
                    Mathf.Lerp(transforms[i].sizeDelta.y, MinHeight + (spectrumData[i] * (MaxHeight - MinHeight) * SizeMultiplier), UpdateSentivity), 
                    MinHeight, 
                    MaxHeight
                );
                
                transforms[i].sizeDelta = new Vector2(transforms[i].sizeDelta.x, height);
            }
        }

        private void InstantiateSamples()
        {
            transforms = new RectTransform[SamplesCount];
            samples = new GameObject[SamplesCount];
            for (var i = 0; i < SamplesCount; i++) {
                samples[i] = Instantiate(SamplePrefab, transform);
                transforms[i] = samples[i].GetComponent<RectTransform>();
            }
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

            for (var i = 0; i < SamplesCount; i++) {
                samples[i].GetComponent<Image>().color = gradient.Evaluate((float)i / SamplesCount);
            }
        }

        public void SetAudioSource(AudioSource source)
        {
            if (source == null) {
                return;
            }

            AudioSource = source;
        }
    }
}
