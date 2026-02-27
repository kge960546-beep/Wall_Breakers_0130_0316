using UnityEngine;

// 싣는 중 상태
public class PickingUpState : IPlayerState
{
    public PlayerStateType StateType => PlayerStateType.PickingUp;

    private PlayerFSM fsm;
    private bool picked; // 픽업 완료 여부 (1회성 보호)

    public PickingUpState(PlayerFSM fsm)
    {
        this.fsm = fsm;
    }

    public void Enter()
    {
        picked = false;
        fsm.RaisePickingUpStarted();
    }

    public void Update()
    {
        StackBackPack backPack = fsm.GetComponent<StackBackPack>();
        if (backPack == null)
            return;

        // 1 백팩이 가득 찼으면 종료
        if (backPack.IsFullBackPack())
        {
            fsm.ExitPickingUp();
            return;
        }

        // 2 실제 픽업은 ProcessResource에서 이미 완료됨
        // 상태는 연출용으로 한 프레임만 유지
        if (!picked)
        {
            picked = true;
            fsm.ExitPickingUp();
        }
    }

    public void Exit()
    {
        fsm.RaisePickingUpEnded();
        fsm.ClearPickupContext();
    }
}
