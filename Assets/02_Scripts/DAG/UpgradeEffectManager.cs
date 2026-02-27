using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 업그레이드 효과 중앙 집계 매니저
/// - ID 기반 Miner / Carrier 완전 통합 구조
/// </summary>
public class UpgradeEffectManager : MonoBehaviour
{
    public static UpgradeEffectManager Instance;

    [Header("Graph Reference")]
    [SerializeField] private UpgradeGraphBuilder graphBuilder;

    // =========================
    // 플레이어 누적값(총합 저장용 변수)
    // =========================

    private float totalPlayerMoveSpeed;
    private float totalPlayerMineSpeed;
    private int totalPlayerMaxCarry;
    private float totalPlayerMineAmount;
    private float totalPlayerRawSellPrice;
    private float totalProcessedSellPrice;

    private int totalMinerUnlockCount;

    // =========================
    // ID 기반 자동화 유닛(유닛ID와 누적된 강화수치를 개별 적용하기 위함)
    // =========================

    private Dictionary<string, float> minerMineSpeedById = new();
    private Dictionary<string, bool> carrierUnlockById = new();
    private Dictionary<string, float> carrierMoveSpeedById = new();
    private Dictionary<string, int> carrierMaxCarryById = new();
    private Dictionary<string, int> miningAreaMaxStorageById = new();
    private Dictionary<string, float> miningAreaRespawnReduceById = new();
    private Dictionary<string, int> processingMaxCapacityById = new();
    private Dictionary<string, float> processorProcessTimeReduceById = new();
    private Dictionary<string, int> minerMineAmountById = new();

    // =========================
    // 이벤트
    // =========================

    public event Action<float> OnPlayerMoveSpeedChanged;
    public event Action<float> OnPlayerMineSpeedChanged;
    public event Action<int> OnPlayerMaxCarryChanged;
    public event Action<float> OnPlayerMineAmountChanged;
    public event Action<float> OnPlayerRawSellPriceChanged;
    public event Action<float> OnProcessedSellPriceChanged;

    public event Action<int> OnMinerUnlockChanged;
    public event Action<string, float> OnMinerMineSpeedChanged;

    public event Action<string, bool> OnCarrierUnlockChanged;
    public event Action<string, float> OnCarrierMoveSpeedChanged;
    public event Action<string, int> OnCarrierMaxCarryChanged;

    public event Action<string, int> OnMiningAreaMaxStorageChanged;
    public event Action<string, float> OnMiningAreaRespawnTimeChanged;

    public event Action<string, int> OnProcessingMaxCapacityChanged;
    public event Action<string, float> OnProcessorProcessTimeChanged;

    public event Action<string, int> OnMinerMineAmountChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        RecalculateAllEffects();
    }

    // 기존 누적값 초기화
    // 현재 활성화된 노드 가져오기
    // 각 노드의 effect순회
    // Accumulate로 누적
    // Dispatch로 통보하는 형태
    public void RecalculateAllEffects()
    {

        ResetAllTotals();

        if (graphBuilder == null) return;

        var activated = graphBuilder.DAG.GetActivatedNodes();

        foreach (var node in activated)
        {
            foreach (var effect in node.Data.effects)
            {
                AccumulateEffect(effect);
            }
        }

        DispatchAllEvents();
    }

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

            case UpgradeType.PlayerRawSellPrice:
                totalPlayerRawSellPrice += effect.value;
                break;

            case UpgradeType.ProcessedSellPrice:
                totalProcessedSellPrice += effect.value;
                break;

            case UpgradeType.MinerUnlock:
                totalMinerUnlockCount += (int)effect.value;
                break;

            case UpgradeType.MinerMineSpeed:
                if (!string.IsNullOrEmpty(effect.targetID))
                {
                    if (!minerMineSpeedById.ContainsKey(effect.targetID))
                        minerMineSpeedById[effect.targetID] = 0f;

                    minerMineSpeedById[effect.targetID] += effect.value;
                }
                break;

            case UpgradeType.CarrierUnlock:
                if (!string.IsNullOrEmpty(effect.targetID))
                    carrierUnlockById[effect.targetID] = true;
                break;

            case UpgradeType.CarrierMoveSpeed:
                if (!string.IsNullOrEmpty(effect.targetID))
                {
                    if (!carrierMoveSpeedById.ContainsKey(effect.targetID))
                        carrierMoveSpeedById[effect.targetID] = 0f;

                    carrierMoveSpeedById[effect.targetID] += effect.value;
                }
                break;

            case UpgradeType.CarrierMaxCarry:
                if (!string.IsNullOrEmpty(effect.targetID))
                {
                    if (!carrierMaxCarryById.ContainsKey(effect.targetID))
                        carrierMaxCarryById[effect.targetID] = 0;

                    carrierMaxCarryById[effect.targetID] += (int)effect.value;
                }
                break;

            case UpgradeType.MiningAreaMaxStorage:
                if (!string.IsNullOrEmpty(effect.targetID))
                {
                    if (!miningAreaMaxStorageById.ContainsKey(effect.targetID))
                        miningAreaMaxStorageById[effect.targetID] = 0;

                    miningAreaMaxStorageById[effect.targetID] += (int)effect.value;
                }
                break;

            case UpgradeType.MiningAreaRespawnReduce:
                if (!string.IsNullOrEmpty(effect.targetID))
                {
                    if (!miningAreaRespawnReduceById.ContainsKey(effect.targetID))
                        miningAreaRespawnReduceById[effect.targetID] = 0f;

                    miningAreaRespawnReduceById[effect.targetID] += effect.value;
                }
                break;

            case UpgradeType.ProcessorMaxStorage:
                if (!string.IsNullOrEmpty(effect.targetID))
                {
                    if (!processingMaxCapacityById.ContainsKey(effect.targetID))
                        processingMaxCapacityById[effect.targetID] = 0;

                    processingMaxCapacityById[effect.targetID] += (int)effect.value;
                }
                break;

            case UpgradeType.ProcessorProcessTimeReduce:
                if (!string.IsNullOrEmpty(effect.targetID))
                {
                    if (!processorProcessTimeReduceById.ContainsKey(effect.targetID))
                        processorProcessTimeReduceById[effect.targetID] = 0f;

                    processorProcessTimeReduceById[effect.targetID] += effect.value;
                }
                break;

            case UpgradeType.MinerMineAmount:
                if (!string.IsNullOrEmpty(effect.targetID))
                {
                    if (!minerMineAmountById.ContainsKey(effect.targetID))
                        minerMineAmountById[effect.targetID] = 0;

                    minerMineAmountById[effect.targetID] += (int)effect.value;
                }
                break;
        }
    }

    // 기존 강화 누적값 전부 0으로 초기화
    // 증분 방식이 아닌 재계산 방식이라
    // 매번 처음부터 다시 합산하는 형태
    private void ResetAllTotals()
    {
        totalPlayerMoveSpeed = 0f;
        totalPlayerMineSpeed = 0f;
        totalPlayerMaxCarry = 0;
        totalPlayerMineAmount = 0f;
        totalPlayerRawSellPrice = 0f;
        totalProcessedSellPrice = 0f;

        totalMinerUnlockCount = 0;

        minerMineSpeedById.Clear();
        carrierUnlockById.Clear();
        carrierMoveSpeedById.Clear();
        carrierMaxCarryById.Clear();
        miningAreaMaxStorageById.Clear();
        miningAreaRespawnReduceById.Clear();
        processingMaxCapacityById.Clear();
        processorProcessTimeReduceById.Clear();
        minerMineAmountById.Clear();
    }

    // 이벤트 통보
    private void DispatchAllEvents()
    {
        OnPlayerMoveSpeedChanged?.Invoke(totalPlayerMoveSpeed);
        OnPlayerMineSpeedChanged?.Invoke(totalPlayerMineSpeed);
        OnPlayerMaxCarryChanged?.Invoke(totalPlayerMaxCarry);
        OnPlayerMineAmountChanged?.Invoke(totalPlayerMineAmount);
        OnPlayerRawSellPriceChanged?.Invoke(totalPlayerRawSellPrice);
        OnProcessedSellPriceChanged?.Invoke(totalProcessedSellPrice);

        OnMinerUnlockChanged?.Invoke(totalMinerUnlockCount);

        foreach (var pair in minerMineSpeedById)
            OnMinerMineSpeedChanged?.Invoke(pair.Key, pair.Value);

        foreach (var pair in carrierUnlockById.ToList())
            OnCarrierUnlockChanged?.Invoke(pair.Key, pair.Value);

        foreach (var pair in carrierMoveSpeedById.ToList())
            OnCarrierMoveSpeedChanged?.Invoke(pair.Key, pair.Value);

        foreach (var pair in carrierMaxCarryById.ToList())
            OnCarrierMaxCarryChanged?.Invoke(pair.Key, pair.Value);

        foreach (var pair in miningAreaMaxStorageById.ToList())
            OnMiningAreaMaxStorageChanged?.Invoke(pair.Key, pair.Value);

        foreach (var pair in miningAreaRespawnReduceById.ToList())
            OnMiningAreaRespawnTimeChanged?.Invoke(pair.Key, pair.Value);

        foreach (var pair in processingMaxCapacityById.ToList())
            OnProcessingMaxCapacityChanged?.Invoke(pair.Key, pair.Value);

        foreach (var pair in processorProcessTimeReduceById.ToList())
            OnProcessorProcessTimeChanged?.Invoke(pair.Key, pair.Value);

        foreach (var pair in minerMineAmountById)
            OnMinerMineAmountChanged?.Invoke(pair.Key, pair.Value);
    }
}
