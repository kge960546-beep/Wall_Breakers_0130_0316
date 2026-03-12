using System.Collections.Generic;
using UnityEngine;

public class CreditSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject creditPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float stackHeight = 0.2f; // 크레딧이 쌓이는 높이
    [SerializeField] private int maxStackPerColumn = 10; // 한 기둥당 최대 개수
    [SerializeField] private float columRadius = 0.5f; // 원형 배치

    private List<CreditObject> spawnedCredits = new();
    private int currentStackIndex = 0;

    public List<CreditObject> SpawnedCredits => spawnedCredits;

    private void Start()
    {
        // 초기 설정 확인
        ValidateSetup();
    }
    private void ValidateSetup()
    {
        if (creditPrefab == null)
        {
            Debug.LogError("[CreditSpawner] CreditPrefab is Not assigned in Inspector!");
        }
        else
        {
            Debug.Log($"[CreditSpawner] CreditPrefab assigned: {creditPrefab.name}");

            CreditObject creditComponent = creditPrefab.GetComponent<CreditObject>();
            if (creditComponent == null)
            {
                Debug.LogError("[CreditSpawner] CreditPrefab doesn't have CrditObject component");
            }
            else
            {
                Debug.Log("[CreditSpawner] CreditObject component found on prefab");
            }
        }

        if (spawnPoint == null)
        {
            Debug.LogError("[CreditSpawner] SpawnPoint is Not assigned in Inspector!");
        }
        else
        {
            Debug.Log($"[CreditSpawner] SpawnPoint assigned at position: {spawnPoint.position}");
        }
    }
    public void SpawnCredit(int amount)
    {
        Debug.Log($"[CreditSpawner] SpawnCredit called with amount: {amount}");

        if (creditPrefab == null)
        {
            Debug.LogError("[CreditSpawner] Cannot spawn - CreditPrefab is null!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("[CreditSpawner] Cannot spawn - SpawnPoint is null!");
            return;
        }

        // 위치 계산
        Vector3 spawnPosition = CalculateSpawnPosition();
        Debug.Log($"[CreditSpawner] Calculated spawn position: {spawnPosition}");

        // 크레딧 오브젝트 생성
        GameObject creditObj = Instantiate(creditPrefab, spawnPosition, Quaternion.identity, transform);
        Debug.Log($"[CreditSpawner] GameObject instantiated: {creditObj.name}");

        CreditObject credit = creditObj.GetComponent<CreditObject>();

        if (credit != null)
        {
            credit.Initialize(amount);
            spawnedCredits.Add(credit);
            currentStackIndex++;

            Debug.Log($"[CreditSpawner] ✓ Credit spawned successfully! Amount: {amount}, Position: {spawnPosition}, Total spawned: {spawnedCredits.Count}");
        }
        else
        {
            Debug.LogError("[CreditSpawner] CreditObject component not found on instantiated prefab!");
            Destroy(creditObj);
        }
    }
    private Vector3 CalculateSpawnPosition()
    {
        int columnIndex = currentStackIndex / maxStackPerColumn;
        int heightIndex = currentStackIndex % maxStackPerColumn;

        float angle = columnIndex * 45f * Mathf.Deg2Rad; //
        float x = Mathf.Cos(angle) * columRadius;
        float z = Mathf.Sin(angle) * columRadius;
        float y = heightIndex * stackHeight;

        return spawnPoint.position + new Vector3(x, y, z);
    }

    public void RemoveCredit(CreditObject credit)
    {
        if (spawnedCredits.Contains(credit))
        {
            spawnedCredits.Remove(credit);
            Destroy(credit.gameObject);
        }
    }
    public void ClearAllCredits()
    {
        foreach (var credit in spawnedCredits)
        {
            if (credit != null)
                Destroy(credit.gameObject);
        }
        spawnedCredits.Clear();
        currentStackIndex = 0;
    }
}
