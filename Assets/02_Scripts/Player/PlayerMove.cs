using UnityEngine;

/// <summary>
/// 플레이어 이동 처리 (3D 탑뷰)
/// - 입력 방향에 따라 XZ 평면 이동
/// - GameManager 상태 체크
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance;

    [SerializeField] private float moveSpeed = 5f;

    private float baseMoveSpeed;     // 기본 이동속도
    private float bonusMoveSpeed;    // 강화 퍼센트 (0.1 = 10%)

    private Rigidbody rb;
    private Vector3 moveDirection;
    public Vector3 MoveDirection => moveDirection;

    [Header("Footstep")]
    [SerializeField] float FootstepInterval = 0.5f;
    private float footstepTimer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        baseMoveSpeed = moveSpeed; // 기본값 저장
    }

    private void Start()
    {
        if (UpgradeEffectManager.Instance != null)
            UpgradeEffectManager.Instance.OnPlayerMoveSpeedChanged += HandleMoveSpeedChanged;
    }

    private void OnDisable()
    {
        if (UpgradeEffectManager.Instance != null)
            UpgradeEffectManager.Instance.OnPlayerMoveSpeedChanged -= HandleMoveSpeedChanged;
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Footsteps()
    {
        Vector2 horizontalSound = new Vector2(rb.velocity.x, rb.velocity.z);

        if (horizontalSound.magnitude > 0.1f)
        {
            footstepTimer -= Time.fixedDeltaTime;

            if (footstepTimer <= 0)
            {
                SFXManager.instance.PlayOnSFX("Footstep2", transform.position);

                //걷는속도에따라 효과음 조절
                float currentStepInterval = FootstepInterval / (moveSpeed / baseMoveSpeed);
                footstepTimer = currentStepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    public void FootstepsSound()
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            SFXManager.instance.PlayOnSFX("Footstep2", transform.position);
        }
    }

    public void SetMoveDirection(Vector3 dir)
    {
        moveDirection = dir;
    }

    public void AddSpeed(float value)
    {
        moveSpeed += value;
        moveSpeed = Mathf.Max(0f, moveSpeed);
    }

    // 퍼센트 적용 방식으로 수정
    private void HandleMoveSpeedChanged(float totalBonus)
    {
        bonusMoveSpeed = totalBonus;           // 0.1 = 10%
        moveSpeed = baseMoveSpeed * (1f + bonusMoveSpeed);
    }

    private void Move()
    {
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.velocity.y;

        rb.velocity = velocity;
    }
}
