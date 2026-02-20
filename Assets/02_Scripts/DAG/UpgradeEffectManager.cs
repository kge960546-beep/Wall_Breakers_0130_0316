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
    // 플레이어 누적값
    // =========================

    private float totalPlayerMoveSpeed;
    private float totalPlayerMineSpeed;
    private int totalPlayerMaxCarry;
    private float totalPlayerMineAmount;
    private float totalPlayerSellPrice;

    private int totalMinerUnlockCount;

    // =========================
    // ID 기반 자동화 유닛
    // =========================

    private Dictionary<string, float> minerMineSpeedById = new();
    private Dictionary<string, bool> carrierUnlockById = new();
    private Dictionary<string, float> carrierMoveSpeedById = new();
    private Dictionary<string, int> carrierMaxCarryById = new();
    private Dictionary<string, int> miningAreaMaxStorageById = new();
    private Dictionary<string, float> miningAreaRespawnReduceById = new();
    private Dictionary<string, int> processingMaxCapacityById = new();
    private Dictionary<string, float> processorProcessTimeReduceById = new();

    // =========================
    // 이벤트
    // =========================

    public event Action<float> OnPlayerMoveSpeedChanged;
    public event Action<float> OnPlayerMineSpeedChanged;
    public event Action<int> OnPlayerMaxCarryChanged;
    public event Action<float> OnPlayerMineAmountChanged;
    public event Action<float> OnPlayerSellPriceChanged;

    public event Action<int> OnMinerUnlockChanged;
    public event Action<string, float> OnMinerMineSpeedChanged;

    public event Action<string, bool> OnCarrierUnlockChanged;
    public event Action<string, float> OnCarrierMoveSpeedChanged;
    public event Action<string, int> OnCarrierMaxCarryChanged;

    public event Action<string, int> OnMiningAreaMaxStorageChanged;
    public event Action<string, float> OnMiningAreaRespawnTimeChanged;

    public event Action<string, int> OnProcessingMaxCapacityChanged;
    public event Action<string, float> OnProcessorProcessTimeChanged;

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

    public void RecalculateAllEffects()
    {
        ResetAllTotals();

        if (graphBuilder == null)
            return;

        var activated = graphBuilder.dag.GetActivatedNodes();

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
        Debug.Log($"[Effect 감지] 타입:{effect.upgradeType}, ID:{effect.targetID}, 값:{effect.value}");

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
                Debug.Log("CarrierMoveSpeed 케이스 진입");

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

        minerMineSpeedById.Clear();
        carrierUnlockById.Clear();
        carrierMoveSpeedById.Clear();
        carrierMaxCarryById.Clear();
        miningAreaMaxStorageById.Clear();
        miningAreaRespawnReduceById.Clear();
        processingMaxCapacityById.Clear();
        processorProcessTimeReduceById.Clear();
    }

    private void DispatchAllEvents()
    {
        OnPlayerMoveSpeedChanged?.Invoke(totalPlayerMoveSpeed);
        OnPlayerMineSpeedChanged?.Invoke(totalPlayerMineSpeed);
        OnPlayerMaxCarryChanged?.Invoke(totalPlayerMaxCarry);
        OnPlayerMineAmountChanged?.Invoke(totalPlayerMineAmount);
        OnPlayerSellPriceChanged?.Invoke(totalPlayerSellPrice);

        OnMinerUnlockChanged?.Invoke(totalMinerUnlockCount);

        foreach (var pair in minerMineSpeedById)
            OnMinerMineSpeedChanged?.Invoke(pair.Key, pair.Value);

        foreach (var pair in carrierUnlockById.ToList())
            OnCarrierUnlockChanged?.Invoke(pair.Key, pair.Value);

        foreach (var pair in carrierMoveSpeedById.ToList())
        {
            Debug.Log($"Dispatch CarrierMoveSpeed → {pair.Key} / {pair.Value}");
            OnCarrierMoveSpeedChanged?.Invoke(pair.Key, pair.Value);
        }

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
    }
}
