using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessResource : MonoBehaviour
{
    [SerializeField] List<MineralSO> processableMinerals = new List<MineralSO>(); //가공 가능한 자원 리스트

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
            StartProcessing(gameObject);
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
    IEnumerator SuccessProcessed(GameObject rawMaterial, MineralSO data)
    {
        isProcessing = true;

        stockingTable.Remove(rawMaterial.transform);
        yield return new WaitForSeconds(delay);
        Destroy(rawMaterial);

        ResourcesManager.instance.ChangeAmount(data, -1);
        Debug.Log($" 가공 시작! 원재료: {data.mineralName} 보유량: {ResourcesManager.instance.GetCurrentAmount(data.Id)}");

        if (data.processedResult != null && data.processedResult.muneralPrefab != null)
        {
            //TODO: 풀링으로 변경 예정
            GameObject processedItem = Instantiate(data.processedResult.muneralPrefab, processingPoint.position, processingPoint.rotation);

            processingTable.Add(processedItem.transform);

            processedItem.transform.SetParent(processingPoint, true);

            if(data.processedResult != null)
            {
                ResourcesManager.instance.ChangeAmount(data.processedResult, 1);
                Debug.Log($"가공완료! 가공자원: {data.processedResult.mineralName} 보유량: {ResourcesManager.instance.GetCurrentAmount(data.processedResult.Id)}");
            }
            else
            {
                if (data.processedResult == null)
                    Debug.LogError($"{data.mineralName}의 Processed Result가 SO에 등록되지 않았습니다!");
                else if (data.processedResult.muneralPrefab == null)
                    Debug.LogError($"{data.processedResult.mineralName} SO에 프리팹이 연결되지 않았습니다!");
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
        if (other.CompareTag("Player"))
        {
            StackBackPack backPack = other.GetComponent<StackBackPack>();
            if (backPack == null)
            {
                return;
            }

            if (processingTable.Count > 0 && !backPack.IsFullBackPack())
            {
                GameObject item = GiveProcessedItem();
                if (item != null)
                {
                    backPack.AddResources(item);
                }
            }

            GameObject topPlayerItem = backPack.PeekResource();

            if (topPlayerItem == null)
            {
                return;
            }

            bool canProc = CanProcess(topPlayerItem);

            if (topPlayerItem != null && canProc)
            {

                GameObject playerItem = backPack.MinusResource();

                if (playerItem != null)
                {
                    AddStock(playerItem);
                }
            }
        }
    }
}
