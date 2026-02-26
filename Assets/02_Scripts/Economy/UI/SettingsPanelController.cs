using UnityEngine;
using UnityEngine.UI; // 추가

public class SettingsPanelController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    [Header("Volume Sliders")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        // 시작할 때 현재 SFXManager의 볼륨 값을 슬라이더에 반영
        if (SFXManager.instance != null)
        {
            bgmSlider.value = SFXManager.instance.bgmVolume;
            sfxSlider.value = SFXManager.instance.sfxVolume;
        }

        // 슬라이더 값이 변할 때마다 매니저 함수 호출 연결
        bgmSlider.onValueChanged.AddListener(val => SFXManager.instance.SetBGMVolume(val));
        sfxSlider.onValueChanged.AddListener(val => SFXManager.instance.SetSFXVolume(val));
    }

    public void Open()
    {
        settingsPanel.SetActive(true);
    }

    public void Close()
    {
        settingsPanel.SetActive(false);
    }

    /// <summary>
    /// 게임 종료 버튼에 연결할 메서드
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Game Quit Requested");

        // 1. 실제 빌드된 게임 종료
        Application.Quit();

        // 2. 유니티 에디터의 Play 모드 종료 (개발 중 확인용)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}