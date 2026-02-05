using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 판매 로직 관리
/// </summary>
public class SellManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ShopLevelData shopData;
    [SerializeField] private SellUI sellUI;

    [Header("Events")]
    public UnityEvent<InventoryItem, int> OnItemSold; // 아이템, 획득 크레딧

    private bool isSelling = false;
    private Coroutine sellCoroutine;

    public void StartSelling()
    {
        if (isSelling) return;

        isSelling = true;
        sellUI.Show();
        sellCoroutine = StartCoroutine(SellCycle());

        Debug.Log("Started selling");
    }

    public void StopSelling()
    {
        if (!isSelling) return;

        isSelling = false;

        if (sellCoroutine != null)
        {
            StopCoroutine(sellCoroutine);
            sellCoroutine = null;
        }

        sellUI.Hide();
        Debug.Log("Stopped selling");
    }

    private IEnumerator SellCycle()
    {
        while (isSelling)
        {
            var itemToSell = PlayerInventory.Instance.GetMostExpensiveItem();

            if (itemToSell == null)
            {
                // 판매할 아이템이 없으면 대기
                sellUI.UpdateDisplay(null, 0);
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // 판매 시간 계산 (상점 레벨에 따른 배수 적용)
            float sellDuration = itemToSell.itemData.sellDuration * shopData.GetSpeedMultiplier();
            int earnedCredits = itemToSell.itemData.sellPrice;

            // UI 업데이트 (판매 시작)
            sellUI.UpdateDisplay(itemToSell, sellDuration);

            // 판매 진행 시간
            float elapsedTime = 0f;

            while (elapsedTime < sellDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / sellDuration;
                sellUI.UpdateProgress(progress);
                yield return null;
            }

            // 아이템 판매 완료
            if (PlayerInventory.Instance.RemoveItem(itemToSell.itemData, 1))
            {
                PlayerInventory.Instance.AddCredits(earnedCredits);
                OnItemSold?.Invoke(itemToSell, earnedCredits);

                Debug.Log($"Sold {itemToSell.itemData.itemName} for {earnedCredits} credits");
            }

            // 다음 판매 사이클까지 짧은 대기
            yield return new WaitForSeconds(0.1f);
        }
    }
}
