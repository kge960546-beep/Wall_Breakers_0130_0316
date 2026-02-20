using UnityEngine;
using System.Collections;

public class SimpleCarrier : MonoBehaviour
{
    [Header("Points")]
    public Transform machinePoint;
    public Transform sellPoint;

    [Header("References")]
    public AutoBackPackV2 autoBackPack;
    public SellManager sellManager;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float arriveDistance = 0.3f;
    public float rotateSpeed = 10f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private CarrierState state = CarrierState.ToMachine;
    private bool isSelling = false;

    [Header("Upgrade ID")]
    [SerializeField] private string targetID;   // UpgradeEffectManager targetID와 동일하게

    private float baseMoveSpeed;
    private float bonusMoveSpeed;
    private void Awake()
    {
        baseMoveSpeed = moveSpeed;   // 기본 속도 저장
    }

    private void OnEnable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnCarrierMoveSpeedChanged += HandleMoveSpeed;

            // 현재 값 즉시 반영 (이미 활성화된 노드 대응)
            UpgradeEffectManager.Instance.RecalculateAllEffects();
        }
    }

    private void OnDisable()
    {
        if (UpgradeEffectManager.Instance != null)
        {
            UpgradeEffectManager.Instance.OnCarrierMoveSpeedChanged -= HandleMoveSpeed;
        }
    }

    void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    // =========================
    // 이동 처리
    // =========================
    void HandleMovement()
    {
        switch (state)
        {
            case CarrierState.ToMachine:
                MoveTo(machinePoint);

                if (Arrived(machinePoint) && autoBackPack != null && autoBackPack.IsFullBackPack())
                {
                    state = CarrierState.ToSellDesk;
                }
                break;

            case CarrierState.ToSellDesk:
                MoveTo(sellPoint);

                if (Arrived(sellPoint) && !isSelling)
                {
                    StartCoroutine(SellRoutine());
                }
                break;
        }
    }

    void MoveTo(Transform target)
    {
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        // 도착하면 더 이상 미세 이동 안 함
        if (dist < arriveDistance)
            return;

        Vector3 direction = (target.position - transform.position);
        direction.y = 0f;

        // 이동
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime);

        // 회전
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * rotateSpeed);
        }
    }

    bool Arrived(Transform target)
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) < arriveDistance;
    }

    // =========================
    // 애니메이션
    // =========================
    void HandleAnimation()
    {
        if (animator == null) return;

        bool isMoving =
            (state == CarrierState.ToMachine && !Arrived(machinePoint)) ||
            (state == CarrierState.ToSellDesk && !Arrived(sellPoint));

        animator.SetFloat("moveSpeed", isMoving ? 1f : 0f);
    }

    // =========================
    // 판매 루틴
    // =========================
    IEnumerator SellRoutine()
    {
        isSelling = true;

        while (autoBackPack != null && !autoBackPack.IsEmptyBackPack())
        {
            GameObject item = autoBackPack.RemoveResource();

            if (item != null && sellManager != null)
            {
                sellManager.SellFromCarrier(item);
            }

            yield return new WaitForSeconds(0.3f);
        }

        state = CarrierState.ToMachine;
        isSelling = false;
    }
    private void HandleMoveSpeed(string id, float bonus)
    {
        if (id != targetID) return;

        bonusMoveSpeed = bonus;

        moveSpeed = baseMoveSpeed * (1f + bonusMoveSpeed);

        Debug.Log($"[SimpleCarrier:{targetID}] 이동속도 적용 → {moveSpeed}");
    }
}



public enum CarrierState
{
    ToMachine,
    ToSellDesk
}