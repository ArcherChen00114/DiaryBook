using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField]
    private List<AudioSource> audioSources = new List<AudioSource>();

    [SerializeField]
    private AudioSource musicSource;
    
    [SerializeField]
    private List<AudioClip> musicList = new List<AudioClip>();

    private void OnEnable()
    {
        //SwitchMusic(MusicName.normal);
        EventHandler.PlaySFXEvent += OnPlayerSFXEvent;
    }


    private void OnDisable()
    {
        EventHandler.PlaySFXEvent -= OnPlayerSFXEvent;
    }
    private void OnPlayerSFXEvent(AudioSource audioSource)
    {
        audioSource.Play();
    }
    public void PlaySoundInList(AudioClipName ClipName) {
        EventHandler.CallPlaySFXEvent(audioSources[(int)ClipName]);
    }

    public void SwitchMusic(MusicName musicname) {
        musicSource.clip = musicList[(int)musicname];
        musicSource.Play();
    }

}
