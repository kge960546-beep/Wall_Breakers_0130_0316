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

    private void Start()
    {
        Debug.Log($"[RegionGate] Start - Gate objects count: {gateObjects?.Length ?? 0}");
        ValidateSetup();
    }
    private void ValidateSetup()
    {
        if (gateObjects == null || gateObjects.Length == 0)
        {
            Debug.LogWarning("[RegionGate] No Gate objects assigned!");
        }
        else
        {
            Debug.Log($"[RegionGate] Gate objects assigned:");
            for (int i = 0; i < gateObjects.Length; i++)
            {
                if (gateObjects[i] != null)
                {
                    Debug.Log($" [{i}] {gateObjects[i].name} - Active: {gateObjects[i].activeSelf}");
                }
                else
                {
                    Debug.LogWarning($"  [{i}] NULL gate object!");
                }
            }
        }
    }
    public void SetGateState(bool open)
    {
        Debug.Log($"[RegionGate] SetGateState called: {open} (current state: {isOpen})");

        if (isOpen == open)
        {
            Debug.Log("[RegionGate] Gate already in requested state, ignoring");
            return;
        }

        isOpen = open;

        if (useAnimation && open)
        {
            Debug.Log("[RegionGate] Opening gate with animation");
            StartCoroutine(OpenGateAnimation());
        }
        else
        {
            Debug.Log($"[RegionGate] Setting gate objects active: {!open}");
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
        Debug.Log($"[RegionGate] SetGateObjectsActive: {active}");

        if (gateObjects == null)
        {
            Debug.LogError("[RegionGate] gateObjects array is NULL!");
            return;
        }

        for (int i = 0; i < gateObjects.Length; i++)
        {
            if (gateObjects[i] != null)
            {
                gateObjects[i].SetActive(active);
                Debug.Log($"[RegionGate] Gate object [{i}] {gateObjects[i].name} set to: {active}");
            }
            else
            {
                Debug.LogWarning($"[RegionGate] Gate object at index {i} is NULL!");
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

    #region DebugTool
    // 디버그용 - Inspector에서 테스트 가능
    [ContextMenu("Test Open Gate")]
    public void TestOpenGate()
    {
        Debug.Log("[RegionGate] Test: Opening gate");
        SetGateState(true);
    }

    [ContextMenu("Test Close Gate")]
    public void TestCloseGate()
    {
        Debug.Log("[RegionGate] Test: Closing gate");
        SetGateState(false);
    }
    #endregion
}