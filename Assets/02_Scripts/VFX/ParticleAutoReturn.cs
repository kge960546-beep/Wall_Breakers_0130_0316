using System.Collections;
using UnityEngine;

public class ParticleAutoReturn : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }
    private void OnEnable()
    {
        if (ps != null)
            StartCoroutine(ReturnRoution());
    }

    private IEnumerator ReturnRoution()
    {
        yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constant);

        PoolManager.instance.ReturnIt(prefab, gameObject);
    }
}
