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
    PlayerMoveSpeed,
    PlayerMineSpeed,
    PlayerMaxCarry,
    PlayerMineAmount,
    PlayerRawSellPrice,

    // =========================
    // 자동화 - 광부
    // =========================
    MinerUnlock,
    MinerMineSpeed,

    // =========================
    // 자동화 - 운반인 (ID 기반 통합)
    // =========================
    CarrierUnlock,
    CarrierMoveSpeed,
    CarrierMaxCarry,

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
    // Section1 최종 효과
    // =========================
    MinerMineAmount,
    ProcessedItemSellPriceIncrease,

    // 가공품 판매가격증가
    ProcessedSellPrice
}
