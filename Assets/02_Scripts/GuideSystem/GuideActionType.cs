public enum GuideActionType
{
    None,

    // 채굴
    MineSoil,

    // 판매
    SellSoil,
    SellBrick,

    // 제작
    CraftBrick,

    // 강화
    UpgradeAny,                 // 아무 강화 1회
    UpgradePlayerMineSpeed,     // 플레이어 채굴속도 강화
    UpgradePlayerTreeComplete,  // 섹션1 플레이어 트리 모두 강화
    UpgradeAutomationTreeComplete, // 섹션1 자동화 트리 모두 강화

    // 해금
    UnlockCarrierA,
    UnlockSection2
}