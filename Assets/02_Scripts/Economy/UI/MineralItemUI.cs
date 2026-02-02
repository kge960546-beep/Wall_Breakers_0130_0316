using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MineralItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[SerializeField] private Image icon;
	[SerializeField] private Canvas canvas;

	public MineralData Mineral { get; private set; }
	public int Amount {  get; private set; }

	private RectTransform rectTransform;
	private CanvasGroup canvasGroup;

	private Vector2 startPos;

	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public void Bind(MineralData mineral, int amount)
	{
		Mineral = mineral;
		Amount = amount;

		icon.sprite = mineral.icon;
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		startPos = rectTransform.anchoredPosition;
		canvasGroup.blocksRaycasts = false;
	}
	public void OnDrag(PointerEventData eventData)
    {
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
	public void OnEndDrag(PointerEventData eventData)
	{
		canvasGroup.blocksRaycasts = true;
		rectTransform.anchoredPosition = startPos;
	}
	public void ConsumeAll()
	{
		Amount = 0;
		Destroy(gameObject);
	}
}