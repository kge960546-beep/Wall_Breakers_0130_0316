using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class BackpackCapacityBarUI : MonoBehaviour
{
    [Header("Top Bar UI")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI capacityText;

    [Header("Item List UI Settings")]
    [SerializeField] private Transform listParent;    
    [SerializeField] private GameObject itemRowPrefab; 

    [Header("Bar Colors")]
    [SerializeField] private Color green = Color.green;
    [SerializeField] private Color yellow = Color.yellow;
    [SerializeField] private Color red = Color.red;

    // 생성된 리스트 UI 객체들을 관리
    private List<GameObject> spawnedRows = new List<GameObject>();

    private void Start()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged += Refresh;

        if (StackBackPack.Instance != null)
            StackBackPack.Instance.OnBackpackChanged += Refresh;

        Refresh();
    }

    private void OnDestroy()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnInventoryChanged -= Refresh;

        if (StackBackPack.Instance != null)
            StackBackPack.Instance.OnBackpackChanged -= Refresh;
    }

    public void Refresh()
    {
        if (PlayerInventory.Instance == null || StackBackPack.Instance == null || fillImage == null)
            return;

        // 1. 전체 수량 및 바 업데이트
        int current = PlayerInventory.Instance.inventory.TotalCount;
        int max = StackBackPack.Instance.MaxCapacity;

        if (capacityText != null)
            capacityText.text = $"{current} / {max}";

        float ratio = max <= 0 ? 0f : (float)current / max;
        fillImage.fillAmount = ratio;
        UpdateColor(ratio);

        // 2. 상세 아이템 리스트 업데이트
        UpdateItemList();
    }

    private void UpdateItemList()
    {
        // 기존 리스트 UI 삭제
        foreach (var row in spawnedRows)
        {
            if (row != null) Destroy(row);
        }
        spawnedRows.Clear();

        if (itemRowPrefab == null || listParent == null) return;

        var sortedList = PlayerInventory.Instance.inventory.Items
            .Where(pair => pair.Value > 0)
            .ToList(); // 일단 리스트로 변환

        // 타입별로 정렬 (원석 0순위, 가공품 1순위)
        sortedList.Sort((a, b) => {
            int typeA = (int)a.Key.itemType;
            int typeB = (int)b.Key.itemType;

            if (typeA != typeB)
                return typeA.CompareTo(typeB); 

            return a.Key.itemName.CompareTo(b.Key.itemName); 
        });

        foreach (var pair in sortedList)
        {
            ItemDataSO data = pair.Key;
            int count = pair.Value;

            GameObject newRow = Instantiate(itemRowPrefab, listParent);
            spawnedRows.Add(newRow);

            // UI Hierarchy 상의 순서를 정렬 순서와 일치시킴
            newRow.transform.SetAsLastSibling();

            // 데이터 반영
            Image iconImage = newRow.transform.Find("ItemIcon")?.GetComponent<Image>();
            TextMeshProUGUI countText = newRow.transform.Find("ItemCount")?.GetComponent<TextMeshProUGUI>();

            if (iconImage != null) iconImage.sprite = data.icon;
            if (countText != null) countText.text = count.ToString();
        }

        // 레이아웃 즉시 갱신
        LayoutRebuilder.ForceRebuildLayoutImmediate(listParent.GetComponent<RectTransform>());
    }

    private void UpdateColor(float ratio)
    {
        if (ratio >= 1f) fillImage.color = red;
        else if (ratio >= 0.5f) fillImage.color = yellow;
        else fillImage.color = green;
    }
}