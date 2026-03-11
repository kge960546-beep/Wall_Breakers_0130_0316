using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    public event Action OnInventoryChanged;

    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();

    private CreditService creditService;

    public List<InventoryItem> Items => items;

    [Header("Kwen추가")]
    public Inventory inventory = new Inventory();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeCreditService();
    }

    private void InitializeCreditService()
    {
        if (GameManager.Instance == null)
        {
            Invoke(nameof(InitializeCreditService), 0.1f);
            return;
        }

        creditService = GameManager.Instance.GetService<CreditService>();

        if (creditService != null)
        {
            Utils.DebugLog("[CreditCollector] CreditService successfully initialized");
        }
        else
        {
            Invoke(nameof(InitializeCreditService), 0.1f);
        }
    }

    // 아이템 추가
    public void AddItem(ItemDataSO itemData, int quantity)
    {
        inventory.AddInventory(itemData, quantity);

        Utils.DebugLog($"획득  이름: {itemData.itemName} 갯수 {quantity} 총: {inventory.Items[itemData]}");


        //var existingItem = items.Find(i => i.itemData == itemData);
        //
        //if (existingItem != null)
        //{
        //    existingItem.quantity += quantity;
        //}
        //else
        //{
        //    items.Add(new InventoryItem(itemData, quantity));
        //}

        OnInventoryChanged?.Invoke();
    }

    // 아이템 제거
    public bool RemoveItem(ItemDataSO itemData, int quantity)
    {
        if (!inventory.Items.ContainsKey(itemData))
        {
            return false;
        }

        if (inventory.Items[itemData] < quantity)
        {
            return false;
        }

        inventory.Remove(itemData, quantity);

        OnInventoryChanged?.Invoke();

        return true;

        //var item = items.Find(i => i.itemData == itemData);
        //
        //if (item != null && item.quantity >= quantity)
        //{
        //    item.quantity -= quantity;
        //
        //    if (item.quantity <= 0)
        //    {
        //        items.Remove(item);
        //    }
        //
        //    return true;
        //}
        //
        //return false;
    }

    // 크레딧 추가 - CreditService 사용
    public void AddCredits(int amount)
    {
        if (creditService != null)
        {
            creditService.AddCredit(amount);
            Utils.DebugLog($"Credits added: {amount}. Total: {creditService.credits}");
        }
        else
        {
            Utils.DebugLogError("CreditService is null!");
        }
    }

    // 가장 비싼 아이템 가져오기
    //public InventoryItem GetMostExpensiveItem()
    //{
    //    return items
    //        .Where(i => i.quantity > 0)
    //        .OrderByDescending(i => i.itemData.sellPrice)
    //        .FirstOrDefault();
    //}
    public ItemDataSO GetItem()
    {
        foreach (var item in inventory.Items)
        {
            if (item.Value > 0)
            {
                return item.Key;
            }
        }

        return null;
    }

    //갯수 확인
    public int GetItemCount(ItemDataSO itemData)
    {
        if (inventory.Items.ContainsKey(itemData))
        {
            return inventory.Items[itemData];
        }
        return 0;
    }
}