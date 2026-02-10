// 가방 용량 늘어나는 보상
public class BackpackCapacityReward : IAdReward
{
    private int amount;

    public BackpackCapacityReward(int amount)
    {
        this.amount = amount;
    }

    public void Apply()
    {
        if (StackBackPack.Instance == null)
            return;

        StackBackPack.Instance.AddCapacity(amount);
    }

    public void Revert()
    {
        if (StackBackPack.Instance == null)
            return;

        StackBackPack.Instance.AddCapacity(-amount);
    }
}
