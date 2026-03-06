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

    private void Start()
    {
        PoolManager.instance.CreatePool(earthPrefab, 9000);
        PoolManager.instance.CreatePool(rockPrefab, 9000);
        PoolManager.instance.CreatePool(copperPrefab, 9000);
        PoolManager.instance.CreatePool(ironPrefab, 9000);
        PoolManager.instance.CreatePool(goldPrefab, 9000);
        PoolManager.instance.CreatePool(moneyPrefab, 9000);

        Utils.DebugLog("풀링 완료");
    }
}
