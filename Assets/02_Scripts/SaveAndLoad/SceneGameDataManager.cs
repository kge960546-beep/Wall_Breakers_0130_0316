using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGameDataManager : MonoBehaviour
{
    public static SceneGameDataManager instance;

    [Header("저장할 데이터들")]
    public int currentGold;
    public int[] sectionMineralCount = new int[5];
    public int unCollectedMoney;   
    public bool[] unlockedSections;
    public int[] sectionFillAmount;    
    public int autoNPCLevel;
    public int playerPowerLevel;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    
    public void SaveGame()
    {
        GameData data = new GameData();

        data.currentGold = this.currentGold;

        data.sectionMineralCount = (int[])this.sectionMineralCount.Clone();        

        data.unCollectedMoney = this.unCollectedMoney;
        data.unlockedSections = (bool[])this.unlockedSections.Clone(); //배열은 복제해서 저장하는게 좋음
        data.sectionFillAmount = (int[])this.sectionFillAmount.Clone();
        data.autoNPCLevel = this.autoNPCLevel;
        data.playerPowerLevel = this.playerPowerLevel;

        SaveSystem.Save(data);
        Utils.DebugLog("저장 완료");
    }

    public void LoadGame()
    {
        GameData data = SaveSystem.Load();

        if(data != null)
        {
            this.currentGold = data.currentGold;

            this.sectionMineralCount = data.sectionMineralCount;            

            this.unCollectedMoney = data.unCollectedMoney;
            this.unlockedSections = data.unlockedSections;
            this.sectionFillAmount = data.sectionFillAmount;
            this.autoNPCLevel = data.autoNPCLevel;
            this.playerPowerLevel = data.playerPowerLevel;

            Utils.DebugLog("게임 불러오기 성공");
        }
    }
}
