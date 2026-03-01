// 잠시 이동속도 상승하는 보상
public class MoveSpeedReward : IAdReward
{
    private float amount;

    public MoveSpeedReward(float amount)
    {
        this.amount = amount;
    }

    public void Apply()
    {
        if (PlayerMove.Instance == null)
            return;

        PlayerMove.Instance.AddSpeed(amount);
    }

    public void Revert()
    {
        if (PlayerMove.Instance == null)
            return;

        PlayerMove.Instance.AddSpeed(-amount);
    }
}
