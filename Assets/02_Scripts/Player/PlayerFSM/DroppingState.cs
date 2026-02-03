using UnityEngine;

// 내려놓는 중 상태
public class DroppingState : IPlayerState
{
    public PlayerStateType StateType => PlayerStateType.Dropping;

    private PlayerFSM fsm;

    public DroppingState(PlayerFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        Debug.Log("DroppingState 시작");
        fsm.RaiseDroppingStarted();
    }

    public void Update()
    {
        StackBackPack backPack = fsm.GetComponent<StackBackPack>();
        if (backPack == null)
            return;

        // 종료 조건:
        // 1. 지게가 비었다 → 더 이상 내려놓을 것 없음

    }

    public void Exit()
    {
        Debug.Log("DroppingState 종료");
        fsm.RaiseDroppingEnded();
        fsm.ClearDropContext();
    }
}
