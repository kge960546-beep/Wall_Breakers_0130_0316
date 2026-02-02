using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mineralAmounts = new int[mineralsDataList.Count];
    }

    [SerializeField] List<MineralSO> mineralsDataList = new List<MineralSO>(); //자원과 수량을 담는 리스트
    private int[] mineralAmounts; //각 자원의 수량을 담는 배열

    private event Action<MineralSO, int> onMineralChanged;

    //구독
    public void SubscribeonMineralChanged(Action<MineralSO, int> action)
    {
        onMineralChanged += action;
    }

    //구독 해제
    public void UnsubscribeonMineralChanged(Action<MineralSO, int> action)
    {
        onMineralChanged -= action;
    }

    //자원 수량 변경
    public void ChangeAmount(MineralSO data, int amount)
    {
        if (data == null) return;

        int index = data.Id;

        if (index < 0 || index >= mineralAmounts.Length)
        {
            return;
        }

        mineralAmounts[index] += amount;

        onMineralChanged?.Invoke(data, mineralAmounts[index]);

#if UNITY_EDITOR
        Debug.Log($"자원 {data.mineralName} 수량이 {mineralAmounts[index]}로 변경되었습니다.");
#endif

        //TODO: UI 업데이트
    }

    //현재 자원이 몇개 있는지 반환
    public int GetCurrentAmount(int id)
    {
        if (id < 0 || id >= mineralAmounts.Length) return 0;
        return mineralAmounts[id];
    }
}
