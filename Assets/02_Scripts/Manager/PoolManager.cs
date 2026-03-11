using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    //풀링을 위한 딕셔너리
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else Destroy(gameObject);
    }

    public void CreatePool(GameObject prefab, int count)
    {
        if (prefab == null) return;

        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary.Add(prefab, new Queue<GameObject>());
        }

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(this.transform);

            poolDictionary[prefab].Enqueue(obj);
        }
    }

    public GameObject Get(GameObject poolPrefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(poolPrefab))
        {
            poolDictionary.Add(poolPrefab, new Queue<GameObject>());
        }

        GameObject obj = null;

        if (poolDictionary[poolPrefab].Count > 0)
        {
            obj = poolDictionary[poolPrefab].Dequeue();
        }
        else
        {
            obj = Instantiate(poolPrefab);
        }

        PoolObject poolObj = obj.GetComponent<PoolObject>();
        if (poolObj == null)
            poolObj = obj.AddComponent<PoolObject>();

        poolObj.prefab = poolPrefab;

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.transform.SetParent(null);
        obj.SetActive(true);

        return obj;
    }

    public void ReturnIt(GameObject poolPrefab, GameObject obj)
    {
        // 자동으로 prefab 교정
        if (!poolDictionary.ContainsKey(poolPrefab))
        {
            PoolObject poolObj = obj.GetComponent<PoolObject>();

            if (poolObj != null)
                poolPrefab = poolObj.prefab;
        }

        if (!poolDictionary.ContainsKey(poolPrefab))
        {
            Debug.LogWarning("Pool key missing. Creating new pool for: " + poolPrefab.name);
            poolDictionary.Add(poolPrefab, new Queue<GameObject>());
        }

        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        poolDictionary[poolPrefab].Enqueue(obj);
    }
}
