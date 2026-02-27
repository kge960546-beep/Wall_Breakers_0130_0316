using UnityEngine;

// 내려놓는 중 상태
public class DroppingState : IPlayerState
{
    public PlayerStateType StateType => PlayerStateType.Dropping;

    private PlayerFSM fsm;
    private bool dropped; // 드롭 완료 여부 (1회성 보호)

    public DroppingState(PlayerFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        dropped = false;
        fsm.RaiseDroppingStarted();
    }

    public void Update()
    {
        // ProcessResource에서 이미 실제 드롭은 완료됨
        // 상태는 연출/애니메이션용으로만 잠깐 유지

        if (!dropped)
        {
            dropped = true;
            fsm.ExitDropping(); // 반드시 종료
        }
    }

    public void Exit()
    {
        fsm.RaiseDroppingEnded();
        fsm.ClearDropContext();
    }
}
