using UnityEngine;

public class UpgradeSyncScript : MonoBehaviour
{

    void Start()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            var data = SceneGameDataManager.instance;

            //플레이어
            UpgradeEffectManager.Instance.OnPlayerMaxCarryChanged += (value) =>
            {
                data.playerMaxCarry = (int)value; data.SaveGame();
            };
            UpgradeEffectManager.Instance.OnPlayerMineAmountChanged += (sectionIndex, value) =>
            {
                data.playerMineAmount = (float)value; data.SaveGame();
            };
            UpgradeEffectManager.Instance.OnPlayerMineSpeedChanged += (sectionIndex, value) =>
            {
                data.playerMineSpeed = (float)value; data.SaveGame();
            };
            UpgradeEffectManager.Instance.OnPlayerMoveSpeedChanged += (value) =>
            {
                data.playerMoveSpeed = (float)value; data.SaveGame();
            };
            UpgradeEffectManager.Instance.OnPlayerRawSellPriceChanged += (value) =>
            {
                data.playerRawSellPrice = (float)value; data.SaveGame();
            };

            //가공자원
            UpgradeEffectManager.Instance.OnProcessedSellPriceChanged += (value) =>
            {
                data.processedSellPrice = (float)value; data.SaveGame();
            };
            UpgradeEffectManager.Instance.OnProcessingMaxCapacityChanged += (id, value) =>
            {
                data.processingMaxCapacity = ((int)value); data.SaveGame();
            };
            UpgradeEffectManager.Instance.OnProcessorProcessTimeChanged += (id, value) =>
            {
                data.processorProcessTime = value; data.SaveGame();
            };

            //운반NPC
            UpgradeEffectManager.Instance.OnCarrierMoveSpeedChanged += (id, value) =>
            {
                data.carrierMoveSpeed = (float)value; data.SaveGame();
            };
            UpgradeEffectManager.Instance.OnCarrierMaxCarryChanged += (id, value) =>
            {
                data.carrierMaxCarry = (int)value; data.SaveGame();
            };
            UpgradeEffectManager.Instance.OnCarrierUnlockChanged += (id, value) =>
            {
                data.carrierUnlock = (bool)value; data.SaveGame();
            };

            //채굴NPC
            UpgradeEffectManager.Instance.OnMinerMineSpeedChanged += (id, value) =>
            {
                data.minerMineSpeed = (float)value; data.SaveGame();
            };
            UpgradeEffectManager.Instance.OnMinerMineAmountChanged += (id, value) =>
            {
                data.minerMineAmount = (int)value; data.SaveGame();
            };
            UpgradeEffectManager.Instance.OnMinerUnlockChanged += (value) =>
            {
                data.minerUnlock = (int)value; data.SaveGame();
            };

            //광산
            UpgradeEffectManager.Instance.OnMiningAreaMaxStorageChanged += (id, value) =>
            {
                data.miningAreaMaxStorage = (int)value; data.SaveGame();
            };
            UpgradeEffectManager.Instance.OnMiningAreaRespawnTimeChanged += (id, value) =>
            {
                data.miningAreaRespawnTime = (float)value; data.SaveGame();
            };


        }
    }

}
