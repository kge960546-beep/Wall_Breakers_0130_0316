using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBackPack : MonoBehaviour
{
    [SerializeField] List<Transform> resources = new List<Transform>();
    [SerializeField] Transform backPackPos; //가방 위치
    [SerializeField] float itemHeight = 0.3f; //아이템 높이 간격
    [SerializeField] int maxCapacity = 5; //최대 수용량

    public bool IsFullBackPack() => resources.Count >= maxCapacity;
    public bool IsEmptyBackPack() => resources.Count == 0;
    private void Update()
    {
        if(resources.Count == 0) return;

        for(int i = 0; i < resources.Count; i++)
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

        if(item.TryGetComponent<Collider>(out var col)) col.enabled = false;
        if(item.TryGetComponent<Rigidbody>(out var rb))
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
        if(other.CompareTag("Resources") && !IsFullBackPack())
        {
            var table = other.GetComponent<ResourceTable>();
            if(table != null)
            {
                GameObject item = table.GiveItem();
                if(item != null) AddResource(item);
            }
        }
    }
}
