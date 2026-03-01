using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiningUI : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Image progressBar;
    [SerializeField] private Image mineralIcon;
    [SerializeField] private TextMeshProUGUI amountText;

    private void Awake()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
    }

    public void OpenUI(Sprite icon, int amount)
    {
        if (uiPanel != null) uiPanel.SetActive(true);
        if (mineralIcon != null) mineralIcon.sprite = icon;
        if (amountText != null) amountText.text = "x" + amount.ToString();
    }

    public void UpdateProgress(float normalizedTime)
    {
        if (progressBar != null) progressBar.fillAmount = normalizedTime;
    }

    public void CloseUI()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
    }

    // 채굴량이 실시간으로 바뀔 수 있으니 텍스트만 업데이트하는 함수
    public void UpdateAmount(int amount)
    {
        if (amountText != null) amountText.text = "x" + amount.ToString();
    }
}