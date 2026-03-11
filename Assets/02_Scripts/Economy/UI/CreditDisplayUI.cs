using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditDisplayUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI creditText;
    [SerializeField] private Image creditIcon;

    [Header("Animation Settings")]
    [SerializeField] private bool useAnimation = true;
    [SerializeField] private float animationSpeed = 5f; // 숫자 증가 애니메이션 속도

    private CreditService creditService;
    private bool isInitialized = false;

    // 애니메이션을 위한 현재 표시 값
    private long currentDiplayValue = 0;
    private long targetValue = 0;

    private void Awake()
    {
        Utils.DebugLog("[CreditDisplayUI] Start called");
        ValidateSetup();
        InitializeCreditService();
    }
    private void ValidateSetup()
    {
        if(creditText == null)
        {
            Utils.DebugLogError("[CreditDisplayUI] CreditText is Not assigned!");
        }
        else
        {
            Utils.DebugLog($"[CreditDisplayUI] CreditText assigned: {creditText.gameObject.name}");
            // 초기 텍스트 설정 (테스트용)
            creditText.text = "0";
        }
    }
    private void InitializeCreditService()
    {
        if (isInitialized) return;

        if (GameManager.Instance == null)
        {
            Invoke(nameof(InitializeCreditService), 0.1f);
            return;
        }

        creditService = GameManager.Instance.GetService<CreditService>();

        if (creditService == null)
        {
            Utils.DebugLogError("[CreditDisplayUI] CreditService is Not assigned!");
            return;
        }

        // 이벤트 구독
        creditService.OnCreditsChanged += HandleCreditsUpdated;
        isInitialized = true;

        // 초기값 설정
        currentDiplayValue = creditService.credits;
        targetValue = creditService.credits;
        UpdateDisplay();
    }
    private void OnDestroy()
    {
        if (creditService != null)
        {
            creditService.OnCreditsChanged -= HandleCreditsUpdated;
        }
    }
    private void HandleCreditsUpdated(long newCredits)
    {
        targetValue = newCredits;

        if(!useAnimation)
        {
            currentDiplayValue = targetValue;
            UpdateDisplay();
        }
    }
    private void Update()
    {
        // 애니메이션 사용 시 부드럽게 증가
        if(useAnimation && currentDiplayValue != targetValue)
        {
            // 차이에 비례한 속도로 증가
            long difference = targetValue - currentDiplayValue;
            long increment = (long)Mathf.Max(1f, Mathf.Abs(difference) * animationSpeed * Time.deltaTime);

            if (currentDiplayValue < targetValue)
            {
                currentDiplayValue += increment;
                if (currentDiplayValue > difference)
                {
                    currentDiplayValue = targetValue;
                }
            }
            else if (currentDiplayValue > targetValue)
            {
                currentDiplayValue -= increment;

                if(currentDiplayValue < targetValue)
                { 
                    currentDiplayValue = targetValue; 
                }
            }

            UpdateDisplay();
        }

    }
    private void UpdateDisplay()
    {
        if(creditText != null)
        {
            string formattedNumber = NotateNumber.ChangeNumber(currentDiplayValue);
            creditText.text = formattedNumber;
        }
    }
}