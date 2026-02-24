using System.Collections.Generic;
using UnityEngine;

public class UpgradeGraphBuilder : MonoBehaviour
{
    public List<SectionUpgradeSet> sections;

    private UpgradeDAGManager<UpgradeDataSO> dag =
    new UpgradeDAGManager<UpgradeDataSO>();

    public UpgradeDAGManager<UpgradeDataSO> DAG => dag;

    private void Awake()
    {
        for (int i = 0; i < sections.Count; i++)
        {
            BuildSection(sections[i], i); ;
        }

        Debug.Log("Total Nodes Count : " + dag.Nodes.Count);
    }

    private void BuildSection(SectionUpgradeSet d, int sectionIndex)
    {
        var root = Create(d.sectionRoot, sectionIndex);

        // =========================
        // PLAYER
        // =========================

        var move = Create(d.playerMoveSpeed, sectionIndex);
        root.AddChild(move);

        var ms1 = Create(d.playerMineSpeed1, sectionIndex);
        var mc1 = Create(d.playerMaxCarry1, sectionIndex);
        var ma1 = Create(d.playerMineAmount1, sectionIndex);

        move.AddChild(ms1);
        move.AddChild(mc1);
        move.AddChild(ma1);

        var ms2 = Create(d.playerMineSpeed2, sectionIndex);
        var mc2 = Create(d.playerMaxCarry2, sectionIndex);
        var ma2 = Create(d.playerMineAmount2, sectionIndex);

        ms1.AddChild(ms2);
        mc1.AddChild(mc2);
        ma1.AddChild(ma2);

        var ms3 = Create(d.playerMineSpeed3, sectionIndex);
        var mc3 = Create(d.playerMaxCarry3, sectionIndex);
        var ma3 = Create(d.playerMineAmount3, sectionIndex);

        ms2.AddChild(ms3);
        mc2.AddChild(mc3);
        ma2.AddChild(ma3);

        var sell = Create(d.playerSellPrice, sectionIndex);
        ms3.AddChild(sell);
        mc3.AddChild(sell);
        ma3.AddChild(sell);

        // =========================
        // MINER
        // =========================

        var m1u = Create(d.miner1Unlock, sectionIndex);
        var m1s = Create(d.miner1MineSpeed, sectionIndex);
        var m2u = Create(d.miner2Unlock, sectionIndex);
        var m2s = Create(d.miner2MineSpeed, sectionIndex);

        root.AddChild(m1u);
        m1u.AddChild(m1s);
        m1s.AddChild(m2u);
        m2u.AddChild(m2s);

        // =========================
        // CARRIER A
        // =========================

        var aU = Create(d.carrierAUnlock, sectionIndex);
        var aM = Create(d.carrierAMoveSpeed, sectionIndex);
        var aC = Create(d.carrierAMaxCarry, sectionIndex);

        m1u.AddChild(aU);
        aU.AddChild(aM);
        aM.AddChild(aC);

        // =========================
        // CARRIER B
        // =========================

        var bU = Create(d.carrierBUnlock, sectionIndex);
        var bM = Create(d.carrierBMoveSpeed, sectionIndex);
        var bC = Create(d.carrierBMaxCarry, sectionIndex);

        m1u.AddChild(bU);
        bU.AddChild(bM);
        bM.AddChild(bC);

        // =========================
        // AREA
        // =========================

        var areaStorage = Create(d.miningAreaMaxStorage, sectionIndex);
        var areaRespawn = Create(d.miningAreaRespawnReduce, sectionIndex);

        m2s.AddChild(areaStorage);
        aC.AddChild(areaStorage);
        areaStorage.AddChild(areaRespawn);

        // =========================
        // PROCESSOR
        // =========================

        var pStorage = Create(d.processorMaxStorage, sectionIndex);
        var pTime = Create(d.processorProcessTimeReduce, sectionIndex);

        aC.AddChild(pStorage);
        bC.AddChild(pStorage);
        pStorage.AddChild(pTime);

        // =========================
        // FINAL
        // =========================

        var final = Create(d.sectionFinal, sectionIndex);
        areaRespawn.AddChild(final);
        pTime.AddChild(final);
    }

    private UpgradeGraphNode<UpgradeDataSO> Create(UpgradeDataSO so, int sectionIndex)
    {
        var node = dag.CreateNode(so.upgradeID, so);
        node.SectionIndex = sectionIndex;
        return node;
    }
}