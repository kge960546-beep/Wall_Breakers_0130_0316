using UnityEngine;

public class GoldService : MonoBehaviour
{
    private long credits;

    public void AddGold(int credit)
    {
        credits += credit;
    }
}
