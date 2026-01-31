using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PprocessResource : MonoBehaviour
{
    [SerializeField] MineralSO mineralData;

    [SerializeField] Transform spawnPoint;
    [SerializeField] float delay = 1.0f;


    public void ProcessResource()
    {
        if (mineralData.mineralType == MineralType.RawMaterial)
        {
            MineralSO processedMineral = mineralData.processedResult;
            if (processedMineral != null)
            {                
                StartCoroutine(SuccessProcessed());
            }
            else
            {
                Debug.LogWarning("No processed result defined for this mineral.");
            }
        }
        else
        {
            Debug.LogWarning("This mineral is already processed.");
        }
    }
    //생성 로직을 타이밍 설정을 위한 코루틴
    IEnumerator SuccessProcessed()
    {        

        yield return new WaitForSeconds(delay);
        //TODO: 생성 로직
    }
}
