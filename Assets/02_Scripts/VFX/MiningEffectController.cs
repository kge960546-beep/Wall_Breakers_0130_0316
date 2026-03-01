using UnityEngine;

public class MiningEffectController : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject destoryEffectPrefab;

    private void Awake()
    {
        if(PoolManager.instance != null)
        {
            if (hitEffectPrefab != null)
                PoolManager.instance.CreatePool(hitEffectPrefab, 10);

            if (destoryEffectPrefab != null)
                PoolManager.instance.CreatePool(destoryEffectPrefab, 5);
        }
    }

    /// <summary>
    /// Ã¤±¼ Å¸°Ý ÀÌÆåÆ®
    /// </summary>
    public void PlayHit(Vector3 pos)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (hitEffectPrefab == null)
            Debug.LogError("Hit Effect Prefab is not assigned");

        if(PoolManager.instance == null)
        {
            Debug.LogError("PoolManager instance not found");
            return;
        }

        PoolManager.instance.Get(
            hitEffectPrefab,
            pos,
            Quaternion.identity
            );
    }

    /// <summary>
    /// ±¤¹° ÆÄ±« ÀÌÆåÆ®
    /// </summary>
    public void PlayDestroy(Vector3 pos)
    {
        if(!gameObject.activeInHierarchy)
            return;

        if(destoryEffectPrefab == null)
        {
            Debug.LogWarning("Destroy Effect Prefab is not assigned");
            return;
        }

        if(PoolManager.instance == null)
        {
            Debug.LogError("PoolManager instance not found");
            return;
        }

        PoolManager.instance.Get(
            destoryEffectPrefab,
            pos,
            Quaternion.identity
            );
    }
}
