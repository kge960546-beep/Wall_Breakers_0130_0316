// 아무 작업도 안 하는 기본 상태
using UnityEngine;

public class FreeState : IPlayerState
{
    public PlayerStateType StateType => PlayerStateType.Free;

    private PlayerFSM fsm;

    public FreeState(PlayerFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        Debug.Log("FreeState 시작");
        // 대기 상태 진입
        // 여기에 대기 동작 뭐 Idle 애니메이션 호출을 넣는다던가 할듯
    }

    public void Update()
    {
        // 트리거 진입 감지 후 상태 전환은 외부에서 호출
        // 아마 비어있게 될 가능성 큼
    }

    public void Exit()
    {
        Debug.Log("FreeState 종료");
        // Idle 애니메이션 종료
        // 대기 상태 종료
    }
}
