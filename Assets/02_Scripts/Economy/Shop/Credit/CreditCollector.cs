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
            Utils.DebugLogError("[CreditCollector] CreditSpawner is NOT assigned!");
        }

        if (collectUI == null)
        {
            Utils.DebugLogError("[CreditCollector] CreditCollectUI is NOT assigned!");
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
            Utils.DebugLog("[CreditCollector] CreditService successfully initialized");
        }
        else
        {
            Invoke(nameof(InitializeCreditService), 0.1f);
        }
    }

    public void StartCollecting()
    {
        Utils.DebugLog($"[CreditCollector] StartCollecting called. Current state: {isCollecting}");

        if (isCollecting)
        {
            Utils.DebugLogWarning("[CreditCollector] Already collecting!");
            return;
        }


        if (creditSpawner == null || collectUI == null)
        {
            Utils.DebugLogError("[CreditCollector] Cannot start - missing references!");
            return;
        }

        isCollecting = true;
        collectUI.Show();
        collectCoroutine = StartCoroutine(CollectCycle());

        Utils.DebugLog("[CreditCollector] ✓ Collection STARTED");
    }

    public void StopCollecting()
    {
        Utils.DebugLog($"[CreditCollector] StopCollecting called. Current state: {isCollecting}");

        if (!isCollecting)
        {
            Utils.DebugLogWarning("[CreditCollector] Not collecting!");
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

        Utils.DebugLog("[CreditCollector] ✓ Collection STOPPED");
    }
    private IEnumerator CollectCycle()
    {
        while (isCollecting)
        {
            List<CreditObject> credits = creditSpawner.SpawnedCredits;

            if (credits.Count == 0)
            {
                collectUI?.UpdateDisplay(0, 0);
                yield return wait;
                break;
            }

            CreditObject creditToCollect = credits[0];

            if (creditToCollect != null && !creditToCollect.IsCollected)
            {
                int amount = creditToCollect.CreditAmount;

                collectUI?.UpdateDisplay(amount, credits.Count);

                creditToCollect.Collect();

                if (creditService != null)
                {
                    creditService.AddCredit(amount);
                }

                creditSpawner.RemoveCredit(creditToCollect);

                Utils.DebugLog($"Collected {amount} credits. Remaing: {credits.Count - 1}");
            }

            SFXManager.instance.PlayOnSFX("Blop Sound", transform.position);            

            //다음 수집까지 대기
            yield return new WaitForSeconds(collectInterval);
        }

        if (SceneGameDataManager.instance != null)
        {
            SceneGameDataManager.instance.SaveGame();
            Utils.DebugLog("골드획득 다하고 저장완료");
        }
    }

    // 강제 정지 (디버깅/안전장치)
    private void OnDisable()
    {
        if (isCollecting)
        {
            Utils.DebugLog("[CreditCollector] Component disabled, forcing stop");
            StopCollecting();
        }
    }
}