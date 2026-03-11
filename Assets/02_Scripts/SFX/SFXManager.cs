using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    WaitForSeconds wait = new WaitForSeconds(1f);

    private Dictionary<string, AudioClip> sfxClipDic;
    AudioSource bgmPlayer;
    AudioSource sfxPlayer;
    [SerializeField] GameObject sfxPrefab;

    [SerializeField] AudioClip[] audioClips;

    [Header("BGM 재생목록")]
    [SerializeField] string[] bgmPlayList;
    int currentBGMIndex = 0;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgmVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 0.5f;

    bool isSFXBlocked = false;
    bool is3D = true;

    // BGM 코루틴 저장용
    Coroutine bgmRoutine;

    private void Awake()
    {
        if (instance == null)
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
        if (audioClips != null)
        {
            foreach (var clip in audioClips)
            {
                sfxClipDic[clip.name] = clip;
            }
        }

        StartPlayBGM();
    }

    // 볼륨변수 추가
    public void PlayOnSFX(string soundName, Vector3 soundPos, float duration = -1.0f)
    {
        if (isSFXBlocked) return;

        if (sfxClipDic.TryGetValue(soundName, out var clip))
        {
            GameObject tr = PoolManager.instance.Get(sfxPrefab, soundPos, Quaternion.identity);
            tr.transform.position = soundPos;
            AudioSource source = tr.GetComponent<AudioSource>();

            source.clip = clip;
            source.volume = sfxVolume;

            source.dopplerLevel = 0.0f;
            source.spatialBlend = 1.0f;
            source.minDistance = 5.0f;
            source.maxDistance = 50f;
            source.rolloffMode = AudioRolloffMode.Linear;

            source.Play();

            float playTime = (duration > 0) ? duration : clip.length;

            StartCoroutine(ReturnSFX(tr, source, playTime));
        }
    }

    IEnumerator ReturnSFX(GameObject tr, AudioSource source, float time)
    {
        yield return new WaitForSeconds(time);

        if (source == null || tr == null) yield break;

        float fadeTime = 0.1f;
        float startVol = source.volume;
        while (source != null && source.volume > 0)
        {
            source.volume -= startVol * (Time.deltaTime / fadeTime);
            yield return null;
        }

        PoolManager.instance.ReturnIt(sfxPrefab, tr);
    }

    public void BlockSFX(bool isblock)
    {
        isSFXBlocked = isblock;
    }
    public void PlayOnBGM(string soundName)
    {
        if (sfxClipDic.TryGetValue(soundName, out var clip))
        {
            if (bgmPlayer.clip == clip && bgmPlayer.isPlaying) return;

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

            if (sfxClipDic.TryGetValue(currentBGM, out var clip))
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

    // BGM 볼륨 조절
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        if (bgmPlayer != null)
        {
            bgmPlayer.volume = bgmVolume;
        }
    }

    // SFX 볼륨 조절
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        // sfxPlayer.volume = sfxVolume; // sfxPlayer를 직접 쓸 경우
    }
}
