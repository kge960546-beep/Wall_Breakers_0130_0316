// 플레이어가 가지는 각 상태가 반드시 구현해야하는 규약
public interface IPlayerState
{
    PlayerStateType StateType { get; }

    void Enter();      // 상태 진입
    void Update();     // 상태 유지
    void Exit();       // 상태 종료
}
