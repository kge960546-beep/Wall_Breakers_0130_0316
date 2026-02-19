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
    private float bonusSellPrice = 0f;

    private void OnEnable()
    {
        if (UpgradeEffectManager.Instance != null)
            UpgradeEffectManager.Instance.OnPlayerSellPriceChanged += HandleSellPriceChanged;
    }

    private void OnDisable()
    {
        if (UpgradeEffectManager.Instance != null)
            UpgradeEffectManager.Instance.OnPlayerSellPriceChanged -= HandleSellPriceChanged;
    }

    private void HandleSellPriceChanged(float totalBonus)
    {
        bonusSellPrice = totalBonus;
    }

    private void Start()
    {
        if (stackBackPack == null)
            stackBackPack = FindObjectOfType<StackBackPack>();

        ValidateSetup();
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

            float sellDuration = itemData.sellDuration * shopData.GetSpeedMultiplier();
            int earnedCredits = Mathf.RoundToInt(itemData.sellPrice * (1f + bonusSellPrice));

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
}
