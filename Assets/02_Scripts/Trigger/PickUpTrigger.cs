using UnityEngine;

// 창고(ResourceTable)에서 플레이어 백팩으로 옮기는 트리거
public class PickUpTrigger : MonoBehaviour
{
    private ResourceTable resourceTable;

    private void Awake()
    {
        resourceTable = GetComponent<ResourceTable>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerFSM fsm = other.GetComponent<PlayerFSM>();
        StackBackPack backPack = other.GetComponent<StackBackPack>();

        if (fsm == null || backPack == null || resourceTable == null)
            return;

        // 백팩이 가득 차면 픽업 불가
        if (backPack.IsFullBackPack())
            return;

        // 창고에 아이템이 없으면 픽업 불가
        GameObject item = resourceTable.GiveItem();
        if (item == null)
            return;

        // 상태 전환은 FSM에 요청만
        if (fsm.CurrentStateType == PlayerStateType.Free)
            fsm.EnterPickingUp(PickupType.Ore);

        // 실제 적재
        backPack.AddResources(item);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerFSM fsm = other.GetComponent<PlayerFSM>();
        if (fsm != null && fsm.CurrentStateType == PlayerStateType.PickingUp)
        {
            fsm.ExitPickingUp();
        }
    }
}
