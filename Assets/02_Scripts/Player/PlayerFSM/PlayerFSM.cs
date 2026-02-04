using System;
using UnityEngine;

/// <summary>
/// 플레이어의 행동 상태를 단일 FSM으로 관리하는 컨트롤러.
///
/// 목적:
/// - 플레이어가 동시에 여러 행동(채굴, 적재, 판매 등)을 하지 않도록 상태를 하나로 제한한다.
/// - 상태 충돌로 인한 애니메이션 / 연출 중복을 방지한다.
///
/// 설계 방식:
/// - FSM은 상태 전환과 상태 생명주기 관리만 담당한다.
/// - 실제 로직(수량 변화, 애니메이션, VFX, SFX 등)은 FSM 외부에서 처리한다.
/// - 상태 진입/종료 시 이벤트를 발생시키는 허브 역할을 한다.
///
/// 사용 흐름:
/// - 트리거(광산, 창고, 판매대 등)는 FSM에게 상태 전환 "요청"만 한다.
///   예) EnterPickingUp(), EnterMining()
/// - 각 State는 Update에서 종료 조건만 체크한다.
/// - 애니메이션, 연출, 수량 변화는 FSM 이벤트를 구독한 객체들이 처리한다.
/// </summary>
public class PlayerFSM : MonoBehaviour
{
    // ===== 현재 상태 =====
    public PlayerStateType CurrentStateType => currentState.StateType;
    private IPlayerState currentState;

    // ===== 상태 인스턴스 =====
    private FreeState freeState;
    private MiningState miningState;
    private PickingUpState pickingUpState;
    private DroppingState droppingState;
    private LockedState lockedState;

    // ===== 현재 상호작용 맥락 =====
    // 무엇을 픽업 / 드롭하는지에 대한 정보 보관용
    public PickupType CurrentPickupType { get; private set; }
    public DropType CurrentDropType { get; private set; }

    // ===== 상태 이벤트 =====
    public event Action OnMiningStarted;
    public event Action OnMiningEnded;

    public event Action OnPickingUpStarted;
    public event Action OnPickingUpEnded;

    public event Action OnDroppingStarted;
    public event Action OnDroppingEnded;

    public event Action OnLocked;
    public event Action OnUnlocked;

    private void Awake()
    {
        // 상태 인스턴스는 한 번만 생성 (GC 방지)
        freeState = new FreeState(this);
        miningState = new MiningState(this);
        pickingUpState = new PickingUpState(this);
        droppingState = new DroppingState(this);
        lockedState = new LockedState(this);
    }

    private void Start()
    {
        ChangeState(PlayerStateType.Free);
    }

    private void Update()
    {
        currentState?.Update();
    }

    // ===== 외부(Trigger)가 호출하는 진입 함수들 =====

    // 채굴 상태 진입 요청
    public void EnterMining()
    {
        if (CurrentStateType != PlayerStateType.Free)
            return;

        ChangeState(PlayerStateType.Mining);
    }

    // 픽업 상태 진입 요청
    public void EnterPickingUp(PickupType pickupType)
    {
        if (CurrentStateType != PlayerStateType.Free)
            return;

        CurrentPickupType = pickupType;
        ChangeState(PlayerStateType.PickingUp);
    }

    // 드롭 상태 진입 요청
    public void EnterDropping(DropType dropType)
    {
        if (CurrentStateType != PlayerStateType.Free)
            return;

        CurrentDropType = dropType;
        ChangeState(PlayerStateType.Dropping);
    }

    // 채굴 상태 종료 요청
    public void ExitMining()
    {
        if (CurrentStateType != PlayerStateType.Mining)
            return;

        ChangeState(PlayerStateType.Free);
    }

    // 픽업 상태 종료 요청
    public void ExitPickingUp()
    {
        if (CurrentStateType != PlayerStateType.PickingUp)
            return;

        ChangeState(PlayerStateType.Free);
    }

    // 드롭 상태 종료 요청
    public void ExitDropping()
    {
        if (CurrentStateType != PlayerStateType.Dropping)
            return;

        ChangeState(PlayerStateType.Free);
    }

    // ===== 상태 변경 (FSM 내부 전용) =====
    private void ChangeState(PlayerStateType newState)
    {
        currentState?.Exit();

        currentState = newState switch
        {
            PlayerStateType.Free => freeState,
            PlayerStateType.Mining => miningState,
            PlayerStateType.PickingUp => pickingUpState,
            PlayerStateType.Dropping => droppingState,
            PlayerStateType.Locked => lockedState,
            _ => freeState
        };

        currentState.Enter();
    }

    // ===== 이벤트 Raise 함수 =====
    // State에서 호출, 실제 처리자는 외부 구독자

    public void RaiseMiningStarted() => OnMiningStarted?.Invoke();
    public void RaiseMiningEnded() => OnMiningEnded?.Invoke();

    public void RaisePickingUpStarted() => OnPickingUpStarted?.Invoke();
    public void RaisePickingUpEnded() => OnPickingUpEnded?.Invoke();

    public void RaiseDroppingStarted() => OnDroppingStarted?.Invoke();
    public void RaiseDroppingEnded() => OnDroppingEnded?.Invoke();

    public void RaiseLocked() => OnLocked?.Invoke();
    public void RaiseUnlocked() => OnUnlocked?.Invoke();

    // ===== 맥락 정리 =====
    // State Exit 시 호출하는 용도

    public void ClearPickupContext()
    {
        CurrentPickupType = default;
    }

    public void ClearDropContext()
    {
        CurrentDropType = default;
    }
}
