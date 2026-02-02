using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellSlotUI : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amountText;

    public SellSlot slot;

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag.GetComponent<MineralItemUI>();

        if (dragged == null)
            return;

        SetSlot(dragged.Mineral, dragged.Amount);
        dragged.ConsumeAll();
    }
    public void SetSlot(MineralData mineral, int amount)
    {
        slot.Set(mineral, amount);

        icon.sprite = mineral.icon;
        icon.enabled = true;
        amountText.text = amount.ToString();
    }
}