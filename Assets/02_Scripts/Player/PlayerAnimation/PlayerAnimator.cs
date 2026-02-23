using UnityEngine;

/// <summary>
/// PlayerFSM의 행동 이벤트를 구독하여
/// 플레이어 애니메이션을 제어하는 표현 전용 클래스
///
/// - Idle / Run : Free 상태 + 이동 입력 기반
/// - Mining    : FSM 이벤트(OnMiningStarted / Ended) 기반
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerFSM fsm;
    private PlayerMove move;
    private ToolsEquip toolsEquip;

    private bool trailActive;

    private readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private readonly int IsMiningHash = Animator.StringToHash("IsMining");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        fsm = GetComponent<PlayerFSM>();
        move = GetComponent<PlayerMove>();
        toolsEquip = GetComponent<ToolsEquip>();
    }

    private void OnEnable()
    {
        if (fsm == null)
            return;

        // Mining 행동 이벤트 구독
        fsm.OnMiningStarted += HandleMiningStarted;
        fsm.OnMiningEnded += HandleMiningEnded;
    }

    private void OnDisable()
    {
        if (fsm == null)
            return;

        fsm.OnMiningStarted -= HandleMiningStarted;
        fsm.OnMiningEnded -= HandleMiningEnded;
    }

    private void Update()
    {
        UpdateMovementAnimation();
        UpdateLookDirection();
    }

    private void LateUpdate()
    {
        HandleMiningTrail();
    }
    /// <summary>
    /// Idle ↔ Running 제어
    /// (Free 상태에서만 이동 애니메이션 허용)
    /// </summary>
    private void UpdateMovementAnimation()
    {
        if (fsm == null || move == null)
            return;

        if (fsm.CurrentStateType != PlayerStateType.Free)
        {
            animator.SetBool(IsMovingHash, false);
            return;
        }

        bool isMoving = move.MoveDirection.sqrMagnitude > 0.001f;
        animator.SetBool(IsMovingHash, isMoving);
    }

    /// <summary>
    /// Mining 시작 시 Chop 애니메이션 재생
    /// </summary>
    private void HandleMiningStarted()
    {
        animator.SetBool(IsMiningHash, true);
    }

    /// <summary>
    /// Mining 종료 시 Chop 애니메이션 종료
    /// </summary>
    private void HandleMiningEnded()
    {
        animator.SetBool(IsMiningHash, false);
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

    /// <summary>
    /// 
    /// </summary>
    private void HandleMiningTrail()
    {
        if(fsm.CurrentStateType != PlayerStateType.Mining)
        {
            if(trailActive)
            {
                toolsEquip.TrailOff();
                trailActive = false;
            }
            return;
        }

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (!state.IsName("Chopping")) 
            return;

        float time = state.normalizedTime;

        // 루프 애니메이션 보정
        if (time > 1f)
            time -= Mathf.Floor(time);

        // 실제 타격 구간
        bool shouldTrail = time >= 0f && time <= 0.3f;

        if(shouldTrail && !trailActive)
        {
            toolsEquip.TrailOn();
            trailActive = true;
        }
        else if (!shouldTrail && trailActive)
        {
            toolsEquip.TrailOff();
            trailActive = false;
        }
    }
}
