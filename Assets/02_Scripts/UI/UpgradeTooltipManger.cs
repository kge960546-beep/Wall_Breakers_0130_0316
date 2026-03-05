using UnityEngine;

public class UpgradeTooltipManager : MonoBehaviour
{
    public static UpgradeTooltipManager Instance;

    [SerializeField] private UpgradeUIDescPanel tooltipPanel;

    private RectTransform rect;
    private Canvas canvas;

    private bool isVisible;

    private void Awake()
    {
        Instance = this;

        rect = tooltipPanel.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        tooltipPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isVisible) return;

        Vector2 mousePos = Input.mousePosition;

        float panelHeight = rect.rect.height;

        // 기본 pivot (왼쪽 상단)
        Vector2 pivot = new Vector2(0f, 1f);

        // 화면 위로 나가는지 체크
        if (mousePos.y - panelHeight < 0)
        {
            // 왼쪽 하단으로 변경
            pivot = new Vector2(0f, 0f);
        }

        rect.pivot = pivot;
        rect.position = mousePos;
    }

    public void Show(Sprite icon, string name, string desc, int cost)
    {
        tooltipPanel.gameObject.SetActive(true);

        tooltipPanel.SetInfo(icon, name, desc, cost);

        isVisible = true;
    }

    public void Hide()
    {
        tooltipPanel.gameObject.SetActive(false);

        isVisible = false;
    }
}