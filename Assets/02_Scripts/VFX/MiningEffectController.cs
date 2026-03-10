using UnityEngine;

public class MiningEffectController : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject destoryEffectPrefab;

    private static float globalVfxTimer;
    private const float globalCooldown = 0.1f;

    private void Awake()
    {
        if(PoolManager.instance != null)
        {
            if (hitEffectPrefab != null)
                PoolManager.instance.CreatePool(hitEffectPrefab, 3);

            if (destoryEffectPrefab != null)
                PoolManager.instance.CreatePool(destoryEffectPrefab, 2);
        }
    }

    /// <summary>
    /// Ã¤±¼ Å¸°Ý ÀÌÆåÆ®
    /// </summary>
    public void PlayHit(Vector3 pos)
    {
        if (Time.time < globalVfxTimer)
            return;

        globalVfxTimer = Time.time + globalCooldown;

        if (!gameObject.activeInHierarchy)
            return;

        if (hitEffectPrefab == null)
            return;

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
