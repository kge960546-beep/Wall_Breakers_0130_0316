using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [SerializeField] private string charaterName;
    [SerializeField] private int level;
    [SerializeField] private long credit;
    [SerializeField] private long iron;         //TODO: 임의로 지정한 광물 이름 추후 수정 바람
    [SerializeField] private long bronze;       //TODO: 임의로 지정한 광물 이름 추후 수정 바람
    [SerializeField] private long gold;         //TODO: 임의로 지정한 광물 이름 추후 수정 바람

    //플레이어 위치
    //[SerializeField] private Vector3 lastPosition;

    public SaveData(string t_charaterName, int t_level, long t_credit, long t_iron, long t_bronze, long t_gold)
    {
        charaterName = t_charaterName;
        level = t_level;
        credit = t_credit;
        iron = t_iron;
        bronze = t_bronze;
        gold = t_gold;

        //TODO: 장비 관련 추가 해야함

        //종료 시점 벡터 저장
        //lastPosition = new Vector3(t_lastPosition.x, t_lastPosition.y, t_lastPosition.z);
    }
}
