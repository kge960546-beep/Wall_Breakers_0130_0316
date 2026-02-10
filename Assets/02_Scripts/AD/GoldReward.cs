public class GoldReward : IAdReward
{
    private int amount;

    public GoldReward(int amount)
    {
        this.amount = amount;
    }

    public void Apply()
    {
        if (GameManager.Instance == null)
            return;

        CreditService creditService = GameManager.Instance.GetService<CreditService>();
        if (creditService == null)
            return;

        creditService.AddCredit(amount);
    }

    public void Revert()
    {
        // 골드 보상은 되돌리지 않음
    }
}
