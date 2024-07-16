using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{

    public static GameTime time = new GameTime(1,8,0,0);


    public static Action<AudioSource> PlaySFXEvent;

    public static void CallPlaySFXEvent(AudioSource audioSource)
    {
        PlaySFXEvent?.Invoke(audioSource);
    }

    public static Action<AudioClip> PlayBGMEvent;

    public static void CallPlayBGMEvent(AudioClip clip) {
        PlayBGMEvent?.Invoke(clip);
    }
}
