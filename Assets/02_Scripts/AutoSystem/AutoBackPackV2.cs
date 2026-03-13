using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 가공품 전용 자동 운반 가방 (판매 전용)
/// 기존 AutoBackPack과 완전 분리
/// </summary>
public class AutoBackPackV2 : MonoBehaviour
{
    [Header("가방 설정")]
    [SerializeField] private List<Transform> resources = new List<Transform>();
    [SerializeField] private Transform backPackPos;
    [SerializeField] private float itemHeight = 0.3f;

    [Header("최대 수용량")]
    [SerializeField] private int maxCapacity = 5;

    [Header("Upgrade ID")]
    [SerializeField] private string targetID;   // CarrierB와 동일하게
    public string TargetID => targetID;

    private int baseCapacity;
    private int bonusCapacity;

    public bool IsFullBackPack() => resources.Count >= maxCapacity;
    public bool IsEmptyBackPack() => resources.Count == 0;

    private void Awake()
    {
        baseCapacity = maxCapacity;   // 기본값 저장
    }

    private void OnEnable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnCarrierMaxCarryChanged += HandleMaxCarryChanged;

            // 현재 활성화된 노드 반영
            UpgradeEffectManager.Instance.RecalculateAllEffects();
        }
    }

    private void OnDisable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnCarrierMaxCarryChanged -= HandleMaxCarryChanged;
        }
    }
    private void Update()
    {
        if (resources.Count == 0) return;

        for (int i = 0; i < resources.Count; i++)
        {
            Transform current = resources[i];

            Vector3 targetPos = backPackPos.position + Vector3.up * itemHeight * i;
            Quaternion targetRot = backPackPos.rotation;

            MineralItem mineralItem = current.GetComponent<MineralItem>();
            if(mineralItem != null && mineralItem.mineralData != null)
            {
                targetRot *= Quaternion.Euler(mineralItem.mineralData.backPackRotationOffset);
            }

            current.position = Vector3.Lerp(current.position, targetPos, Time.deltaTime * 10f);
            current.rotation = Quaternion.Lerp(current.rotation, targetRot, Time.deltaTime * 10f);
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

    private void HandleMaxCarryChanged(string id, int bonus)
    {
        if (id != targetID) return;

        bonusCapacity = bonus;

        maxCapacity = baseCapacity + bonusCapacity;

        Utils.DebugLog($"[AutoBackPackV2:{targetID}] 최대 적재량 적용 → {maxCapacity}");
    }
}