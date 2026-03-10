using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessResource : MonoBehaviour
{
    [SerializeField] List<ItemDataSO> processableMinerals = new List<ItemDataSO>(); //가공 가능한 자원 리스트

    private List<Transform> stockingTable = new();
    private List<Transform> processingTable = new();

    private MineralItem firstMineralItemStock;
    [SerializeField] Transform stockingPoint;   //창고 위치
    [SerializeField] Transform processingPoint; //가공 위치

    [SerializeField] float itemHeight = 0.3f;   //아이템 높이 간격

    [SerializeField] float delay = 1.0f;        //가공 딜레이

    [Header("가공 시간 설정")]
    [SerializeField] private float processTime = 1.0f;

    private float baseProcessTime;
    private float bonusProcessTimeReduction;

    bool isProcessing = false;  //가공 중인지 여부

    public int sectionIndex; //섹션별 인덱스

    [Header("Lever")]
    [SerializeField] Transform leverHandle;   // 레버 손잡이
    [SerializeField] float leverUpY = 0.33f;  // 기본 위치
    [SerializeField] float leverDownY = -0.33f; // 작동 위치
    [SerializeField] float leverMoveSpeed = 2f; // 이동 속도

    [Header("가공 결과 최대 저장량")]
    [SerializeField] private int maxProcessedCapacity = 5;

    private int baseProcessedCapacity;
    private int bonusProcessedCapacity;

    [SerializeField] private string processingAreaID;

    [Header("UI Reference")]
    [SerializeField] private ProcessorUI processorUI;

    private void Awake()
    {
        baseProcessedCapacity = maxProcessedCapacity;
        baseProcessTime = processTime;
    }

    private IEnumerator Start()
    {
        yield return null; // 한 프레임 대기

        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnProcessingMaxCapacityChanged += HandleProcessingCapacityChanged;
            UpgradeEffectManager.Instance.OnProcessorProcessTimeChanged += HandleProcessTimeChanged;

            // 구독 후 즉시 현재 상태 반영
            UpgradeEffectManager.Instance.RecalculateAllEffects();
        }
    }

    private void OnDisable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnProcessingMaxCapacityChanged -= HandleProcessingCapacityChanged;
            UpgradeEffectManager.Instance.OnProcessorProcessTimeChanged -= HandleProcessTimeChanged;
        }
    }


    private void Update()
    {
        StackPosition(stockingTable, stockingPoint);
        StackPosition(processingTable, processingPoint);

        if (!isProcessing && stockingTable.Count > 0 && !IsProcessedFull())
        {
            GameObject gameObject = stockingTable[0].gameObject;

            //캐싱처리
            if (firstMineralItemStock == null)
            {
                firstMineralItemStock = gameObject.GetComponent<MineralItem>();
            }            

            if(firstMineralItemStock != null)
            {
                if (stockingTable.Count >= firstMineralItemStock.mineralData.inputAmountPerProcess)
                {
                    StartProcessing(gameObject);

                    firstMineralItemStock = null;
                }
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

    public void RebuildProcessedStack(int amount)
    {
        foreach(var item in processingTable)
        {
            if (item != null) PoolManager.instance.ReturnIt(processableMinerals[0].processedResult.mineralPrefab, item.gameObject);
        }
        processingTable.Clear();

        for(int i = 0; i < amount; i++)
        {
            GameObject prefab = processableMinerals[0].processedResult.mineralPrefab;
            GameObject processedItem = PoolManager.instance.Get(prefab, processingPoint.position, Quaternion.identity);

            processingTable.Add(processedItem.transform);
            processedItem.transform.SetParent(processingPoint, true);
        }
    }

    //플레이어한테서 자원 받기
    public void AddStock(GameObject rawMaterial)
    {
        stockingTable.Add(rawMaterial.transform);
        rawMaterial.transform.SetParent(stockingPoint, true);

        SFXManager.instance.PlayOnSFX("19987__acclivity__fingerplop1", stockingPoint.position);

        if (rawMaterial.TryGetComponent<Collider>(out var col)) col.enabled = false;
        if (rawMaterial.TryGetComponent<Rigidbody>(out var rb)) Destroy(rb);
    }

    //가공시작
    public void StartProcessing(GameObject rawMaterial)
    {
        if (isProcessing) return;

        MineralItem item = rawMaterial.GetComponent<MineralItem>();
        if (item != null && item.mineralData != null && item.mineralData.processedResult != null)
        {
            isProcessing = true;
            StartCoroutine(SuccessProcessed(rawMaterial, item.mineralData));
        }
    }

    //생성 로직을 타이밍 설정을 위한 코루틴
    IEnumerator SuccessProcessed(GameObject rawMaterial, ItemDataSO data)
    {
        // 1. 즉시 중복 실행 방지 잠금
        isProcessing = true;

        // 2. 한 프레임 대기 (동기화 안전성)
        yield return null;

        // --- 수량 계산 로직 추가 ---
        int inputPerProcess = data.inputAmountPerProcess > 0 ? data.inputAmountPerProcess : 1;
        int currentStock = stockingTable.Count; // 현재 쌓여있는 원재료 총합

        // 현재 재료로 가능한 총 가공 횟수 (정수 나눗셈: 20 / 3 = 6)
        int possibleBatchCount = currentStock / inputPerProcess;

        // UI에 표시할 값들
        int totalInputVisual = possibleBatchCount * inputPerProcess; // 소모될 총 재료 (예: 18)
        int totalOutputVisual = possibleBatchCount; // 생성될 총 결과물 (예: 6)
                                                    // -------------------------

        // 3. UI 초기화 및 표시 (계산된 수량 전달)
        if (processorUI != null)
        {
            if (data != null && data.processedResult != null)
            {
                processorUI.SetupUI(data, data.processedResult, totalInputVisual, totalOutputVisual);
            }
        }

        // 4. 가공 시간 확정
        float finalProcessTime = Mathf.Max(0.1f, baseProcessTime - bonusProcessTimeReduction);

        SFXManager.instance.PlayOnSFX("232869__lagezon__cardboard_factory_machine-004", transform.position, finalProcessTime);

        // 5. 원재료 소모 (실제 가공 1회분인 resourceQuantity만큼만 소모)
        int resourceQuantity = inputPerProcess;

        if (SceneGameDataManager.instance != null)
            SceneGameDataManager.instance.sectionMineralCount[sectionIndex] -= resourceQuantity;

        for (int i = 0; i < resourceQuantity; i++)
        {
            if (stockingTable.Count > 0)
            {
                GameObject obj = stockingTable[0].gameObject;
                stockingTable.RemoveAt(0);
                if (PoolManager.instance != null)
                    PoolManager.instance.ReturnIt(data.mineralPrefab, obj);
            }
        }

        // 6. 가공 진행 루프
        float elapsed = 0f;
        while (elapsed < finalProcessTime)
        {
            elapsed += Time.deltaTime;
            float normalized = elapsed / finalProcessTime;

            if (processorUI != null)
                processorUI.UpdateProgress(normalized, finalProcessTime - elapsed);

            if (leverHandle != null)
            {
                float leverY = (normalized < 0.5f)
                    ? Mathf.Lerp(leverUpY, leverDownY, normalized * 2f)
                    : Mathf.Lerp(leverDownY, leverUpY, (normalized - 0.5f) * 2f);
                leverHandle.localPosition = new Vector3(leverHandle.localPosition.x, leverY, leverHandle.localPosition.z);
            }

            yield return null;
        }

        // 7. 가공 결과물 생성 (1회분 생성)
        if (data.processedResult != null && data.processedResult.mineralPrefab != null)
        {
            GameObject processedItem = PoolManager.instance.Get(
                data.processedResult.mineralPrefab,
                processingPoint.position,
                Quaternion.identity
            );

            GuideManager.Instance?.AddProgress(GuideActionType.CraftBrick);

            SFXManager.instance.PlayOnSFX("149270__organicmanpl__ding-1", processingPoint.position);

            if (SceneGameDataManager.instance != null)
                SceneGameDataManager.instance.sectionProcessMineralCount[sectionIndex] += 1;

            processingTable.Add(processedItem.transform);
            processedItem.transform.SetParent(processingPoint, true);
        }

        // 8. 종료 및 UI 닫기
        if (processorUI != null) processorUI.CloseUI();

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
        if (!other.CompareTag("Player") && !other.CompareTag("AutoCarrierWorker"))
            return;

        // ===============================
        //  AutoBackPackV2 (가공품 전용 운반원)
        // ===============================
        var autoBackPackV2 = other.GetComponent<AutoBackPackV2>();
        if (autoBackPackV2 != null)
        {
            // 가공품 픽업
            if (!autoBackPackV2.IsFullBackPack() && processingTable.Count > 0)
            {
                GameObject processed = GiveProcessedItem();
                if (processed != null)
                    autoBackPackV2.AddResource(processed);
            }

            return; // V2는 여기서 끝
        }

        // ===============================
        //  기존 AutoBackPack (원자재 운반용)
        // ===============================
        var autoBackPack = other.GetComponent<AutoBackPack>();
        if (autoBackPack != null)
        {
            GameObject item = autoBackPack.RemoveResource();
            if (item != null)
                AddStock(item);

            return;
        }

        // ===============================
        //  Player 처리 (기존 로직 유지)
        // ===============================
        PlayerFSM fsm = other.GetComponent<PlayerFSM>();
        StackBackPack backPack = other.GetComponent<StackBackPack>();

        if (fsm == null || backPack == null)
            return;

        // 드롭 우선
        if (TryDrop(backPack))
        {
            fsm.EnterDropping(DropType.Process);
            return;
        }

        // 픽업
        if (TryPickUp(backPack))
        {
            fsm.EnterPickingUp(PickupType.ProcessedItem);
            return;
        }
    }

    // 드롭 처리    
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
    
    // 픽업 처리    
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

    // 레버 이동 코루틴
    //IEnumerator MoveLeverY(float targetY, float duration)
    //{
    //    Vector3 startPos = leverHandle.localPosition;
    //    Vector3 targetPos = new Vector3(startPos.x, targetY, startPos.z);
    //
    //    float elapsed = 0f;
    //
    //    while (elapsed < duration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = elapsed / duration;
    //        leverHandle.localPosition = Vector3.Lerp(startPos, targetPos, t);
    //        yield return null;
    //    }
    //
    //    leverHandle.localPosition = targetPos;
    //}

    private bool IsProcessedFull()
    {
        return processingTable.Count >= maxProcessedCapacity;
    }

    private void HandleProcessingCapacityChanged(string id, int bonus)
    {
        if (id != processingAreaID)
            return;

        bonusProcessedCapacity = bonus;

        maxProcessedCapacity = baseProcessedCapacity + bonusProcessedCapacity;
    }

    private void HandleProcessTimeChanged(string id, float reduction)
    {
        if (id != processingAreaID)
            return;

        bonusProcessTimeReduction = reduction;
    }

}
