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

    public void ShowPanel(AchievementSO data)
    {
        Utils.DebugLog("UI수신성공" + data.title);
        StopAllCoroutines();

        titleText.text = data.title;
        descriptionText.text = data.description;

        StartCoroutine(PanelAmin());
    }   

    IEnumerator WaitSubscribe()
    {
        if(AchievementsManager.instance == null)
        {
            yield return null;
        }

        AchievementsManager.instance.SubscribeonAchievementUnlocked(ShowPanel);
        Utils.DebugLog("업적매니저 구동 성공");
    }
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
