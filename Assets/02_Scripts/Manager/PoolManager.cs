using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    //Ç®¸µÀ» À§ÇÑ µñ¼Å³Ê¸®
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

        if(!poolDictionary.ContainsKey(prefab))
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

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.transform.SetParent(null);
        obj.SetActive(true);

        return obj;
    }

    public void ReturnIt(GameObject poolPrefab, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(poolPrefab))
        {
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        poolDictionary[poolPrefab].Enqueue(obj);
    }
}
