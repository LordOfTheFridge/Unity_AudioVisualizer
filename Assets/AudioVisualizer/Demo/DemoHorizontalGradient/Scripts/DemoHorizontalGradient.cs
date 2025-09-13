using System.Collections.Generic;
using UnityEngine;
using Utilities.UI.AudioVisualizer;

namespace Demo.HorizontalGradient
{
    public class DemoHorizontalGradient : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> AudioClips;
        [SerializeField] private bool IsLoop = true;
        [SerializeField] private AudioSource AudioSource;
        [SerializeField] private AudioVisualizerHorizontalGradient Visualizer;

        void Start()
        {
            if (
                AudioClips == null ||
                AudioClips.Count < 1 ||
                AudioSource == null
            ) {
                return;
            }

            Visualizer.SetAudioSource(AudioSource);

            AudioSource.clip = AudioClips[Random.Range(0, AudioClips.Count)];
            AudioSource.loop = IsLoop;
            AudioSource.Play();
        }
    }
}
