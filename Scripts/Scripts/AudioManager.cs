using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music Clips")]
    [SerializeField] private AudioClip gameMusicClip;
    [SerializeField] private AudioClip menuMusicClip;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip walkingClip;
    [SerializeField] private AudioClip boomerangThrowClip;
    [SerializeField] private AudioClip boomerangSpinClip;
    [SerializeField] private AudioClip boomerangHitClip;
    [SerializeField] private AudioClip boomerangBreakClip;
    [SerializeField] private AudioClip playerHitClip;
    [SerializeField] private AudioClip powerUpCollectedClip;
    [SerializeField] private AudioClip levelUpClip;
    [SerializeField] private AudioClip timerClip;
    [SerializeField] private AudioSource ambienceSoundSource;

    private AudioSource musicSource;
    private List<AudioSource> soundSources = new List<AudioSource>();

    public enum AudioType { GameMusic, MenuMusic, Walking, BoomerangThrow, BoomerangSpin, BoomerangHit, BoomerangBreak, PlayerHit, PowerUpCollected, LevelUp, Timer }

    private Dictionary<AudioType, AudioClip> audioClips;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        audioClips = new Dictionary<AudioType, AudioClip>
        {
            { AudioType.GameMusic, gameMusicClip },
            { AudioType.MenuMusic, menuMusicClip },
            { AudioType.Walking, walkingClip },
            { AudioType.PlayerHit, playerHitClip },
            { AudioType.BoomerangThrow, boomerangThrowClip },
            { AudioType.BoomerangSpin, boomerangSpinClip },
            { AudioType.BoomerangHit, boomerangHitClip },
            { AudioType.BoomerangBreak, boomerangBreakClip },
            { AudioType.PowerUpCollected, powerUpCollectedClip },
            { AudioType.LevelUp, levelUpClip },
            { AudioType.Timer, timerClip }
        };
    }

    public void PlayMusic(AudioType musicType)
    {
        if (audioClips.TryGetValue(musicType, out AudioClip music))
        {
            musicSource.Stop();
            musicSource.clip = music;
            musicSource.loop = true;
            musicSource.volume = 1f;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
        ambienceSoundSource.Stop();
        
    }

    public void PlaySound(AudioType soundType, float volume = 1f, float pitch = 1f)
    {
        if (audioClips.TryGetValue(soundType, out AudioClip sound))
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = sound;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
            soundSources.Add(source);
            StartCoroutine(RemoveAudioSourceWhenFinished(source));
        }
        ambienceSoundSource.Play();
    }

    public void StopSound(AudioType soundType)
    {
        for (int i = soundSources.Count - 1; i >= 0; i--)
        {
            if (soundSources[i].clip == audioClips[soundType])
            {
                Destroy(soundSources[i]);
                soundSources.RemoveAt(i);
            }
        }
        
    }

    private System.Collections.IEnumerator RemoveAudioSourceWhenFinished(AudioSource source)
    {
        yield return new WaitUntil(() => !source.isPlaying);
        soundSources.Remove(source);
        Destroy(source);
    }
}