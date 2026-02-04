using UnityEngine;

// 완전 제어 불가 상태
// 퀘스트 연출, 컷씬, 해금 연출 등 플레이어 조작을 막아야 할 때 사용
public class LockedState : IPlayerState
{
    public PlayerStateType StateType => PlayerStateType.Locked;

    private PlayerFSM fsm;

    public LockedState(PlayerFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        Debug.Log("LockedState 시작");
        // 이 State 진입 시점에서
        // - 플레이어 입력 차단
        // - 컷씬 / 연출 / UI 블로킹 시작
        // 같은 처리를 외부에서 하도록 이벤트만 호출
        fsm.RaiseLocked();
    }

    public void Update()
    {
        // 이 State의 Update 책임:
        // - 아무 것도 하지 않음
        // - 종료 조건 판단조차 FSM이 하지 않음

        // LockedState는
        // "외부 시스템이 상태 해제를 요청할 때까지"
        // 유지되는 점유 상태임

        // 예:
        // 컷씬 종료
        // 퀘스트 연출 완료
        // 해금 연출 끝
        //
        // → 외부에서 fsm.ChangeState(PlayerStateType.Free) 호출
    }

    public void Exit()
    {
        Debug.Log("LockedState 종료");
        // 입력 복구, 연출 종료 등
        // 역시 FSM 외부에서 처리
        fsm.RaiseUnlocked();
    }
}
