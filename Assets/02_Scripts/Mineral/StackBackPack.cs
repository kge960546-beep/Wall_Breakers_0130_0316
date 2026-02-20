using System.Collections.Generic;
using UnityEngine;

public class StackBackPack : MonoBehaviour
{
    private List<Transform> acquiredResources = new List<Transform>(); //가방에 담길 리스트
    [SerializeField] GameObject player;   //플레이어
    [SerializeField] Transform backPackPos; //가방 위치
    public float itemHeight = 0.3f; //아이템 높이 간격

    [SerializeField] private int maxCapacity = 10; //최대 수용량
    private int baseCapacity;                      //기본 수용량
    private int bonusCapacity;                     //강화 수용량

    public bool IsFullBackPack() => acquiredResources.Count >= maxCapacity;

    [SerializeField] private PlayerFullUI playerFullUI;

    public static StackBackPack Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        baseCapacity = maxCapacity; // 기본값 저장
    }

    private void OnDisable()
    {
        if (UpgradeEffectManager.Instance != null)
            UpgradeEffectManager.Instance.OnPlayerMaxCarryChanged -= HandleMaxCarryChanged;
    }

    private void HandleMaxCarryChanged(int totalBonus)
    {
        bonusCapacity = totalBonus;
        maxCapacity = baseCapacity + bonusCapacity;

        if (acquiredResources.Count < maxCapacity)
        {
            playerFullUI.Hide();
        }
    }

    private void Start()
    {
        if (backPackPos == null)
        {
            backPackPos = GameObject.Find("BackPackPos").transform;
        }

        if (UpgradeEffectManager.Instance != null)
            UpgradeEffectManager.Instance.OnPlayerMaxCarryChanged += HandleMaxCarryChanged;
    }

    private void Update()
    {
        if (acquiredResources.Count == 0) return;

        if (acquiredResources.Count > 0)
        {
            Quaternion targetRot = backPackPos.rotation;
            MineralItem mineralItem = acquiredResources[0].GetComponent<MineralItem>();

            if (mineralItem != null && mineralItem.mineralData != null)
            {
                targetRot *= Quaternion.Euler(mineralItem.mineralData.backPackRotationOffset);
            }

            acquiredResources[0].position = Vector3.Lerp(acquiredResources[0].position, backPackPos.position, Time.deltaTime * 10f);
            acquiredResources[0].rotation = Quaternion.Lerp(acquiredResources[0].rotation, targetRot, Time.deltaTime * 10f);
        }

        for (int i = 1; i < acquiredResources.Count; i++)
        {
            Transform currentAcquired = acquiredResources[i];
            Transform previousAcquired = acquiredResources[i - 1];

            Vector3 targetPos = previousAcquired.position + Vector3.up * itemHeight;

            Quaternion targetRot = backPackPos.rotation;
            MineralItem mineralItem = currentAcquired.GetComponent<MineralItem>();

            if (mineralItem != null && mineralItem.mineralData != null)
            {
                targetRot *= Quaternion.Euler(mineralItem.mineralData.backPackRotationOffset);
            }

            currentAcquired.position = Vector3.Lerp(currentAcquired.position, targetPos, Time.deltaTime * 10f);
            currentAcquired.rotation = Quaternion.Lerp(currentAcquired.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

    public void AddResources(GameObject resourcesObj)
    {
        if (resourcesObj == null) return;

        MineralItem mineralItem = resourcesObj.GetComponent<MineralItem>();

        if (mineralItem == null || mineralItem.mineralData == null)
        {
            Debug.LogWarning("없습니다 MineralItem 또는 mineralData.");
            Destroy(resourcesObj);
            return;
        }

        if (mineralItem != null && mineralItem.mineralData != null)
        {
            PlayerInventory.Instance.AddItem(mineralItem.mineralData, 1);
        }

        acquiredResources.Add(resourcesObj.transform);
        resourcesObj.transform.SetParent(backPackPos, true);

        Collider col = resourcesObj.GetComponent<Collider>();
        if (col) col.enabled = false;

        Rigidbody rb = resourcesObj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (IsFullBackPack())
        {
            playerFullUI.Show();
        }
    }

    public GameObject MinusResource()
    {
        if (acquiredResources.Count == 0) return null;

        int lastIndex = acquiredResources.Count - 1;
        GameObject itemObj = acquiredResources[lastIndex].gameObject;

        MineralItem mineralItem = itemObj.GetComponent<MineralItem>();
        if (mineralItem != null && mineralItem.mineralData != null)
        {
            PlayerInventory.Instance.RemoveItem(mineralItem.mineralData, 1);
        }

        acquiredResources.RemoveAt(lastIndex);
        itemObj.transform.SetParent(null);

        if (!IsFullBackPack())
        {
            playerFullUI.Hide();
        }

        return itemObj;
    }

    public GameObject PeekResource()
    {
        if (acquiredResources.Count == 0) return null;

        int lastIndex = acquiredResources.Count - 1;
        return acquiredResources[lastIndex].gameObject;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Resources"))
        {
            if (acquiredResources.Count >= maxCapacity) return;

            ResourceTable table = other.GetComponent<ResourceTable>();
            if (table != null)
            {
                GameObject item = table.GiveItem();

                if (item != null)
                {
                    AddResources(item);
                }
            }
        }
    }

    public void AddCapacity(int amount)
    {
        maxCapacity += amount;

        if (maxCapacity < 0)
            maxCapacity = 0;

        if (acquiredResources.Count < maxCapacity)
        {
            playerFullUI.Hide();
        }
    }
}
