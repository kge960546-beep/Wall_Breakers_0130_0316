using UnityEngine;

public class CreditService
{
    public long credits { get; private set; }

    public void AddCredit(int credit)
    {
        credits += credit;
    }
}
