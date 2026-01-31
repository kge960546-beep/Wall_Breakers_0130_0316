using System.Collections.Generic;
using UnityEngine;

public class StackBackPack : MonoBehaviour
{
    [SerializeField] private List<Transform> acquiredResources = new List<Transform>(); //가방에 담길 리스트
    [SerializeField] GameObject player;   //플레이어
    [SerializeField] Transform backPackPos; //가방 위치
    public float itemHeight = 0.3f; //아이템 높이 간격

    [SerializeField] private int maxCapacity = 10; //최대 수용량

    private void Start()
    {
        if (backPackPos == null)
        {
            backPackPos = GameObject.Find("BackPackPos").transform;
        }
    }
    private void Update()
    {
        if (acquiredResources.Count == 0) return;

        //가방 시작 위치
        if (acquiredResources.Count > 0)
        {
            acquiredResources[0].position = Vector3.Lerp(acquiredResources[0].position, backPackPos.position, Time.deltaTime * 10f);
            acquiredResources[0].rotation = Quaternion.Lerp(acquiredResources[0].rotation, backPackPos.rotation, Time.deltaTime * 10f);
        }

        for (int i = 1; i < acquiredResources.Count; i++)
        {
            Transform currentAcquired = acquiredResources[i];
            Transform previousAcquired = acquiredResources[i - 1];

            Vector3 targetPos = previousAcquired.position + Vector3.up * itemHeight;

            currentAcquired.position = Vector3.Lerp(currentAcquired.position, targetPos, Time.deltaTime * 10f);
            currentAcquired.rotation = Quaternion.Lerp(currentAcquired.rotation, backPackPos.rotation, Time.deltaTime * 10f);
        }
    }
    public void AddResources(GameObject resources)
    {
        if (resources == null) return;

        acquiredResources.Add(resources.transform);

        resources.transform.SetParent(backPackPos, true);

        Collider col = resources.GetComponent<Collider>();
        if (col) col.enabled = false;

        Destroy(resources.GetComponent<Rigidbody>());
    }

    //TODO: 자원 획득 트리거를 테이블에 있는 자원을 트리거하는방식으로 Stay변경 예정 
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Resources")) //TODO: 자원에서 창고로 쓰이는 테이블 Tag, Layer 로 체인지
        {
            if (acquiredResources.Count >= maxCapacity) return; //가방이 지정한 갯수만큼 꽉 찼으면 리턴

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
}
