using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAutoReturn : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private float returnTime;

    private void OnEnable()
    {
        CancelInvoke();

        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();

        float maxTime = 0f;

        foreach (var p in particles)
        {
            var main = p.main;
            float time = main.duration + main.startLifetime.constantMax;

            if (time > maxTime)
                maxTime = time;
        }

        returnTime = maxTime;

        if (prefab == null)
        {
            Debug.LogError($"Prefab missing on {name}");
            return;
        }

        Invoke(nameof(ReturnToPool), returnTime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    void ReturnToPool()
    {
        if (PoolManager.instance != null)
        {
            PoolManager.instance.ReturnIt(prefab, gameObject);
        }
    }
}
