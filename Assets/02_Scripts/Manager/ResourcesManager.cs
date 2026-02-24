using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    [Header("자원 프리팹")]
    [SerializeField] GameObject earthPrefab;
    [SerializeField] GameObject rockPrefab;
    [SerializeField] GameObject copperPrefab;
    [SerializeField] GameObject ironPrefab;
    [SerializeField] GameObject goldPrefab;
    [SerializeField] GameObject moneyPrefab;

    private void Start()
    {
        PoolManager.instance.CreatePool(earthPrefab, 90);
        PoolManager.instance.CreatePool(rockPrefab, 90);
        PoolManager.instance.CreatePool(copperPrefab, 90);
        PoolManager.instance.CreatePool(ironPrefab, 90);
        PoolManager.instance.CreatePool(goldPrefab, 90);
        PoolManager.instance.CreatePool(moneyPrefab, 90);
        
        Utils.DebugLog("풀링 완료");
    }
}
