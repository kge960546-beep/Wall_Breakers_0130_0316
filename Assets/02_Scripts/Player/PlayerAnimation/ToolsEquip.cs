using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ToolsEquip : MonoBehaviour
{
    [SerializeField] private Transform toolsAnchor;
    [SerializeField] private GameObject pickaxePrefab;

    private GameObject currentPickaxe;
    private PlayerFSM fsm;

    private void Awake()
    {
        fsm = GetComponent<PlayerFSM>();
    }

    private void OnEnable()
    {
        fsm.OnMiningStarted += EquipPickaxe;
        fsm.OnMiningEnded += UnequipPickaxe;
    }

    private void OnDisable()
    {
        fsm.OnMiningStarted += EquipPickaxe;
        fsm.OnMiningEnded -= UnequipPickaxe;
    }

    public void EquipPickaxe()
    {
        if (currentPickaxe != null) return;

        currentPickaxe = Instantiate(pickaxePrefab, toolsAnchor);

        Transform pickaxeAnchor = currentPickaxe.transform.Find("Pickaxe Anchor");

        if(pickaxeAnchor == null)
        {
            Debug.LogError("PickaxeAnchor not found!");
            return;
        }
        // 부모 설정
        currentPickaxe.transform.SetParent(toolsAnchor);

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
        if(currentPickaxe == null) return;
        currentPickaxe.SetActive(false);
    }
}
