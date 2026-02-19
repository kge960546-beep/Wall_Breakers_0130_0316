using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 업그레이드 효과 중앙 집계 매니저
/// - 활성 노드 전체 재집계
/// - 상태 기반 계산
/// - 타입별 이벤트 발행
/// </summary>
public class UpgradeEffectManager : MonoBehaviour
{
    public static UpgradeEffectManager Instance;

    [Header("Graph Reference")]
    [SerializeField] private UpgradeGraphBuilder graphBuilder;

    // =========================
    // 누적 결과 값
    // =========================

    private float totalPlayerMoveSpeed;
    private float totalPlayerMineSpeed;
    private int totalPlayerMaxCarry;
    private float totalPlayerMineAmount;
    private float totalPlayerSellPrice;

    private int totalMinerUnlockCount;
    private float totalMinerMineSpeed;

    private bool carrierAUnlocked;
    private float carrierAMoveSpeed;
    private int carrierAMaxCarry;

    private bool carrierBUnlocked;
    private float carrierBMoveSpeed;
    private int carrierBMaxCarry;

    private int miningAreaMaxStorage;
    private float miningAreaRespawnReduce;

    private int processorMaxStorage;
    private float processorProcessTimeReduce;

    private float miningOneTimeAmountIncrease;
    private float processedItemSellPriceIncrease;

    // =========================
    // 이벤트
    // =========================

    public event Action<float> OnPlayerMoveSpeedChanged;
    public event Action<float> OnPlayerMineSpeedChanged;
    public event Action<int> OnPlayerMaxCarryChanged;
    public event Action<float> OnPlayerMineAmountChanged;
    public event Action<float> OnPlayerSellPriceChanged;

    public event Action<int> OnMinerUnlockChanged;
    public event Action<float> OnMinerMineSpeedChanged;

    public event Action<bool> OnCarrierAUnlockChanged;
    public event Action<float> OnCarrierAMoveSpeedChanged;
    public event Action<int> OnCarrierAMaxCarryChanged;

    public event Action<bool> OnCarrierBUnlockChanged;
    public event Action<float> OnCarrierBMoveSpeedChanged;
    public event Action<int> OnCarrierBMaxCarryChanged;

    public event Action<int> OnMiningAreaMaxStorageChanged;
    public event Action<float> OnMiningAreaRespawnReduceChanged;

    public event Action<int> OnProcessorMaxStorageChanged;
    public event Action<float> OnProcessorProcessTimeReduceChanged;

    public event Action<float> OnMiningOneTimeAmountIncreaseChanged;
    public event Action<float> OnProcessedItemSellPriceIncreaseChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // =========================
    // 핵심 재집계 메서드
    // =========================

    public void RecalculateAllEffects()
    {
        ResetAllTotals();

        if (graphBuilder == null)
            return;

        List<UpgradeGraphNode<UpgradeDataSO>> activated =
            graphBuilder.dag.GetActivatedNodes();

        foreach (var node in activated)
        {
            foreach (var effect in node.Data.effects)
            {
                AccumulateEffect(effect);
            }
        }

        DispatchAllEvents();
    }

    // =========================
    // 누적 계산
    // =========================

    private void AccumulateEffect(UpgradeEffect effect)
    {
        switch (effect.upgradeType)
        {
            case UpgradeType.PlayerMoveSpeed:
                totalPlayerMoveSpeed += effect.value;
                break;

            case UpgradeType.PlayerMineSpeed:
                totalPlayerMineSpeed += effect.value;
                break;

            case UpgradeType.PlayerMaxCarry:
                totalPlayerMaxCarry += (int)effect.value;
                break;

            case UpgradeType.PlayerMineAmount:
                totalPlayerMineAmount += effect.value;
                break;

            case UpgradeType.PlayerSellPrice:
                totalPlayerSellPrice += effect.value;
                break;

            case UpgradeType.MinerUnlock:
                totalMinerUnlockCount += (int)effect.value;
                break;

            case UpgradeType.MinerMineSpeed:
                totalMinerMineSpeed += effect.value;
                break;

            case UpgradeType.CarrierAUnlock:
                carrierAUnlocked = true;
                break;

            case UpgradeType.CarrierAMoveSpeed:
                carrierAMoveSpeed += effect.value;
                break;

            case UpgradeType.CarrierAMaxCarry:
                carrierAMaxCarry += (int)effect.value;
                break;

            case UpgradeType.CarrierBUnlock:
                carrierBUnlocked = true;
                break;

            case UpgradeType.CarrierBMoveSpeed:
                carrierBMoveSpeed += effect.value;
                break;

            case UpgradeType.CarrierBMaxCarry:
                carrierBMaxCarry += (int)effect.value;
                break;

            case UpgradeType.MiningAreaMaxStorage:
                miningAreaMaxStorage += (int)effect.value;
                break;

            case UpgradeType.MiningAreaRespawnReduce:
                miningAreaRespawnReduce += effect.value;
                break;

            case UpgradeType.ProcessorMaxStorage:
                processorMaxStorage += (int)effect.value;
                break;

            case UpgradeType.ProcessorProcessTimeReduce:
                processorProcessTimeReduce += effect.value;
                break;

            case UpgradeType.MiningOneTimeAmountIncrease:
                miningOneTimeAmountIncrease += effect.value;
                break;

            case UpgradeType.ProcessedItemSellPriceIncrease:
                processedItemSellPriceIncrease += effect.value;
                break;
        }
    }

    private void ResetAllTotals()
    {
        totalPlayerMoveSpeed = 0f;
        totalPlayerMineSpeed = 0f;
        totalPlayerMaxCarry = 0;
        totalPlayerMineAmount = 0f;
        totalPlayerSellPrice = 0f;

        totalMinerUnlockCount = 0;
        totalMinerMineSpeed = 0f;

        carrierAUnlocked = false;
        carrierAMoveSpeed = 0f;
        carrierAMaxCarry = 0;

        carrierBUnlocked = false;
        carrierBMoveSpeed = 0f;
        carrierBMaxCarry = 0;

        miningAreaMaxStorage = 0;
        miningAreaRespawnReduce = 0f;

        processorMaxStorage = 0;
        processorProcessTimeReduce = 0f;

        miningOneTimeAmountIncrease = 0f;
        processedItemSellPriceIncrease = 0f;
    }

    private void DispatchAllEvents()
    {
        OnPlayerMoveSpeedChanged?.Invoke(totalPlayerMoveSpeed);
        OnPlayerMineSpeedChanged?.Invoke(totalPlayerMineSpeed);
        OnPlayerMaxCarryChanged?.Invoke(totalPlayerMaxCarry);
        OnPlayerMineAmountChanged?.Invoke(totalPlayerMineAmount);
        OnPlayerSellPriceChanged?.Invoke(totalPlayerSellPrice);

        OnMinerUnlockChanged?.Invoke(totalMinerUnlockCount);
        OnMinerMineSpeedChanged?.Invoke(totalMinerMineSpeed);

        OnCarrierAUnlockChanged?.Invoke(carrierAUnlocked);
        OnCarrierAMoveSpeedChanged?.Invoke(carrierAMoveSpeed);
        OnCarrierAMaxCarryChanged?.Invoke(carrierAMaxCarry);

        OnCarrierBUnlockChanged?.Invoke(carrierBUnlocked);
        OnCarrierBMoveSpeedChanged?.Invoke(carrierBMoveSpeed);
        OnCarrierBMaxCarryChanged?.Invoke(carrierBMaxCarry);

        OnMiningAreaMaxStorageChanged?.Invoke(miningAreaMaxStorage);
        OnMiningAreaRespawnReduceChanged?.Invoke(miningAreaRespawnReduce);

        OnProcessorMaxStorageChanged?.Invoke(processorMaxStorage);
        OnProcessorProcessTimeReduceChanged?.Invoke(processorProcessTimeReduce);

        OnMiningOneTimeAmountIncreaseChanged?.Invoke(miningOneTimeAmountIncrease);
        OnProcessedItemSellPriceIncreaseChanged?.Invoke(processedItemSellPriceIncrease);
    }
}
