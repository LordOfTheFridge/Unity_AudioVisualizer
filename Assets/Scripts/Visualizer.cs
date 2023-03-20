using UnityEngine;
using UnityEngine.UI;

public class Visualizer : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private bool loop = true;
    [SerializeField] private AudioSource audioSource;
    [Space(15)]

    [SerializeField] private Color[] gradientColors;
    [Space(15)]

    [SerializeField] private float minHeight = 15.0f;
    [SerializeField] private float maxHeight = 425.0f;
    [SerializeField] private float updateSentivity = 0.1f;
    [SerializeField] [Range(64, 8192)] private int samplesCount = 64;
    [SerializeField] private FFTWindow spectrumAnalysisType = FFTWindow.Triangle;
    private float[] spectrumData;
    [Space(15)]

    [SerializeField] private GameObject samplePrefab;

    private GameObject[] samples;
    private RectTransform[] transforms;

    void Start()
    {
        if (
            audioClip == null ||
            audioSource == null ||
            samplePrefab == null
        ) {
            return;
        }

        InstantiateSamples();

        if (gradientColors != null && gradientColors.Length != 0) {
            SetColorsFromGradient();
        }

        spectrumData = new float[samplesCount];

        audioSource.loop = loop;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    void Update()
    {
        if (audioSource.isPlaying) {
            audioSource.GetSpectrumData(spectrumData, 0, spectrumAnalysisType);

            for (var i = 0; i < samplesCount; i++) {
                var newSize = transforms[i].rect.size;
                newSize.y = Mathf.Clamp(Mathf.Lerp(newSize.y, minHeight + (spectrumData[i] * (maxHeight - minHeight) * 5.0f), updateSentivity), minHeight, maxHeight);
                transforms[i].sizeDelta = newSize;
            }
        }
    }

    private void InstantiateSamples()
    {
        transforms = new RectTransform[samplesCount];
        samples = new GameObject[samplesCount];
        for (var i = 0; i < samplesCount; i++) {
            samples[i] = Instantiate(samplePrefab, transform);
            transforms[i] = samples[i].GetComponent<RectTransform>();
        }
    }

    private void SetColorsFromGradient()
    {
        var gradient = new Gradient();
        var colorKeys = new GradientColorKey[gradientColors.Length];
        var alphaKeys = new GradientAlphaKey[gradientColors.Length];
        for (var i = 0; i < gradientColors.Length; i++) {
            var time = (float)i / gradientColors.Length;
            colorKeys[i] = new GradientColorKey(gradientColors[i], time);
            alphaKeys[i] = new GradientAlphaKey(gradientColors[i].a, time);
        }
        gradient.SetKeys(colorKeys, alphaKeys);

        for (var i = 0; i < samplesCount; i++) {
            samples[i].GetComponent<Image>().color = gradient.Evaluate((float)i / samplesCount);
        }
    }
}
