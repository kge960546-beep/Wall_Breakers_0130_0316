using System;
using UnityEngine;

/// <summary>
/// 업그레이드 효과 중앙 집계 매니저
/// - 모든 업그레이드 효과 누적 관리
/// - 타입별 switch 분기 처리
/// - 각 시스템에 이벤트 발행
/// </summary>
public class UpgradeEffectManager : MonoBehaviour
{
    public static UpgradeEffectManager Instance;

    // =========================
    // 플레이어 직접 강화
    // =========================

    private float totalPlayerMoveSpeed;
    private float totalPlayerMineSpeed;
    private int totalPlayerMaxCarry;
    private float totalPlayerMineAmount;
    private float totalPlayerSellPrice;

    // =========================
    // 자동화 - 광부
    // =========================

    private int totalMinerUnlockCount;
    private float totalMinerMineSpeed;

    // =========================
    // 자동화 - 운반인 A
    // =========================

    private bool carrierAUnlocked;
    private float carrierAMoveSpeed;
    private int carrierAMaxCarry;

    // =========================
    // 자동화 - 운반인 B
    // =========================

    private bool carrierBUnlocked;
    private float carrierBMoveSpeed;
    private int carrierBMaxCarry;

    // =========================
    // 채굴 구역
    // =========================

    private int miningAreaMaxStorage;
    private float miningAreaRespawnReduce;

    // =========================
    // 가공기계
    // =========================

    private int processorMaxStorage;
    private float processorProcessTimeReduce;

    // =========================
    // 최종 특수 효과
    // =========================

    private float miningOneTimeAmountIncrease;
    private float processedItemSellPriceIncrease;

    // =========================
    // 이벤트 영역
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
    // 외부 호출 메서드
    // =========================

    public void ApplyUpgrade(UpgradeDataSO data)
    {
        foreach (var effect in data.effects)
        {
            ApplyEffect(effect);
        }
    }

    // =========================
    // 타입별 분기 처리
    // =========================

    private void ApplyEffect(UpgradeEffect effect)
    {
        switch (effect.upgradeType)
        {
            // =========================
            // Player
            // =========================

            case UpgradeType.PlayerMoveSpeed:
                totalPlayerMoveSpeed += effect.value;
                OnPlayerMoveSpeedChanged?.Invoke(totalPlayerMoveSpeed);
                break;

            case UpgradeType.PlayerMineSpeed:
                totalPlayerMineSpeed += effect.value;
                OnPlayerMineSpeedChanged?.Invoke(totalPlayerMineSpeed);
                break;

            case UpgradeType.PlayerMaxCarry:
                totalPlayerMaxCarry += (int)effect.value;
                OnPlayerMaxCarryChanged?.Invoke(totalPlayerMaxCarry);
                break;

            case UpgradeType.PlayerMineAmount:
                totalPlayerMineAmount += effect.value;
                OnPlayerMineAmountChanged?.Invoke(totalPlayerMineAmount);
                break;

            case UpgradeType.PlayerSellPrice:
                totalPlayerSellPrice += effect.value;
                OnPlayerSellPriceChanged?.Invoke(totalPlayerSellPrice);
                break;

            // =========================
            // Miner
            // =========================

            case UpgradeType.MinerUnlock:
                totalMinerUnlockCount += (int)effect.value;
                OnMinerUnlockChanged?.Invoke(totalMinerUnlockCount);
                break;

            case UpgradeType.MinerMineSpeed:
                totalMinerMineSpeed += effect.value;
                OnMinerMineSpeedChanged?.Invoke(totalMinerMineSpeed);
                break;

            // =========================
            // Carrier A
            // =========================

            case UpgradeType.CarrierAUnlock:
                carrierAUnlocked = true;
                OnCarrierAUnlockChanged?.Invoke(carrierAUnlocked);
                break;

            case UpgradeType.CarrierAMoveSpeed:
                carrierAMoveSpeed += effect.value;
                OnCarrierAMoveSpeedChanged?.Invoke(carrierAMoveSpeed);
                break;

            case UpgradeType.CarrierAMaxCarry:
                carrierAMaxCarry += (int)effect.value;
                OnCarrierAMaxCarryChanged?.Invoke(carrierAMaxCarry);
                break;

            // =========================
            // Carrier B
            // =========================

            case UpgradeType.CarrierBUnlock:
                carrierBUnlocked = true;
                OnCarrierBUnlockChanged?.Invoke(carrierBUnlocked);
                break;

            case UpgradeType.CarrierBMoveSpeed:
                carrierBMoveSpeed += effect.value;
                OnCarrierBMoveSpeedChanged?.Invoke(carrierBMoveSpeed);
                break;

            case UpgradeType.CarrierBMaxCarry:
                carrierBMaxCarry += (int)effect.value;
                OnCarrierBMaxCarryChanged?.Invoke(carrierBMaxCarry);
                break;

            // =========================
            // Mining Area
            // =========================

            case UpgradeType.MiningAreaMaxStorage:
                miningAreaMaxStorage += (int)effect.value;
                OnMiningAreaMaxStorageChanged?.Invoke(miningAreaMaxStorage);
                break;

            case UpgradeType.MiningAreaRespawnReduce:
                miningAreaRespawnReduce += effect.value;
                OnMiningAreaRespawnReduceChanged?.Invoke(miningAreaRespawnReduce);
                break;

            // =========================
            // Processor
            // =========================

            case UpgradeType.ProcessorMaxStorage:
                processorMaxStorage += (int)effect.value;
                OnProcessorMaxStorageChanged?.Invoke(processorMaxStorage);
                break;

            case UpgradeType.ProcessorProcessTimeReduce:
                processorProcessTimeReduce += effect.value;
                OnProcessorProcessTimeReduceChanged?.Invoke(processorProcessTimeReduce);
                break;

            // =========================
            // Final
            // =========================

            case UpgradeType.MiningOneTimeAmountIncrease:
                miningOneTimeAmountIncrease += effect.value;
                OnMiningOneTimeAmountIncreaseChanged?.Invoke(miningOneTimeAmountIncrease);
                break;

            case UpgradeType.ProcessedItemSellPriceIncrease:
                processedItemSellPriceIncrease += effect.value;
                OnProcessedItemSellPriceIncreaseChanged?.Invoke(processedItemSellPriceIncrease);
                break;
        }
    }
}
