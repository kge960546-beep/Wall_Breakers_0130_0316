using UnityEngine;

public class CreditZone : MonoBehaviour
{
    [SerializeField] private CreditCollector creditCollector;

    private bool playerInZone = false;

    private void Start()
    {
        ValidateSetup();
    }

    private void ValidateSetup()
    {
        if (creditCollector == null)
        {
            Utils.DebugLogError("[CreditZone] CreditCollector is Not assigned!");
        }
        else
        {
            Utils.DebugLog("[CreditZone] CreditCollector is properly assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Utils.DebugLog($"[CreditZone] OntriggerEnter detected: {other.gameObject.name}, Tag{other.tag}");

        if (other.CompareTag("Player"))
        {
            if (playerInZone)
            {
                Utils.DebugLogWarning("[CreditZone] Player already in Credit Zone!");
                return;
            }

            playerInZone = true;
            Utils.DebugLog("[CreditZone] Player Entered Credit Zone");

            if (creditCollector != null)
            {
                creditCollector.StartCollecting();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Utils.DebugLog($"[CreditZone] OnTriggerExit detected: {other.gameObject.name}, Tag: {other.tag}");

        if (other.CompareTag("Player"))
        {
            if (!playerInZone)
            {
                Utils.DebugLogWarning("[CreditZone] Player was not in zone!");
                return;
            }

            playerInZone = false;
            Utils.DebugLog("[CreditZone] Player exited Credit Zone!");

            if (creditCollector != null)
            {
                creditCollector.StopCollecting();
            }
        }
    }

    /// <summary>
    /// 강제로 상태 초기화 (디버깅 용)
    /// </summary>
    private void OnDisable()
    {
        if (playerInZone && creditCollector != null)
        {
            Utils.DebugLog("[CreditZone] Zone disableed, sropping collection");
            creditCollector.StopCollecting();

            playerInZone = false;
        }
    }
}
