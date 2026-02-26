using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProcessorUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image inputIcon;
    [SerializeField] private TextMeshProUGUI inputCountText;

    [SerializeField] private Image progressBar; // Fill Amount를 사용하는 이미지
    [SerializeField] private TextMeshProUGUI timeText; // 남은 시간 표시 (선택사항)

    [SerializeField] private Image outputIcon;
    [SerializeField] private TextMeshProUGUI outputCountText;

    [SerializeField] private GameObject uiPanel; // 가공 중이 아닐 때 숨기기 용도

    private void Awake()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
    }

    // 가공 시작 시 초기 세팅
    public void SetupUI(ItemDataSO inputData, ItemDataSO outputData, int totalInput, int totalOutput)
    {
        if (uiPanel != null) uiPanel.SetActive(true);

        if (inputIcon != null) inputIcon.sprite = inputData.icon;
        // 고정값이 아닌 전달받은 totalInput 표시
        if (inputCountText != null) inputCountText.text = totalInput.ToString();

        if (outputIcon != null) outputIcon.sprite = outputData.icon;
        // 고정값이 아닌 전달받은 totalOutput 표시
        if (outputCountText != null) outputCountText.text = totalOutput.ToString();

        Debug.Log($"[UI 업데이트] 투입재료:{totalInput}, 예상결과:{totalOutput}");
    }

    // 매 프레임 진행도 업데이트
    public void UpdateProgress(float normalizedTime, float remainingTime)
    {
        if (progressBar != null) progressBar.fillAmount = normalizedTime;
        if (timeText != null) timeText.text = remainingTime.ToString("F1") + "s";
    }

    public void CloseUI()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
    }
}