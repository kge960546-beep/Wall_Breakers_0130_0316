using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    [Header("현재 가지고있는 골드, 생성된 골드 오브젝트")]
    public int currentGold;
    public int unCollectedGold;

    [Header("각 섹션별 채굴되는 자원 갯수, 섹션별 가공품 갯수")]
    public int[] sectionMineralCount = new int[5]; // 0이상 5미만
    public int[] sectionProcessMineralCount = new int[5];

    [Header("판매후 스폰된 재화 오브젝트 갯수")]
    public int unCollectedMoney;

    [Header("섹션해금유무, 섹션해금에 들어간 자원수량")]
    public List<int> unlockedRegionList;
    public bool[] unlockedSections = new bool[5];
    public int[] sectionFillAmount = new int[100];

    [Header("업그래이드")]
    public List<string> unlockedUpgradeNodeIds;


    [Header("업적 저장 리스트")]
    public List<string> achievementProgess = new List<string>();

    [Header("가이드 퀘스트")]
    public int guideCurrentIndex;
    public List<int> guideSaveData;
}
