using UnityEngine;

/// <summary>
/// 드롭 트리거
/// 역할:
/// - 플레이어가 드롭 가능한 영역에 들어왔는지 감지
/// - FSM에 "드롭 상태 진입/종료" 요청만 수행
///
/// 주의:
/// - 실제 드롭(아이템 내려놓기) 로직은 여기서 처리하지 않는다
/// - 실제 로직은 ProcessResource / 판매대 / 기타 시스템이 담당한다
/// </summary>
public class DropTrigger : MonoBehaviour
{
    [Header("드롭 타입")]
    [SerializeField] private DropType dropType;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerFSM fsm = other.GetComponent<PlayerFSM>();
        if (fsm == null)
            return;

        // Free 상태일 때만 드롭 상태 진입 요청
        if (fsm.CurrentStateType == PlayerStateType.Free)
        {
            fsm.EnterDropping(dropType);
        }

        // 실제 드롭 처리:
        // - ProcessResource / 판매 시스템 / 기타 시스템의
        //   OnTriggerStay에서 자동으로 처리됨
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerFSM fsm = other.GetComponent<PlayerFSM>();
        if (fsm == null)
            return;

        // 드롭 상태였다면 Free 상태로 복귀
        if (fsm.CurrentStateType == PlayerStateType.Dropping)
        {
            fsm.ExitDropping();
        }
    }
}
