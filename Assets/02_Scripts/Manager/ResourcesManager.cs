using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    [Header("자원 프리팹")]
    [SerializeField] GameObject earthPrefab;
    [SerializeField] GameObject rockPrefab;
    [SerializeField] GameObject copperPrefab;
    [SerializeField] GameObject ironPrefab;
    [SerializeField] GameObject goldPrefab;
    [SerializeField] GameObject moneyPrefab;
    [SerializeField] GameObject soundPrefab;

    private void Start()
    {
        PoolManager.instance.CreatePool(earthPrefab, 100);
        PoolManager.instance.CreatePool(rockPrefab, 100);
        PoolManager.instance.CreatePool(copperPrefab, 100);
        PoolManager.instance.CreatePool(ironPrefab, 100);
        PoolManager.instance.CreatePool(goldPrefab, 100);
        PoolManager.instance.CreatePool(moneyPrefab, 100);
        PoolManager.instance.CreatePool(soundPrefab, 100);

        Utils.DebugLog("풀링 완료");
    }
}
