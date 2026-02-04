using UnityEngine;

// 자동 상호작용 트리거 공통 베이스
// 일종의 상태전환 요청자
public abstract class AutoInteractTrigger : MonoBehaviour
{
    protected PlayerFSM playerFSM;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerFSM = other.GetComponent<PlayerFSM>();
        if (playerFSM == null) return;

        TryEnter(playerFSM);
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (playerFSM == null) return;

        TryStay(playerFSM);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        TryExit(playerFSM);
        playerFSM = null;
    }

    // 각 트리거가 구현해야 할 부분
    protected abstract void TryEnter(PlayerFSM fsm);
    protected abstract void TryStay(PlayerFSM fsm);
    protected abstract void TryExit(PlayerFSM fsm);
}
