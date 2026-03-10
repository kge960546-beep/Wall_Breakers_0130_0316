using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    [Header("UI설정")]
    [SerializeField] GameObject achievementUIpanel;
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;

    [Header("애니메이션 설정")]
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [SerializeField] float slideSpeed;
    [SerializeField] float stayTime;
    

    private void OnEnable()
    {
        StartCoroutine(WaitSubscribe());
    }
    private void OnDisable()
    {
        AchievementsManager.instance.UnsubscribeonAchievementUnlocked(ShowPanel);
    }
    void Start()
    {
        achievementUIpanel.SetActive(false);
    }

    //최종적으로 업적 달성시 이벤트구동으로 UI로직을 불러오는 함수
    public void ShowPanel(AchievementSO data)
    {
        Utils.DebugLog("UI수신성공" + data.title);
        StopAllCoroutines();

        if(SFXManager.instance != null)
        {
            SFXManager.instance.PlayOnSFX("541980__rob_marion__gasp_chimes_success_3", Camera.main.transform.position);
        }

        titleText.text = data.title;
        descriptionText.text = data.description;

        StartCoroutine(PanelAmin());
    }   

    //이벤트 구독 실행순서 오류로 싱글톤보다 먼저 실행되게 하지않기
    IEnumerator WaitSubscribe()
    {
        if(AchievementsManager.instance == null)
        {
            yield return null;
        }

        AchievementsManager.instance.SubscribeonAchievementUnlocked(ShowPanel);
        Utils.DebugLog("업적매니저 구동 성공");
    }

    //UI가 어떤식으로 움직이는지 정한 함수
    IEnumerator PanelAmin()
    {
        achievementUIpanel.SetActive(true);

        float elapsed = 0.0f;
        float duration = slideSpeed > 0 ? slideSpeed : 0.5f;
        float wait = stayTime > 0 ? stayTime : 2.0f;

        Vector3 startPos = startPoint.position;
        Vector3 endPos = endPoint.position;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            achievementUIpanel.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            yield return null;
        }

        yield return new WaitForSeconds(wait);

        elapsed = 0.0f;
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            achievementUIpanel.transform.position = Vector3.Lerp(endPos, startPos, elapsed / duration);
            yield return null;
        }

        achievementUIpanel.SetActive(false);
    }
}
