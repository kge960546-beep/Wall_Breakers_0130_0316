using UnityEngine;

/// <summary>
/// PlayerFSM의 현재 상태를 조회하여
/// Animator 파라미터만 제어하는 표현 전용 클래스
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerFSM fsm;
    private PlayerMove move;

    private readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private readonly int IsMiningHash = Animator.StringToHash("IsMining");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        fsm = GetComponent<PlayerFSM>();
        move = GetComponent<PlayerMove>();
    }

    private void Update()
    {
        UpdateMovementAnimation();
        UpdateMiningAnimation();
        UpdateLookDirection();
    }

    /// <summary>
    /// Idle ↔ Running 제어
    /// </summary>
    private void UpdateMovementAnimation()
    {
        if (fsm == null || move == null)
            return;

        bool isMoving =
            fsm.CurrentStateType == PlayerStateType.Free &&
            move.MoveDirection.sqrMagnitude > 0.001f;

        animator.SetBool(IsMovingHash, isMoving);
    }

    /// <summary>
    /// Mining 상태일 때 Chop 애니메이션 재생
    /// </summary>
    private void UpdateMiningAnimation()
    {
        if (fsm == null)
            return;

        bool isMining = fsm.CurrentStateType == PlayerStateType.Mining;
        animator.SetBool(IsMiningHash, isMining);
    }

    /// <summary>
    /// 이동 방향으로 캐릭터 회전
    /// </summary>
    private void UpdateLookDirection()
    {
        if (move == null)
            return;

        Vector3 dir = move.MoveDirection;
        if (dir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * 10f
        );
    }
}
