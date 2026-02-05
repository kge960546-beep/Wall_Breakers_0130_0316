using UnityEngine;

public class SellZone : MonoBehaviour
{
    [SerializeField] private SellManager sellManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sellManager.StartSelling();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sellManager.StopSelling();
        }
    }
}