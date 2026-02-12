using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    private Dictionary<string, AudioClip> sfxClipDic;
    AudioSource bgmPlayer;
    AudioSource sfxPlayer;

    [SerializeField] AudioClip[] audioClips;

    [Header("BGM 재생목록")]
    [SerializeField] string[] bgmPlayList;
    int currentBGMIndex = 0;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if(bgmPlayList != null && bgmPlayList.Length > 0)
        {
            if(!bgmPlayer.isPlaying)
            {
                PlayNextBGM();
            }
        }
    }

    void Init()
    {
        bgmPlayer = gameObject.AddComponent<AudioSource>();
        sfxPlayer = gameObject.AddComponent<AudioSource>();

        bgmPlayer.loop = false;
        bgmPlayer.volume = 0.5f;
        bgmPlayer.playOnAwake = false;

        bgmPlayer.spatialBlend = 0.0f;
        sfxPlayer.spatialBlend = 1.0f;

        sfxClipDic = new Dictionary<string, AudioClip>();

        if(audioClips != null)
        {
            foreach(var clip in audioClips)
            {
                sfxClipDic[clip.name] = clip;
            }
        }
    }

    public void PlayOnSFX(string soundName, Vector3 soundPos)
    {
        if(sfxClipDic.TryGetValue(soundName, out var clip))
        {
            AudioSource.PlayClipAtPoint(clip, soundPos);            
        }
    }

    public void PlayOnBGM(string soundName)
    {
        if (sfxClipDic.TryGetValue(soundName, out var clip))
        {
            if(bgmPlayer.clip == clip && bgmPlayer.isPlaying) return;

            bgmPlayer.clip = clip;
            bgmPlayer.Play();
        }
    }

    public void StartPlayBGM()
    {
        if(bgmPlayList != null && bgmPlayList.Length > 0)
        {
            PlayOnBGM(bgmPlayList[currentBGMIndex]);
        }
    }

    public void PlayNextBGM()
    {
        currentBGMIndex = (currentBGMIndex + 1) % bgmPlayList.Length;
        PlayOnBGM(bgmPlayList[currentBGMIndex]);
    }
}
