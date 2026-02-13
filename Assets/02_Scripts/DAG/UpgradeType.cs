public enum UpgradeType
{
    None,

    // =========================
    // 섹션 루트
    // =========================
    SectionRoot,

    // =========================
    // 플레이어 직접 강화
    // =========================
    PlayerMoveSpeed,          // 플레이어 이동속도 (단일)

    PlayerMineSpeed,          // 플레이어 채굴속도 (3단계)
    PlayerMaxCarry,           // 플레이어 최대 적재량 (3단계)
    PlayerMineAmount,         // 플레이어 1회 채굴 수량 (3단계)

    PlayerSellPrice,          // 플레이어 직접 판매 가격 상승

    // =========================
    // 자동화 - 광부
    // =========================
    MinerUnlock,              // 광부 해금 (1,2명 누적)
    MinerMineSpeed,           // 광부 채굴속도 증가

    // =========================
    // 자동화 - 운반인 A
    // =========================
    CarrierAUnlock,
    CarrierAMoveSpeed,
    CarrierAMaxCarry,

    // =========================
    // 자동화 - 운반인 B
    // =========================
    CarrierBUnlock,
    CarrierBMoveSpeed,
    CarrierBMaxCarry,

    // =========================
    // 채굴구역 강화
    // =========================
    MiningAreaMaxStorage,
    MiningAreaRespawnReduce,

    // =========================
    // 가공기계 강화
    // =========================
    ProcessorMaxStorage,
    ProcessorProcessTimeReduce,

    // =========================
    // Section1 최종 종착 효과
    // =========================
    MiningOneTimeAmountIncrease,      // 광부 1,2 1회 채굴 수량 증가
    ProcessedItemSellPriceIncrease    // 가공품 판매가격 증가
}
