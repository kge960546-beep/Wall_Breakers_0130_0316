using UnityEngine;

// 싣는 중 상태
public class PickingUpState : IPlayerState
{
    public PlayerStateType StateType => PlayerStateType.PickingUp;

    private PlayerFSM fsm;

    public PickingUpState(PlayerFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        Debug.Log("PickingUpState 시작");
        fsm.RaisePickingUpStarted();
    }

    public void Update()
    {
        // 종료 조건:
        // 1. 백팩이 가득 찼다
        // 2. 픽업 대상에 더 이상 가져올 자원이 없다

        StackBackPack backPack = fsm.GetComponent<StackBackPack>();
        if (backPack == null)
            return;

        // 1. 지게가 가득 찼으면 종료
        if (backPack.IsFullBackPack())
        {
            fsm.ExitPickingUp();
            return;
        }

        // 2. 픽업 대상이 비었으면 종료
        // 여기에 리소스테이블이 비었다면 종료 조건 추가
    }

    public void Exit()
    {
        Debug.Log("PickingUpState 종료");
        fsm.RaisePickingUpEnded();
        fsm.ClearPickupContext();
    }
}
