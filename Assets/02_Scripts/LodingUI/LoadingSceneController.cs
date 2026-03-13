using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    #region Singleton

    private static LoadingSceneController instance;
    public static LoadingSceneController Instance
    {
        get
        {
            if (instance == null)
            {
                LoadingSceneController sceneController = FindAnyObjectByType<LoadingSceneController>();

                if (sceneController != null)
                {
                    instance = sceneController;
                }
                else
                {
                    instance = Create();
                }
            }

            return instance;
        }
    }

    /// <summary>
    /// 모든씬에서 특별한 설정 없이 불러오기
    /// 한번만하면 싱글톤이기 때문에 더이상 로드를 하지않아도됩니다
    /// 이유:DontDestroyOnLoad를 통해 로딩 화면이 씬 A에서 씬 B로 넘어가는 공백기를 메꿔주는 역할을 하기 때문
    /// </summary>
    /// <returns></returns>
    private static LoadingSceneController Create()
    {
        return Instantiate(Resources.Load<LoadingSceneController>("LoadingUI"));
    }
    #endregion

    [SerializeField] private CanvasGroup canvasGroup;       //페이드 인 아웃 효과를 주기위한 그룹
    [SerializeField] private Image progressBar;             //로딩이 얼마나 되었는지 보여주느귀한 이미지
    [SerializeField] private TextMeshProUGUI toolTipLabel;  //로딩 화면 도중 정보를 텍스트로 제공하기 위한 툴팁 라벨

    [SerializeField][TextArea] String[] toolTips;           //제공할 툴팁을 미리 정한다.
    private string loadSceneName;

    Action onSceneLoadAction;                               //로딩할때 함수를 인자로 추가로 전달했다면 해당 함수를 호출하기위한 Action


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 싱글턴을 이용하여 외부에서 호출하여 씬을 로드
    /// action을 추가하여 씬을 완전히 로드한 후 한 프레임이 지난 LateStart 시점에 함수를 호출  
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="action"></param>
    public void LoadScene(string sceneName, Action action = null)
    {
        gameObject.SetActive(true);
        loadSceneName = sceneName;
        onSceneLoadAction = action;

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (toolTips.Length > 0)
        {
            toolTipLabel.text = toolTips[UnityEngine.Random.Range(0, toolTips.Length)];
        }

        StartCoroutine(LoadSceneProcessCo());
    }

    /// <summary>
    /// Fade가 우선으로 일어나게하고
    /// AsyncOperation op 를 이용하여 비동기 씬 로드를 한다
    /// mProgressBar.fillAmount = op.progress; 값을 이용하여 로딩이 얼마나 되었는지 진척도 갱신
    /// if (op.progress < 0.9f)로 약 90% 정도는 실제 진척도만큼의 로딩바를 보여주고
    /// 나머지 10%는 자연스럽게 차오르는 연출로 구성
    /// 로딩이 완료되면 op.allowSceneActivation = true;을 호출하여 씬을 활성화    
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadSceneProcessCo()
    {
        progressBar.fillAmount = 0.0f;

        //1. 해당 캔버스를 검게 페이드인
        yield return StartCoroutine(Fade(true));

        //2. 비동기 씬로드 시작
        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        op.allowSceneActivation = false; //90%에서 로딩대기

        float process = 0.0f;

        while (!op.isDone)
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                //로딩 진핼률 반영
                progressBar.fillAmount = op.progress;
            }
            else
            {
                //90% 이후는 가짜로딩으로 자연스러운 연출
                process += Time.deltaTime * 1.5f; //속도
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1.0f, process);

                if (process > 1.0f)
                {
                    //로딩바가100% 가되면 씬 활성화
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// 씬이 로드되었을 때 StartCoroutine(Fade(false));를 호출하여 현재 로딩화면을 서서히 사라지게 한다.
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == loadSceneName)
        {
            //구독해지
            SceneManager.sceneLoaded -= OnSceneLoaded;
            StartCoroutine(Fade(false));
        }
    }

    /// <summary>
    /// 씬이 로드되면 Awake, Start 등 씬 내에 배치되어 있는 컴포넌트들이 초기화를 진행
    /// 이 상태에서 한 프레임을 쉰 후 예약된 함수를 호출하면 Awake, Start가 모두 실행된 후에 호출되기에 스크립트 실행 순서에 상관없이 가장 마지막에 호출되는 점을 이용하여 안정적인 함수 구성이 가능
    /// </summary>
    /// <returns></returns>
    IEnumerator LateStartCo()
    {
        yield return new WaitForEndOfFrame();

        onSceneLoadAction?.Invoke();
        onSceneLoadAction = null;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 페이드인 아웃을 주기위한 코루틴
    /// 페이드이후 로직처리
    /// </summary>
    /// <param name="isFadeIn"></param>
    /// <returns></returns>
    IEnumerator Fade(bool isFadeIn)
    {
        float process = 0f;

        while (process < 1.0f)
        {
            process += Time.unscaledDeltaTime;
            canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0.0f, 1.0f, process) : Mathf.Lerp(1.0f, 0.0f, process);

            yield return null;
        }

        if (!isFadeIn)
        {
            StartCoroutine(LateStartCo());
        }
    }
}
