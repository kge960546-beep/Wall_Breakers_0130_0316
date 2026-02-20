using UnityEngine;

/// <summary>
/// 상점 주인 애니메이션 트리거용 포인트
/// - 플레이어가 포인트 위에 올라가면 애니메이션전환 ON
/// - 내려오면 기본 Idle
/// - 여러 포인트에서 공용으로 사용 가능
/// </summary>
public class ShopInteractPoint : MonoBehaviour
{
    [SerializeField] private Animator shopAnimator;

    private static readonly int IsCheeringHash = Animator.StringToHash("IsCheering");

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") &&
            !other.CompareTag("AutoCarrierWorker"))
            return;

        shopAnimator.SetBool(IsCheeringHash, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") &&
            !other.CompareTag("AutoCarrierWorker"))
            return;

        shopAnimator.SetBool(IsCheeringHash, false);
    }
}
