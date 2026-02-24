[System.Serializable]
public class SectionUpgradeSet
{
    public UpgradeDataSO sectionRoot;

    // Player
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
    public UpgradeDataSO sectionFinal;
}