using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.AudioVisualizer
{
    public class DemoStarter : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> AudioClips;
        [SerializeField] private bool IsLoop = true;
        [SerializeField] private AudioSource AudioSource;

        void Start()
        {
            if (
                AudioClips == null ||
                AudioClips.Count < 1 ||
                AudioSource == null
            ) {
                return;
            }

            AudioSource.clip = AudioClips[Random.Range(0, AudioClips.Count)];
            AudioSource.loop = IsLoop;
            AudioSource.Play();
        }
    }
}
