using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 자동 운반인 가방
/// - ID 기반으로 UpgradeEffectManager 이벤트 구독
/// - CarrierMaxCarry 효과를 직접 반영
/// </summary>
public class AutoBackPack : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] private string targetID;   // CarrierController와 동일하게 입력

    [Header("가방 설정")]
    [SerializeField] private List<Transform> resources = new List<Transform>();
    [SerializeField] private Transform backPackPos;
    [SerializeField] private float itemHeight = 0.3f;

    [Header("현재 최대 수용량 (실시간 반영)")]
    [SerializeField] private int maxCapacity = 5;   // ← 인스펙터에 보이는 값

    private int baseCapacity;
    private int bonusCapacity;

    public bool IsFullBackPack() => resources.Count >= maxCapacity;
    public bool IsEmptyBackPack() => resources.Count == 0;

    private void Awake()
    {
        baseCapacity = maxCapacity;   // 처음 값 저장
    }

    private void OnEnable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnCarrierMaxCarryChanged += HandleMaxCarry;
        }
    }

    private void OnDisable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnCarrierMaxCarryChanged -= HandleMaxCarry;
        }
    }

    private void HandleMaxCarry(string id, int bonus)
    {
        if (id != targetID) return;

        bonusCapacity = bonus;
        maxCapacity = baseCapacity + bonusCapacity;
    }

    private void Update()
    {
        if (resources.Count == 0) return;

        for (int i = 0; i < resources.Count; i++)
        {
            Transform currentResource = resources[i];

            Vector3 targetPos = backPackPos.position + Vector3.up * itemHeight * i;
            Quaternion targetRot = backPackPos.rotation;

            currentResource.position = Vector3.Lerp(currentResource.position, targetPos, Time.deltaTime * 10f);
            currentResource.rotation = Quaternion.Lerp(currentResource.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

    public void AddResource(GameObject item)
    {
        resources.Add(item.transform);
        item.transform.SetParent(backPackPos);

        if (item.TryGetComponent<Collider>(out var col))
            col.enabled = false;

        SFXManager.instance.PlayOnSFX("Blop Sound", transform.position);

        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public GameObject RemoveResource()
    {
        if (IsEmptyBackPack()) return null;

        int lastIndex = resources.Count - 1;
        GameObject item = resources[lastIndex].gameObject;

        resources.RemoveAt(lastIndex);
        item.transform.SetParent(null);

        return item;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Resources") && !IsFullBackPack())
        {
            var table = other.GetComponent<ResourceTable>();

            if (table != null)
            {
                GameObject item = table.GiveItem();

                if (item != null)
                    AddResource(item);
            }
        }
    }
}
