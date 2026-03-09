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
    public UnityEvent<ItemDataSO, int> OnItemSold;

    private bool isSelling = false;
    private Coroutine sellCoroutine;

    [Header("kwen 추가")]
    [SerializeField] StackBackPack stackBackPack;
    [SerializeField] WaitForSeconds wait = new WaitForSeconds(0.5f);
    [SerializeField] WaitForSeconds shortWait = new WaitForSeconds(0.1f);

    // =========================
    // 업그레이드 관련 추가
    // =========================
    private float playerRawSellBonus = 0f;
    private float processedSellBonus = 0f;

    private void OnDisable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnPlayerRawSellPriceChanged -= HandlePlayerRawSellChanged;
            UpgradeEffectManager.Instance.OnProcessedSellPriceChanged -= HandleProcessedSellChanged;
        }
    }

    private void HandlePlayerRawSellChanged(float totalBonus)
    {
        playerRawSellBonus = totalBonus;
    }

    private void HandleProcessedSellChanged(float totalBonus)
    {
        processedSellBonus = totalBonus;
    }

    private void Start()
    {
        if (stackBackPack == null)
            stackBackPack = FindObjectOfType<StackBackPack>();

        ValidateSetup();

        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnPlayerRawSellPriceChanged += HandlePlayerRawSellChanged;
            UpgradeEffectManager.Instance.OnProcessedSellPriceChanged += HandleProcessedSellChanged;
        }
    }

    private void ValidateSetup()
    {
        if (shopData == null)
            Debug.LogError("[SellManager] ShopData is NOT assigned!");

        if (sellUI == null)
            Debug.LogError("[SellManager] SellUI is NOT assigned!");

        if (creditSpawner == null)
            Debug.LogError("[SellManager] CreditSpawner is NOT assigned!");
    }

    public void StartSelling()
    {
        if (isSelling) return;

        isSelling = true;
        sellUI.Show();
        sellCoroutine = StartCoroutine(SellCycle());
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
    }

    private IEnumerator SellCycle()
    {
        while (isSelling)
        {
            GameObject topItem = stackBackPack.PeekResource();
            if (topItem == null)
            {
                sellUI.UpdateDisplay(null, 0, 0);
                yield return wait;
                continue;
            }

            MineralItem mineral = topItem.GetComponent<MineralItem>();
            ItemDataSO itemData = mineral.mineralData;

            float sellDuration = itemData.sellDuration * shopData.GetSpeedMultiplier()*0.3f;

            // ---------------------------
            // 판매 보너스 계산 분기
            // ---------------------------
            float finalBonus = 0f;

            if (itemData.itemType == ItemType.RawMaterial)
            {
                // 플레이어 직접 판매 원석 전용
                finalBonus = playerRawSellBonus;
            }
            else if (itemData.itemType == ItemType.processed)
            {
                // 가공품 전체 판매 증가
                finalBonus = processedSellBonus;
            }

            int earnedCredits = Mathf.RoundToInt(itemData.sellPrice * (1f + finalBonus));

            sellUI.UpdateDisplay(itemData, sellDuration, earnedCredits);

            float elapsedTime = 0f;

            while (elapsedTime < sellDuration)
            {
                elapsedTime += Time.deltaTime;
                sellUI.UpdateProgress(elapsedTime / sellDuration);
                yield return null;
            }

            GameObject soldItem = stackBackPack.MinusResource();

            if (soldItem != null)
            {
                SFXManager.instance.PlayOnSFX("209578__zott820__cash-register-purchase", transform.position);

                creditSpawner.SpawnCredit(earnedCredits);
                OnItemSold?.Invoke(itemData, earnedCredits);

                if (itemData.mineralPrefab != null)
                    PoolManager.instance.ReturnIt(itemData.mineralPrefab, soldItem);
                else
                    Destroy(soldItem);
            }

            yield return shortWait;
        }
    }

    // =========================
    // Carrier 전용 즉시 판매 함수
    // =========================
    public void SellFromCarrier(GameObject item)
    {
        if (item == null) return;

        MineralItem mineral = item.GetComponent<MineralItem>();
        if (mineral == null) return;

        ItemDataSO itemData = mineral.mineralData;
        if (itemData == null) return;

        // ---------------------------
        // 판매 보너스 계산 분기
        // ---------------------------
        float finalBonus = 0f;

        if (itemData.itemType == ItemType.processed)
        {
            // 가공품 전체 판매 증가 적용
            finalBonus = processedSellBonus;
        }
        // RawMaterial은 플레이어 전용이므로 적용 안함

        int earnedCredits = Mathf.RoundToInt(itemData.sellPrice * (1f + finalBonus));

        creditSpawner.SpawnCredit(earnedCredits);
        OnItemSold?.Invoke(itemData, earnedCredits);

        if (itemData.mineralPrefab != null)
            PoolManager.instance.ReturnIt(itemData.mineralPrefab, item);
        else
            Destroy(item);
    }
}
