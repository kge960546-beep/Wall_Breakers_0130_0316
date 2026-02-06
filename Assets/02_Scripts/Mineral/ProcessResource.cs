using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessResource : MonoBehaviour
{
    [SerializeField] List<ItemDataSO> processableMinerals = new List<ItemDataSO>(); //가공 가능한 자원 리스트

    [SerializeField] List<Transform> stockingTable = new List<Transform>();     //창고 테이블
    [SerializeField] List<Transform> processingTable = new List<Transform>();   //가공 테이블

    [SerializeField] Transform stockingPoint;   //창고 위치
    [SerializeField] Transform processingPoint; //가공 위치

    [SerializeField] float itemHeight = 0.3f;   //아이템 높이 간격
    [SerializeField] float delay = 1.0f;        //가공 딜레이

    bool isProcessing = false;  //가공 중인지 여부

    private void Update()
    {
        StackPosition(stockingTable, stockingPoint);
        StackPosition(processingTable, processingPoint);

        if (!isProcessing && stockingTable.Count > 0)
        {
            GameObject gameObject = stockingTable[0].gameObject;

            var mineralItem = gameObject.GetComponent<MineralItem>();
            if (stockingTable.Count >= mineralItem.mineralData.inputAmountPerProcess)
            {
                StartProcessing(gameObject);
            }
        }
    }

    //쌓이는 아이템 공용함수
    public void StackPosition(List<Transform> list, Transform basePos)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Vector3 targetPos = basePos.position + Vector3.up * itemHeight * i;
            list[i].position = Vector3.Lerp(list[i].position, targetPos, Time.deltaTime * 10f);
            list[i].rotation = Quaternion.Lerp(list[i].rotation, basePos.rotation, Time.deltaTime * 10f);
        }
    }

    public bool CanProcess(GameObject itemObj)
    {
        if (itemObj == null) return false;

        MineralItem mineralItem = itemObj.GetComponent<MineralItem>();

        if (mineralItem == null || mineralItem.mineralData == null) return false;

        return processableMinerals.Contains(mineralItem.mineralData);
    }

    //플레이어한테서 자원 받기
    public void AddStock(GameObject rawMaterial)
    {
        stockingTable.Add(rawMaterial.transform);
        rawMaterial.transform.SetParent(stockingPoint, true);

        if (rawMaterial.TryGetComponent<Collider>(out var col)) col.enabled = false;
        if (rawMaterial.TryGetComponent<Rigidbody>(out var rb)) Destroy(rb);
    }

    //가공시작
    public void StartProcessing(GameObject rawMaterial)
    {
        if (isProcessing) return;

        MineralItem item = rawMaterial.GetComponent<MineralItem>();
        if (item != null && item.mineralData.processedResult != null)
        {
            StartCoroutine(SuccessProcessed(rawMaterial, item.mineralData));
        }

    }
    //생성 로직을 타이밍 설정을 위한 코루틴
    IEnumerator SuccessProcessed(GameObject rawMaterial, ItemDataSO data)
    {
        isProcessing = true;

        int resourceQuantity = data.inputAmountPerProcess;

        if (resourceQuantity <= 0) resourceQuantity = 1;

        List<GameObject> destroyResources = new List<GameObject>();
        for (int i = 0; i < resourceQuantity; i++)
        {
            if (stockingTable.Count > 0)
            {
                GameObject obj = stockingTable[0].gameObject;
                stockingTable.RemoveAt(0);
                destroyResources.Add(obj);
            }
        }

        //stockingTable.Remove(rawMaterial.transform);
        yield return new WaitForSeconds(delay);
        foreach (var obj in destroyResources)
        {
            Destroy(obj);
        }
        //Destroy(rawMaterial);

        ResourcesManager.instance.ChangeAmount(data, -destroyResources.Count);
        Debug.Log($" 가공 시작! 원재료: {data.itemName} 보유량: {ResourcesManager.instance.GetCurrentAmount(data.Id)}");

        if (data.processedResult != null && data.processedResult.muneralPrefab != null)
        {
            //TODO: 풀링으로 변경 예정
            GameObject processedItem = PoolManager.instance.Get(data.processedResult.muneralPrefab, processingPoint.position, Quaternion.identity);
            processedItem.transform.SetParent(processingPoint);

            processingTable.Add(processedItem.transform);

            processedItem.transform.SetParent(processingPoint, true);

            if (data.processedResult != null)
            {
                ResourcesManager.instance.ChangeAmount(data.processedResult, 1);
                Debug.Log($"가공완료! 가공자원: {data.processedResult.itemName} 보유량: {ResourcesManager.instance.GetCurrentAmount(data.processedResult.Id)}");
            }
            else
            {
                if (data.processedResult == null)
                    Debug.LogError($"{data.itemName}의 Processed Result가 SO에 등록되지 않았습니다!");
                else if (data.processedResult.muneralPrefab == null)
                    Debug.LogError($"{data.processedResult.itemName} SO에 프리팹이 연결되지 않았습니다!");
            }
        }

        isProcessing = false;
    }

    public GameObject GiveProcessedItem()
    {
        if (processingTable.Count == 0) return null;

        int lastIndex = processingTable.Count - 1;
        Transform itemTr = processingTable[lastIndex];
        processingTable.RemoveAt(lastIndex);
        itemTr.SetParent(null);

        return itemTr.gameObject;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerFSM fsm = other.GetComponent<PlayerFSM>();
        StackBackPack backPack = other.GetComponent<StackBackPack>();

        if (fsm == null || backPack == null)
            return;

        // ===== 1 드롭 우선 =====
        if (TryDrop(backPack))
        {
            // 실제 드롭이 발생했을 때 상태 전환 요청
            fsm.EnterDropping(DropType.Process);
            return;
        }

        // ===== 2 픽업 =====
        if (TryPickUp(backPack))
        {
            // 실제 픽업이 발생했을 때 상태 전환 요청
            fsm.EnterPickingUp(PickupType.ProcessedItem);
            return;
        }
    }

    // =========================
    // 드롭 처리
    // =========================
    bool TryDrop(StackBackPack backPack)
    {
        GameObject topItem = backPack.PeekResource();
        if (!CanProcess(topItem))
            return false;

        GameObject item = backPack.MinusResource();
        if (item == null)
            return false;

        AddStock(item);
        return true;
    }

    // =========================
    // 픽업 처리
    // =========================
    bool TryPickUp(StackBackPack backPack)
    {
        if (processingTable.Count == 0)
            return false;

        if (backPack.IsFullBackPack())
            return false;

        GameObject item = GiveProcessedItem();
        if (item == null)
            return false;

        backPack.AddResources(item);
        return true;
    }
}
