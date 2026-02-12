using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditCollector : MonoBehaviour
{
    [Header("Refernecs")]
    [SerializeField] private CreditSpawner creditSpawner;
    [SerializeField] private CreditCollectUI collectUI;

    [Header("Collection Settings")]
    [SerializeField] private float collectInterval = 0.2f; // 수집간격

    private bool isCollecting = false;
    private Coroutine collectCoroutine;
    private CreditService creditService;

    [SerializeField] WaitForSeconds wait = new WaitForSeconds(0.5f);

    private void Start()
    {
        ValidateSetup();
        InitializeCreditService();
    }

    private void ValidateSetup()
    {
        if (creditSpawner == null)
        {
            Debug.LogError("[CreditCollector] CreditSpawner is NOT assigned!");
        }

        if (collectUI == null)
        {
            Debug.LogError("[CreditCollector] CreditCollectUI is NOT assigned!");
        }
    }

    private void InitializeCreditService()
    {
        if (GameManager.Instance == null)
        {
            Invoke(nameof(InitializeCreditService), 0.1f);
            return;
        }

        creditService = GameManager.Instance.GetService<CreditService>();

        if (creditService != null)
        {
            Debug.Log("[CreditCollector] CreditService successfully initialized");
        }
        else
        {
            Invoke(nameof(InitializeCreditService), 0.1f);
        }
    }

    public void StartCollecting()
    {
        Debug.Log($"[CreditCollector] StartCollecting called. Current state: {isCollecting}");

        if (isCollecting)
        {
            Debug.LogWarning("[CreditCollector] Already collecting!");
            return;
        }

        if (creditSpawner == null || collectUI == null)
        {
            Debug.LogError("[CreditCollector] Cannot start - missing references!");
            return;
        }

        isCollecting = true;
        collectUI.Show();
        collectCoroutine = StartCoroutine(CollectCycle());

        Debug.Log("[CreditCollector] ✓ Collection STARTED");
    }

    public void StopCollecting()
    {
        Debug.Log($"[CreditCollector] StopCollecting called. Current state: {isCollecting}");

        if (!isCollecting)
        {
            Debug.LogWarning("[CreditCollector] Not collecting!");
            return;
        }

        isCollecting = false;

        if (collectCoroutine != null)
        {
            StopCoroutine(collectCoroutine);
            collectCoroutine = null;
        }

        if (collectUI != null)
        {
            collectUI.Hide();
        }

        Debug.Log("[CreditCollector] ✓ Collection STOPPED");
    }
    private IEnumerator CollectCycle()
    {
        while(isCollecting)
        {
            List<CreditObject> credits = creditSpawner.SpawnedCredits;

            if(credits.Count == 0)
            {
                collectUI?.UpdateDisplay(0, 0);
                yield return wait;
                continue;
            }

            CreditObject creditToCollect = credits[0];

            if (creditToCollect != null && !creditToCollect.IsCollected)
            {
                int amount = creditToCollect.CreditAmount;

                collectUI?.UpdateDisplay(amount, credits.Count);

                creditToCollect.Collect();

                if(creditService != null)
                {
                    creditService.AddCredit(amount);
                }

                creditSpawner.RemoveCredit(creditToCollect);

                Debug.Log($"Collected {amount} credits. Remaing: {credits.Count - 1}");
            }

            //다음 수집까지 대기
            yield return new WaitForSeconds(collectInterval);
        }
    }

    // 강제 정지 (디버깅/안전장치)
    private void OnDisable()
    {
        if (isCollecting)
        {
            Debug.Log("[CreditCollector] Component disabled, forcing stop");
            StopCollecting();
        }
    }
}