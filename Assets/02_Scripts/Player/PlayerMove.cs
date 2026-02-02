using UnityEngine;

/// <summary>
/// 플레이어 이동 처리 (3D 탑뷰)
/// - 입력 방향에 따라 XZ 평면 이동
/// - GameManager 상태 체크
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector3 moveDirection;
    public Vector3 MoveDirection => moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        // 게임이 일시정지 상태면 이동 불가
        //if (!GameManager.Instance.IsPlaying())
        //    return;

        Move();
    }

    // 외부에서 이동 방향 설정
    public void SetMoveDirection(Vector3 dir)
    {
        moveDirection = dir;
    }

    // 실제 이동 처리
    private void Move()
    {
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.velocity.y;

        rb.velocity = velocity;
    }
}
