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
    [SerializeField] private CreditSpawner creditSpawner;

    [Header("Events")]
    public UnityEvent<ItemDataSO, int> OnItemSold; // 아이템, 획득 크레딧, 전: InventoryItem

    private bool isSelling = false;
    private Coroutine sellCoroutine;

    [Header("kwen 추가")]
    [SerializeField] StackBackPack stackBackPack;

    private void Start()
    {
        if (stackBackPack == null)
        {
            stackBackPack = FindObjectOfType<StackBackPack>();
        }

        // 초기 설정 확인
        ValidateSetup();
    }

    private void ValidateSetup()
    {
        if (shopData == null)
        {
            Debug.LogError("[SellManager] ShopData is NOT assigned!");
        }

        if (sellUI == null)
        {
            Debug.LogError("[SellManager] SellUI is NOT assigned!");
        }

        if (creditSpawner == null)
        {
            Debug.LogError("[SellManager] CreditSpawner is NOT assigned!");
        }
        else
        {
            Debug.Log("[SellManager] CreditSpawner is properly assigned");
        }
    }

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
            //var itemToSell = PlayerInventory.Instance.GetMostExpensiveItem();            

            //ItemDataSO itemData = PlayerInventory.Instance.GetItem();

            //if (itemData == null) //전 itemToSell == null
            //{
            //    // 판매할 아이템이 없으면 대기
            //    sellUI.UpdateDisplay(null, 0);
            //    yield return new WaitForSeconds(0.5f);
            //    continue;
            //}

            GameObject topItem = stackBackPack.PeekResource();
            if (topItem == null)
            {
                // 판매할 아이템이 없으면 대기
                sellUI.UpdateDisplay(null, 0);
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            MineralItem mineral = topItem.GetComponent<MineralItem>();
            ItemDataSO itemData = mineral.mineralData;

            // 판매 시간 계산 (상점 레벨에 따른 배수 적용)
            float sellDuration = itemData.sellDuration * shopData.GetSpeedMultiplier(); //전 itemToSell.itemData.sellDuration
            int earnedCredits = itemData.sellPrice;

            // UI 업데이트 (판매 시작)
            sellUI.UpdateDisplay(itemData, sellDuration); //전 itemToSell

            // 판매 진행 시간
            float elapsedTime = 0f;

            while (elapsedTime < sellDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / sellDuration;
                sellUI.UpdateProgress(progress);
                yield return null;
            }

            GameObject soldItem = stackBackPack.MinusResource();

            // 아이템 판매 완료
            if (soldItem != null) //전 PlayerInventory.Instance.RemoveItem(itemToSell.itemData, 1)
            {
                // 크레딧을 직접 추가하지 않고 오브젝트로 생성
                creditSpawner.SpawnCredit(earnedCredits);

                OnItemSold?.Invoke(itemData, earnedCredits); //전 itemToSell

                Debug.Log($"Sold {itemData.itemName} for {earnedCredits} credits"); //전 itemToSell

                if(itemData.mineralPrefab != null)
                {
                    PoolManager.instance.ReturnIt(itemData.mineralPrefab, soldItem);
                }
                else
                {
                    Destroy(soldItem);
                }
            }

            // 다음 판매 사이클까지 짧은 대기
            yield return new WaitForSeconds(0.1f);
        }
    }
}
