using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GuidePanelToggle : MonoBehaviour
{
    [SerializeField] private RectTransform guidePanel;
    [SerializeField] private RectTransform toggleButton;

    [Header("Animation")]
    [SerializeField] private float duration = 0.25f;

    private bool isOpen = true;
    private bool isAnimating = false;

    private Button button;
    private Vector2 originalPos;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickToggle);
        originalPos = guidePanel.anchoredPosition;
    }

    private void OnClickToggle()
    {
        if (isAnimating) return;

        if (isOpen)
            StartCoroutine(ClosePanel());
        else
            StartCoroutine(OpenPanel());
    }

    private IEnumerator ClosePanel()
    {
        isAnimating = true;

        Vector2 startPos = guidePanel.anchoredPosition;

        Vector2 buttonLocal;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            guidePanel.parent as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, toggleButton.position),
            null,
            out buttonLocal
        );

        Vector2 offset = buttonLocal - startPos;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float scale = Mathf.Lerp(1f, 0f, t);

            guidePanel.localScale = Vector3.one * scale;

            guidePanel.anchoredPosition = startPos + offset * (1f - scale);

            yield return null;
        }

        guidePanel.localScale = Vector3.zero;
        guidePanel.anchoredPosition = buttonLocal;

        guidePanel.gameObject.SetActive(false);

        guidePanel.anchoredPosition = originalPos;

        isOpen = false;
        isAnimating = false;
    }

    private IEnumerator OpenPanel()
    {
        isAnimating = true;

        guidePanel.gameObject.SetActive(true);

        Vector2 buttonLocal;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            guidePanel.parent as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, toggleButton.position),
            null,
            out buttonLocal
        );

        Vector2 startPos = originalPos;
        Vector2 offset = buttonLocal - startPos;

        guidePanel.anchoredPosition = buttonLocal;
        guidePanel.localScale = Vector3.zero;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float scale = Mathf.Lerp(0f, 1f, t);

            guidePanel.localScale = Vector3.one * scale;

            guidePanel.anchoredPosition = startPos + offset * (1f - scale);

            yield return null;
        }

        guidePanel.localScale = Vector3.one;
        guidePanel.anchoredPosition = originalPos;

        isOpen = true;
        isAnimating = false;
    }
}