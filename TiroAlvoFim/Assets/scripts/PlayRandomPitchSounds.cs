using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomPitchSounds : MonoBehaviour
{
    private AudioSource source;
    [SerializeField] AudioClip[] sounds;

    [SerializeField] float minPitch = 75;
    [SerializeField] float maxPitch = 120;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    public void Play(int index)
    {
        source.pitch = Random.Range(minPitch, maxPitch) / 100;
        source.PlayOneShot(sounds[index]);
    }
    public void PlayRandom()
    {
        if (sounds.Length > 0)
        {
            source.pitch = Random.Range(minPitch, maxPitch) / 100;
            source.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
        }
    }
    public bool IsPlaying() { return source.isPlaying; }
}
