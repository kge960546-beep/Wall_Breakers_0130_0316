using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTable : MonoBehaviour
{
     List<Transform> resourceInTable = new List<Transform>();
    [SerializeField] Transform tablePos; //테이블 위치
    [SerializeField] float itemHeight = 0.3f; //아이템 높이 간격

    [Header("저장할 데이터 설정")]
    [SerializeField] int sectionIndex;
    [SerializeField] GameObject mineralPrefab;

    [Header("최대 저장 용량")]
    [SerializeField] private int maxCapacity = 20;

    [Header("Upgrade ID")]
    [SerializeField] private string targetID;

    private int baseCapacity;
    private int bonusCapacity;

    public int SectionIndex => sectionIndex;
    public int CurrentCount => resourceInTable.Count;

    public bool IsFull => resourceInTable.Count >= maxCapacity;

    private void Awake()
    {
        baseCapacity = maxCapacity;
    }

    private void OnEnable()
    {
        Debug.Log($"[OnEnable] {targetID}");
    }
    private IEnumerator Start()
    {
        yield return null; // 한 프레임 대기

        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnMiningAreaMaxStorageChanged += HandleMaxStorageChanged;
            UpgradeEffectManager.Instance.RecalculateAllEffects();

            Debug.Log($"[구독 완료] {targetID}");
        }
        else
        {
            Debug.LogError($"[구독 실패] EffectManager 없음 - {targetID}");
        }
    }

    private void OnDisable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnMiningAreaMaxStorageChanged -= HandleMaxStorageChanged;
        }
    }

    void Update()
    {
        for (int i = resourceInTable.Count - 1; i >= 0; i--)
        {
            if (resourceInTable[i] == null)
                resourceInTable.RemoveAt(i);
        }

        if (resourceInTable.Count == 0) return;

        //테이블 시작 위치
        if (resourceInTable.Count > 0)
        {
            resourceInTable[0].position = Vector3.Lerp(resourceInTable[0].position, tablePos.position, Time.deltaTime * 10f);
            resourceInTable[0].rotation = Quaternion.Lerp(resourceInTable[0].rotation, tablePos.rotation, Time.deltaTime * 10f);
        }

        //테이블에 자원 쌓기
        for (int i = 1; i < resourceInTable.Count; i++)
        {
            Transform currentAcquired = resourceInTable[i];
            Transform previousAcquired = resourceInTable[i - 1];

            Vector3 targetPos = previousAcquired.position + Vector3.up * itemHeight;

            currentAcquired.position = Vector3.Lerp(currentAcquired.position, targetPos, Time.deltaTime * 10f);
            currentAcquired.rotation = Quaternion.Lerp(currentAcquired.rotation, tablePos.rotation, Time.deltaTime * 10f);
        }
    }

    //플레이어한테서(채굴하는 곳) 자원 받기
    public void AddResources(GameObject resources)
    {
        if (resources == null) return;

        if (resourceInTable.Count >= maxCapacity)
        {
            PoolManager.instance.ReturnIt(mineralPrefab, resources);
            Debug.Log("채굴구역 저장공간 가득 참 → 추가 채굴 반영 안됨");
            return;
        }

        resourceInTable.Add(resources.transform);
        resources.transform.SetParent(tablePos, true);

        if (resources.TryGetComponent<Collider>(out var col))
            col.enabled = false;

        if (resources.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    //플레이어에게 자원 주기
    public GameObject GiveItem()
    {
        if (resourceInTable.Count == 0) return null;

        int lastIndex = resourceInTable.Count - 1;
        Transform itemTr = resourceInTable[lastIndex];

        resourceInTable.RemoveAt(lastIndex);

        if (itemTr == null) return null;

        return itemTr.gameObject;
    }

    public void RebuildStack(int amount)
    {
        Debug.Log($"[Table] {gameObject.name} 복구 시작. 목표 개수: {amount}");

        if (mineralPrefab == null)
        {
            Debug.LogError($"{gameObject.name}의 mineralPrefab이 비어있습니다!");
            return;
        }

        while (resourceInTable.Count > 0)
        {
            GameObject obj = GiveItem();
            PoolManager.instance.ReturnIt(mineralPrefab, obj);
        }

        for(int i = 0; i < amount; i++)
        {
            GameObject item = PoolManager.instance.Get(mineralPrefab, tablePos.position, tablePos.rotation);

            if (item == null)
            {
                Debug.LogError("PoolManager에서 아이템을 가져오지 못했습니다.");
                continue;
            }

            AddResources(item);
        }
    }
    private void HandleMaxStorageChanged(string id, int bonus)
    {
        Debug.Log($"[이벤트 수신 시도] 나는 {targetID}, 받은ID:{id}");

        if (id != targetID) return;

        bonusCapacity = bonus;
        maxCapacity = baseCapacity + bonusCapacity;

        Debug.Log($"[MiningArea:{targetID}] 최대 저장공간 → {maxCapacity}");
    }
}
