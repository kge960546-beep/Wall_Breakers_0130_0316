using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();

    private CreditService creditService;

    public List<InventoryItem> Items => items;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        creditService = GameManager.Instance.GetService<CreditService>();
    }

    // 아이템 추가
    public void AddItem(ItemData itemData, int quantity)
    {
        var existingItem = items.Find(i => i.itemData == itemData);

        if (existingItem != null)
        {
            existingItem.quantity += quantity;
        }
        else
        {
            items.Add(new InventoryItem(itemData, quantity));
        }
    }

    // 아이템 제거
    public bool RemoveItem(ItemData itemData, int quantity)
    {
        var item = items.Find(i => i.itemData == itemData);

        if (item != null && item.quantity >= quantity)
        {
            item.quantity -= quantity;

            if (item.quantity <= 0)
            {
                items.Remove(item);
            }

            return true;
        }

        return false;
    }

    // 크레딧 추가 - CreditService 사용
    public void AddCredits(int amount)
    {
        if (creditService != null)
        {
            creditService.AddCredit(amount);
            Debug.Log($"Credits added: {amount}. Total: {creditService.credits}");
        }
        else
        {
            Debug.LogError("CreditService is null!");
        }
    }

    // 가장 비싼 아이템 가져오기
    public InventoryItem GetMostExpensiveItem()
    {
        return items
            .Where(i => i.quantity > 0)
            .OrderByDescending(i => i.itemData.sellPrice)
            .FirstOrDefault();
    }
}