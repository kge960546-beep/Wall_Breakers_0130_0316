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
        PoolManager.instance.CreatePool(earthPrefab, 30);
        PoolManager.instance.CreatePool(rockPrefab, 30);
        PoolManager.instance.CreatePool(copperPrefab, 30);
        PoolManager.instance.CreatePool(ironPrefab, 30);
        PoolManager.instance.CreatePool(goldPrefab, 30);
        PoolManager.instance.CreatePool(moneyPrefab, 30);

        Debug.Log("풀링 완료");
    }
}
