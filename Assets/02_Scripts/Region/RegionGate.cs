using System.Collections;
using UnityEngine;

public class RegionGate : MonoBehaviour
{
    [Header("Gate Objects")]
    [SerializeField] private GameObject[] gateObjects; // 벽
    //[SerializeField] private GameObject lockIcon; //자물쇠 아이콘

    [Header("Animation (Optional)")]
    [SerializeField] private bool useAnimation = false;
    [SerializeField] private float openDuration = 1f;

    private bool isOpen = false;

    public void SetGateState(bool open)
    {
        if (isOpen = open) return;

        isOpen = open;

        if(useAnimation && open)
        {
            StartCoroutine(OpenGateAnimation());
        }
        else
        {
            SetGateObjectsActive(!open);
        }

        //if(lockIcon != null)
        //{
        //    lockIcon.SetActive(!open);
        //}

        Debug.Log($"[RegionGate] Gate {(open ? "opende" : "closed")}");
    }
    private void SetGateObjectsActive(bool active)
    {
        foreach(var obj in gateObjects)
        {
            if(obj != null)
            {
                obj.SetActive(active);
            }
        }
    }
    private IEnumerator OpenGateAnimation()
    {
        // 간단한 아래로 내려가는 애니메이션
        float elapsed = 0f;
        Vector3[] initialPositions = new Vector3[gateObjects.Length];

        for (int i = 0; i < gateObjects.Length; i++)
        {
            if (gateObjects[i] != null)
            {
                initialPositions[i] = gateObjects[i].transform.position;
            }
        }

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / openDuration;

            for (int i = 0; i < gateObjects.Length; i++)
            {
                Vector3 targetPos = initialPositions[i] + Vector3.down * 5f;
                gateObjects[i].transform.position = Vector3.Lerp(initialPositions[i], targetPos, progress);
            }

            yield return null;
        }

        // 애니메이션 끝나면 비활성화 
        SetGateObjectsActive(false);
    }
}