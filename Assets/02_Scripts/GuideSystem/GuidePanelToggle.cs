using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GuidePanelToggle : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform guidePanel;

    [Header("Animation")]
    [SerializeField] private float duration = 0.25f;

    private bool isOpen = true;
    private bool isAnimating = false;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickToggle);
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

        yield return UITween.Scale(guidePanel, Vector3.one, Vector3.zero, duration);

        guidePanel.gameObject.SetActive(false);

        isOpen = false;
        isAnimating = false;
    }

    private IEnumerator OpenPanel()
    {
        isAnimating = true;

        guidePanel.gameObject.SetActive(true);

        guidePanel.localScale = Vector3.zero;

        yield return UITween.Scale(guidePanel, Vector3.zero, Vector3.one, duration);

        isOpen = true;
        isAnimating = false;
    }
}