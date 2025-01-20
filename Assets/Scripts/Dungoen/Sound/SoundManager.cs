using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField][Range(0f, 1f)] private float soundEffectVolume;
    [SerializeField][Range(0f, 1f)] private float musicVolume;

    Pool objectPool;
    AudioSource musicAudioSource;
    public AudioClip clip;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }


        musicAudioSource = GetComponent<AudioSource>();
        musicAudioSource.volume = musicVolume;
        musicAudioSource.loop = true;

        objectPool = GetComponent<Pool>();
        objectPool.CreatePool(transform);
    }


    public void ChangeBackGroundMusic(AudioClip clip, float soundVolume)
    {
        musicAudioSource.Stop();
        musicAudioSource.clip = clip;
        musicAudioSource.volume = soundVolume;
        musicAudioSource.Play();

    }

    public void StopAudio()
    {
        musicAudioSource.Stop();
    }

    public void PlayClip(AudioClip clip)
    {
        GameObject obj = objectPool.SpawnFromPool("SoundSource");
        obj.SetActive(true);
        SoundSource soundSource = obj.GetComponent<SoundSource>();
        soundSource.Play(clip, soundEffectVolume);
    }

}
