using UnityEngine;

public class UpgradeGraphBuilder : MonoBehaviour
{
    public UpgradeDAGManager<UpgradeDataSO> dag =
        new UpgradeDAGManager<UpgradeDataSO>();

    [Header("Roots")]
    public UpgradeDataSO section1Root;
    public UpgradeDataSO section2Root;

    // Player Direct
    public UpgradeDataSO playerMoveSpeed;
    public UpgradeDataSO playerMineSpeed1;
    public UpgradeDataSO playerMineSpeed2;
    public UpgradeDataSO playerMineSpeed3;
    public UpgradeDataSO playerMaxCarry1;
    public UpgradeDataSO playerMaxCarry2;
    public UpgradeDataSO playerMaxCarry3;
    public UpgradeDataSO playerMineAmount1;
    public UpgradeDataSO playerMineAmount2;
    public UpgradeDataSO playerMineAmount3;
    public UpgradeDataSO playerSellPrice;

    // Miner
    public UpgradeDataSO miner1Unlock;
    public UpgradeDataSO miner1MineSpeed;
    public UpgradeDataSO miner2Unlock;
    public UpgradeDataSO miner2MineSpeed;

    // Carrier A
    public UpgradeDataSO carrierAUnlock;
    public UpgradeDataSO carrierAMoveSpeed;
    public UpgradeDataSO carrierAMaxCarry;

    // Carrier B
    public UpgradeDataSO carrierBUnlock;
    public UpgradeDataSO carrierBMoveSpeed;
    public UpgradeDataSO carrierBMaxCarry;

    // Mining Area
    public UpgradeDataSO miningAreaMaxStorage;
    public UpgradeDataSO miningAreaRespawnReduce;

    // Processor
    public UpgradeDataSO processorMaxStorage;
    public UpgradeDataSO processorProcessTimeReduce;

    // Final
    public UpgradeDataSO section1FinalAuto;

    private void Awake()
    {
        BuildGraph();
    }

    private void BuildGraph()
    {
        var s1 = Create(section1Root);
        var s2 = Create(section2Root);

        // =========================
        // PLAYER DIRECT (수정됨)
        // =========================

        var move = Create(playerMoveSpeed);
        s1.AddChild(move);

        // 1단계는 이동속도가 부모
        var ms1 = Create(playerMineSpeed1);
        var mc1 = Create(playerMaxCarry1);
        var ma1 = Create(playerMineAmount1);

        move.AddChild(ms1);
        move.AddChild(mc1);
        move.AddChild(ma1);

        // 2단계
        var ms2 = Create(playerMineSpeed2);
        var mc2 = Create(playerMaxCarry2);
        var ma2 = Create(playerMineAmount2);

        ms1.AddChild(ms2);
        mc1.AddChild(mc2);
        ma1.AddChild(ma2);

        // 3단계
        var ms3 = Create(playerMineSpeed3);
        var mc3 = Create(playerMaxCarry3);
        var ma3 = Create(playerMineAmount3);

        ms2.AddChild(ms3);
        mc2.AddChild(mc3);
        ma2.AddChild(ma3);

        // 판매 가격 (3개 모두 부모)
        var sell = Create(playerSellPrice);
        ms3.AddChild(sell);
        mc3.AddChild(sell);
        ma3.AddChild(sell);

        // =========================
        // MINER
        // =========================

        var m1u = Create(miner1Unlock);
        var m1s = Create(miner1MineSpeed);
        var m2u = Create(miner2Unlock);
        var m2s = Create(miner2MineSpeed);

        s1.AddChild(m1u);
        m1u.AddChild(m1s);
        m1s.AddChild(m2u);
        m2u.AddChild(m2s);

        // =========================
        // CARRIER A (부모: miner1Unlock)
        // =========================

        var aU = Create(carrierAUnlock);
        var aM = Create(carrierAMoveSpeed);
        var aC = Create(carrierAMaxCarry);

        m1u.AddChild(aU);
        aU.AddChild(aM);
        aM.AddChild(aC);

        // =========================
        // CARRIER B (부모: miner1Unlock)
        // =========================

        var bU = Create(carrierBUnlock);
        var bM = Create(carrierBMoveSpeed);
        var bC = Create(carrierBMaxCarry);

        m1u.AddChild(bU);
        bU.AddChild(bM);
        bM.AddChild(bC);

        // =========================
        // 채굴구역 저장공간 증가 (부모 2개)
        // miner2MineSpeed + carrierAMaxCarry
        // =========================

        var areaStorage = Create(miningAreaMaxStorage);
        var areaRespawn = Create(miningAreaRespawnReduce);

        m2s.AddChild(areaStorage);
        aC.AddChild(areaStorage);

        areaStorage.AddChild(areaRespawn);

        // =========================
        // 가공기계 최대저장공간 증가 (부모 2개)
        // carrierAMaxCarry + carrierBMaxCarry
        // =========================

        var pStorage = Create(processorMaxStorage);
        var pTime = Create(processorProcessTimeReduce);

        aC.AddChild(pStorage);
        bC.AddChild(pStorage);

        pStorage.AddChild(pTime);

        // =========================
        // FINAL AUTO (부모 2개)
        // =========================

        var finalAuto = Create(section1FinalAuto);

        areaRespawn.AddChild(finalAuto);
        pTime.AddChild(finalAuto);

        // =========================
        // SECTION2
        // =========================

        sell.AddChild(s2);
        finalAuto.AddChild(s2);

        Debug.Log("Total Nodes Count : " + dag.Nodes.Count);
    }

    private UpgradeGraphNode<UpgradeDataSO> Create(UpgradeDataSO so)
    {
        return dag.CreateNode(so.upgradeID, so);
    }
}
