using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum aStarCurrentState { Idle, MovingToTable, MovingMachine, Processing }
public class AutoAStarCarrier : MonoBehaviour
{
    [SerializeField] private AutoBackPack autoBackPack; //가방 스크립트 참조
    [SerializeField] private Animator anim;
    [SerializeField] AStarFindPath pathFinder;    //A스타 경로 찾기 스크립트 참조


    [SerializeField] Transform resourceTable;        //자원 놓는 테이블 위치
    [SerializeField] Transform processingMachine;    //가공기 위치
    [SerializeField] Transform idleSpot;             //대기 위치
    [SerializeField] float moveDelay = 4.0f;         //이동후 딜레이
    [SerializeField] float stopDistance = 0.5f;      //목적지 도착 거리

    [SerializeField] private float moveSpeed = 5f;   //이동 속도
    [SerializeField] private float rotateSpeed = 30f; //회전 속도

    private Rigidbody rb;
    private Vector3 moveDirection;
    public Vector3 MoveDirection => moveDirection;
    private aStarCurrentState state = aStarCurrentState.Idle;

    private List<Vector2Int> currentPath;
    private int pathIndex = 0;
    private bool isMoving = false;

    private void Awake()
    {
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
        Move();
    }

    //이동 후 딜레이 처리
    IEnumerator MoveDelay()
    {
        state = aStarCurrentState.Processing;

        SetMoveDirection(Vector3.zero);
        currentPath = null;
        
        yield return new WaitForSeconds(moveDelay);
        
        state = aStarCurrentState.Idle;
    }
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

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        if (anim != null)
        {
            float currentSpeed = moveDirection.magnitude;
            anim.SetFloat("moveSpeed", currentSpeed);
        }
    }

    //자동 운반 상태 처리
    public void HandleAuto()
    {
        switch (state)
        {
            case (aStarCurrentState.Idle):
                if (!autoBackPack.IsFullBackPack())
                {
                    state = aStarCurrentState.MovingToTable;
                }
                else
                {
                    state = aStarCurrentState.MovingMachine;
                }
                break;
            case (aStarCurrentState.MovingToTable):
                GoTargetPoint(resourceTable.position);
                break;
            case (aStarCurrentState.MovingMachine):
                GoTargetPoint(processingMachine.position);
                break;
        }
    }

    //네비매쉬 이동으로 변경
    public void GoTargetPoint(Vector3 targetPos)
    {
        //경로가 없으면 새로 계산
        if (currentPath == null || currentPath.Count == 0)
        {
            Vector2Int start = pathFinder.WorldToGrid(transform.position);
            Vector2Int end = pathFinder.WorldToGrid(targetPos);

            currentPath = pathFinder.GetAstarPath(start, end);
            pathIndex = 0;

            if (currentPath == null) return;
        }

        //경로가 있으면 다음 노드로 이동
        if (pathIndex < currentPath.Count)
        {
            Vector3 nextPos = pathFinder.GridToWorld(currentPath[pathIndex]);

            nextPos.y = transform.position.y;

            Vector3 offset = (nextPos - transform.position);
            offset.y = 0f;
            float distance = offset.magnitude;

            if (distance < stopDistance)
            {
                pathIndex++;
            }
            else
            {
                SetMoveDirection(offset.normalized);
            }
        }
        else //목적지 도착
        {
            if(state != aStarCurrentState.Processing)
            {
                StartCoroutine(MoveDelay());
            }

            SetMoveDirection(Vector3.zero);
            currentPath = null;            
        }
    }

    private void OnDrawGizmos()
    {
        if (currentPath != null && pathIndex < currentPath.Count)
        {
            Gizmos.color = Color.cyan; // 경로 선 색상

            for (int i = pathIndex; i < currentPath.Count - 1; i++)
            {
                // 1. 경로 전체를 시각화 (파란색 선과 구체)
                Vector3 startNode = pathFinder.GridToWorld(currentPath[i]);
                Vector3 endNode = pathFinder.GridToWorld(currentPath[i + 1]);

                // 선을 약간 위로 올려서 그리기
                startNode.y += 0.1f;
                endNode.y += 0.1f;

                Gizmos.DrawLine(startNode, endNode);

                // 노드를 약간 위로 올려서 그리기
                Gizmos.DrawWireSphere(endNode, 0.2f);
            }

            // 2. 현재 목표 노드 표시 (노란색 구체)
            Gizmos.color = Color.yellow;
            Vector3 nextTarget = pathFinder.GridToWorld(currentPath[pathIndex]);
            nextTarget.y += 0.2f;
            Gizmos.DrawSphere(nextTarget, 0.3f);
        }

        // 3. 목적지 오브젝트 표시
        if (state == aStarCurrentState.MovingToTable && resourceTable != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(resourceTable.position, Vector3.one * 0.5f);
        }
        else if (state == aStarCurrentState.MovingMachine && processingMachine != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(processingMachine.position, Vector3.one * 0.5f);
        }
    }
}
