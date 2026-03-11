using UnityEngine;
using UnityEngine.AI;


public enum navMeshCurrentState { Idle, MovingToTable, MovingMachine, }
public class AutoNavMeshCarrier : MonoBehaviour
{
    [SerializeField] private AutoBackPack autoBackPack; //가방 스크립트 참조
    [SerializeField] private Animator anim;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Transform resourceTable;        //자원 놓는 테이블 위치
    [SerializeField] Transform processingMachine;    //가공기 위치
    [SerializeField] Transform idleSpot;             //대기 위치
    [SerializeField] float moveDelay = 4.0f;         //이동후 딜레이
    [SerializeField] float stopDistance = 0.5f;      //목적지 도착 거리

    [SerializeField] private float moveSpeed = 5f;   //이동 속도

    private Rigidbody rb;
    private Vector3 moveDirection;
    public Vector3 MoveDirection => moveDirection;
    private navMeshCurrentState state = navMeshCurrentState.Idle;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (autoBackPack == null)
        {
            autoBackPack = GetComponent<AutoBackPack>();
        }
    }

    private void FixedUpdate()
    {
        HandleAuto();
        UpdateAnim();
    }

    public void SetMoveDirection(Vector3 dir)
    {
        moveDirection = dir;
    }

    public void UpdateAnim()
    {
        if (anim != null)
        {
            float currentSpeed = agent.velocity.magnitude;
            anim.SetFloat("moveSpeed", currentSpeed);
        }
    }

    // 실제 이동 처리
    private void Move()
    {
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.velocity.y;

        rb.velocity = velocity;

        if (anim != null)
        {
            float currentSpeed = moveDirection.magnitude;
            anim.SetFloat("moveSpeed", currentSpeed);
        }

        if (moveDirection != Vector3.zero)
        {
            rb.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    //자동 운반 상태 처리
    public void HandleAuto()
    {
        switch (state)
        {
            case (navMeshCurrentState.Idle):
                if (!autoBackPack.IsFullBackPack())
                {
                    state = navMeshCurrentState.MovingToTable;
                }
                else
                {
                    state = navMeshCurrentState.MovingMachine;
                }
                break;
            case (navMeshCurrentState.MovingToTable):
                GoTargetPoint(resourceTable.position);
                break;
            case (navMeshCurrentState.MovingMachine):
                GoTargetPoint(processingMachine.position);
                break;
        }
    }

    //네비매쉬 이동으로 변경
    public void GoTargetPoint(Vector3 targetPos)
    {
        if (agent.destination != targetPos)
        {
            agent.SetDestination(targetPos);
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            state = navMeshCurrentState.Idle;
        }
        //float distanceToTarget = Vector3.Distance(transform.position, targetPos);
        //
        //if (distanceToTarget <= stopDistance)
        //{
        //    SetMoveDirection(Vector3.zero);
        //    state = currentState.Idle;
        //
        //}
        //else
        //{
        //    Vector3 direction = (targetPos - transform.position).normalized; //이동 방향 계산
        //    direction.y = 0f; // Y축 이동 무시
        //    SetMoveDirection(direction);
        //}
    }
}
