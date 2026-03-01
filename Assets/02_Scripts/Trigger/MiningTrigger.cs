using UnityEngine;

// 채굴 트리거
public class MiningTrigger : MonoBehaviour
{
    private MiningNode miningNode;

    private void Awake()
    {
        miningNode = GetComponent<MiningNode>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("AutoMineWorker"))
            return;

        PlayerFSM fsm = other.GetComponent<PlayerFSM>();
        if (fsm == null || miningNode == null)
            return;

        // 1. 자원 자체가 있는지 먼저 체크
        if (!miningNode.canMine)
        {
            if (fsm.CurrentStateType == PlayerStateType.Mining) fsm.ExitMining();
            return;
        }

        // 2. 점유권 확인 및 채굴 시도 (여기서 점유권을 따내야만 다음으로 넘어감)
        bool isSuccess = false;

        if (other.CompareTag("Player"))
        {
            isSuccess = miningNode.TryMine(other.gameObject);
        }
        else if (other.CompareTag("AutoMineWorker"))
        {
            isSuccess = miningNode.AutoUnitTryMine(other.gameObject);
        }

        // 3. 점유권 획득에 성공했을 때만 상태 전환 (애니메이션 발생 지점)
        if (isSuccess)
        {
            if (fsm.CurrentStateType == PlayerStateType.Free)
            {
                fsm.EnterMining();
            }
        }
        else
        {
            // 점유권을 못 얻었는데(이미 다른 놈이 캐고 있는데) 
            // 혹시 채굴 애니메이션 중이라면 강제로 끄기
            if (fsm.CurrentStateType == PlayerStateType.Mining)
            {
                fsm.ExitMining();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerFSM fsm = other.GetComponent<PlayerFSM>();
        if (fsm != null && fsm.CurrentStateType == PlayerStateType.Mining)
        {
            fsm.ExitMining();
        }
    }
}
