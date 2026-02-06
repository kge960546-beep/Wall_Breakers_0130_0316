using System.Collections;
using System.Collections.Generic;
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
        if(creditCollector == null)
        {
            Debug.LogError("[CreditZone] CreditCollector is Not assigned!");
        }
        else
        {
            Debug.Log("[CreditZone] CreditCollector is properly assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[CreditZone] OntriggerEnter detected: {other.gameObject.name}, Tag{other.tag}");

        if(other.CompareTag("Player"))
        {
            if(playerInZone)
            {
                Debug.LogWarning("[CreditZone] Player already in Credit Zone!");
                return;
            }

            playerInZone = true;
            Debug.Log("[CreditZone] Player Entered Credit Zone");

            if (creditCollector != null)
            {
                creditCollector.StartCollecting();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[CreditZone] OnTriggerExit detected: {other.gameObject.name}, Tag: {other.tag}");

        if(other.CompareTag("Player"))
        {
            if(!playerInZone)
            {
                Debug.LogWarning("[CreditZone] Player was not in zone!");
                return;
            }

            playerInZone = false;
            Debug.Log("[CreditZone] Player exited Credit Zone!");

            if(creditCollector != null)
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
        if(playerInZone && creditCollector != null)
        {
            Debug.Log("[CreditZone] Zone disableed, sropping collection");
            creditCollector.StopCollecting();

            playerInZone = false;
        }
    }
}
