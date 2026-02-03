using UnityEngine;

// 채굴 중 상태
public class MiningState : IPlayerState
{
    public PlayerStateType StateType => PlayerStateType.Mining;

    private PlayerFSM fsm;

    public MiningState(PlayerFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        Debug.Log("MiningState 시작");
        // 채굴 상태 진입 알림
        fsm.RaiseMiningStarted();
    }

    public void Update()
    {
        // 이 State의 Update 책임:
        // - "채굴 상태를 유지할 수 있는지" 조건만 판단
        // - 실제 채굴 처리, 수량 변화, 애니메이션은 관여하지 않음

        // 체크 대상 (판단만 수행):
        // 만일 창고가 가득찼다면
        // 리소스테이블의 최대용량까지 가득찼다면 상태 종료 조건 명시
    }

    public void Exit()
    {
        Debug.Log("MiningState 종료");
        // 채굴 상태 종료 알림
        fsm.RaiseMiningEnded();
    }
}
