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

        // 상태 전환은 FSM이 관리
        if (fsm.CurrentStateType == PlayerStateType.Free)
        {
            fsm.EnterMining();
        }

        // 실제 채굴은 MiningNode가 담당
        if (fsm.CurrentStateType == PlayerStateType.Mining)
        {
            if (!miningNode.canMine)
            {
                fsm.ExitMining();
                return;
            }                

            if(other.CompareTag("Player"))
            {
                miningNode.TryMine();
            }
            else if(other.CompareTag("AutoMineWorker"))
            {
                miningNode.AutoUnitTryMine();
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
