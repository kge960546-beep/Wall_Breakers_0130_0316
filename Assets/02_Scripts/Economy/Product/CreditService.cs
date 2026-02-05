using System;
using UnityEngine;

public class CreditService
{
    public long credits { get; private set; }

    public event Action<long> OnCreditsChanged;

    public void AddCredit(int credit)
    {
        credits += credit;
        OnCreditsChanged?.Invoke(credits);
    }
}
