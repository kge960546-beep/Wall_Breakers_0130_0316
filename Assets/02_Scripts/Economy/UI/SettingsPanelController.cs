using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private RectTransform panelRect;


    [Header("Volume Sliders")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Credits Panel")]
    [SerializeField] private GameObject creditsPanel;

    private void Start()
    {
        if (SFXManager.instance != null)
        {
            bgmSlider.value = SFXManager.instance.bgmVolume;
            sfxSlider.value = SFXManager.instance.sfxVolume;
        }

        bgmSlider.onValueChanged.AddListener(val => SFXManager.instance.SetBGMVolume(val));
        sfxSlider.onValueChanged.AddListener(val => SFXManager.instance.SetSFXVolume(val));
    }

    public void Open()
    {
        if (settingsPanel.activeSelf == true) return;

        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlayOnSFX("683097__florianreichelt__bubble-bursting 2", Camera.main.transform.position);
        }

        settingsPanel.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(OpenAnimation());

    }

    IEnumerator OpenAnimation()
    {

        Vector2 end = panelRect.anchoredPosition;

        Vector2 start;

        int dir = Random.Range(0, 4);

        switch (dir)
        {
            case 0: start = new Vector2(0, 900); break;
            case 1: start = new Vector2(0, -900); break;
            case 2: start = new Vector2(-1400, 0); break;
            default: start = new Vector2(1400, 0); break;
        }

        Vector2 control = (start + end) * 0.5f + new Vector2(
            Random.Range(-400f, 400f),
            Random.Range(200f, 600f)
        );

        // 시작 위치 이동
        panelRect.anchoredPosition = start;

        // 1/10 크기 시작
        panelRect.localScale = Vector3.one * 0.1f;

        // 작은 상태로 Bezier 이동
        yield return StartCoroutine(
            UITween.MoveBezierUI(panelRect, start, control, end, 0.9f)
        );

        // 도착 후 스케일 복구
        yield return StartCoroutine(
            UITween.Scale(panelRect, Vector3.one * 0.1f, Vector3.one, 0.25f)
        );
    }

    public void Close()
    {
        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlayOnSFX("683097__florianreichelt__bubble-bursting 2", Camera.main.transform.position);
        }

        StopAllCoroutines();
        StartCoroutine(ClosePanelRoutine(settingsPanel));
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenCredits()
    {
        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlayOnSFX("683097__florianreichelt__bubble-bursting 2", Camera.main.transform.position);
        }

        creditsPanel.SetActive(true);

        RectTransform rect = creditsPanel.GetComponent<RectTransform>();

        StopAllCoroutines();

        rect.localScale = Vector3.one * 0.01f;

        StartCoroutine(
            UITween.Scale(rect, Vector3.one * 0.01f, Vector3.one, 0.25f)
        );
    }

    public void CloseCredits()
    {
        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlayOnSFX("683097__florianreichelt__bubble-bursting 2", Camera.main.transform.position);
        }

        StopAllCoroutines();
        StartCoroutine(ClosePanelRoutine(creditsPanel));
    }

    IEnumerator ClosePanelRoutine(GameObject panel)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();

        yield return StartCoroutine(
            UITween.Scale(rect, Vector3.one, Vector3.one * 0.01f, 0.2f)
        );

        panel.SetActive(false);
    }
}