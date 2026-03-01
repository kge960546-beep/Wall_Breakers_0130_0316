using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ToolsEquip : MonoBehaviour
{
    [SerializeField] private Transform toolsAnchor;
    [SerializeField] private GameObject pickaxePrefab;

    private GameObject currentPickaxe;
    private PickaxeTrailController trailController;
    private PlayerFSM fsm;

    private void Awake()
    {
        fsm = GetComponent<PlayerFSM>();

        if (fsm == null)
            Debug.LogError("PlayerFSM 없음");

    }
    private void OnEnable()
    {
        fsm.OnMiningStarted += EquipPickaxe;
        fsm.OnMiningEnded += UnequipPickaxe;

        Debug.Log("ToolsEquip 이벤트 등록");
    }

    private void OnDisable()
    {
        fsm.OnMiningStarted -= EquipPickaxe;
        fsm.OnMiningEnded -= UnequipPickaxe;

        Debug.Log("ToolsEquip 이벤트 해제");
    }

    public void EquipPickaxe()
    {
        if(currentPickaxe != null)
        {
            currentPickaxe.SetActive(true);
            return;
        }

        currentPickaxe = Instantiate(pickaxePrefab, toolsAnchor);

        Transform pickaxeAnchor = currentPickaxe.transform.Find("Pickaxe Anchor");

        trailController = currentPickaxe.GetComponent<PickaxeTrailController>();

        if (trailController == null)
            Debug.LogError("TrailController 없음");

        trailController.TrailOn();

        // Anchor 위치 맞추기
        Vector3 deltaPos = toolsAnchor.position - pickaxeAnchor.position;
        currentPickaxe.transform.position += deltaPos;

        // Anchor 회전 맞추기
        Quaternion deltaRot =
            toolsAnchor.rotation * Quaternion.Inverse(pickaxeAnchor.rotation);

        currentPickaxe.transform.rotation =
            deltaRot * currentPickaxe.transform.rotation;
    }
    public void UnequipPickaxe()
    {
        if (currentPickaxe == null) return;

        if(trailController != null)
        {
            trailController.TrailOff();
        }

        currentPickaxe.SetActive(false);
    }

    // trail 이벤트
    public void TrailOn()
    {
        Debug.Log("Trail On 호출됨");

        if (trailController == null)
        {
            Debug.Log("trailController NULL");
            return;
        }

        trailController.TrailOn();
    }
    public void TrailOff()
    {
        Debug.Log("Trail Off 호출됨");

        trailController.TrailOff();
    }
}
