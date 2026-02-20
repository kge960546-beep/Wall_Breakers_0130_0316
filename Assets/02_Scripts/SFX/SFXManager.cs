using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    WaitForSeconds wait = new WaitForSeconds(1f);

    private Dictionary<string, AudioClip> sfxClipDic;
    AudioSource bgmPlayer;
    AudioSource sfxPlayer;

    [SerializeField] AudioClip[] audioClips;

    [Header("BGM 犁积格废")]
    [SerializeField] string[] bgmPlayList;
    int currentBGMIndex = 0;

    // BGM 内风凭 历厘侩
    Coroutine bgmRoutine;

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

        StartPlayBGM();
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
        if (bgmPlayList != null && bgmPlayList.Length > 0)
        {
            if (bgmRoutine != null)
                StopCoroutine(bgmRoutine);

            bgmRoutine = StartCoroutine(BGMQueueRoutine());
        }
    }

    public void PlayNextBGM()
    {
        currentBGMIndex = (currentBGMIndex + 1) % bgmPlayList.Length;
        PlayOnBGM(bgmPlayList[currentBGMIndex]);
    }

    IEnumerator BGMQueueRoutine()
    {
        while (true)
        {
            string currentBGM = bgmPlayList[currentBGMIndex];

            if(sfxClipDic.TryGetValue(currentBGM, out var clip))
            {
                bgmPlayer.clip = clip;
                bgmPlayer.Play();
                yield return new WaitForSeconds(clip.length);
            }
            else
            {
                yield return wait;
            }

            currentBGMIndex = (currentBGMIndex + 1) % bgmPlayList.Length;
        }
    }

    public void PauseBGM()
    {
        if (bgmRoutine != null)
        {
            StopCoroutine(bgmRoutine);
            bgmRoutine = null;
        }

        if (bgmPlayer.isPlaying)
            bgmPlayer.Pause();
    }

    public void ResumeBGM()
    {
        if (bgmPlayer.clip != null)
        {
            bgmPlayer.UnPause();
            bgmRoutine = StartCoroutine(BGMQueueRoutine());
        }
    }
}
