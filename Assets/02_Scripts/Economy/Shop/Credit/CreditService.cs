using System;
using UnityEngine;

public class CreditService
{
    private long _credits;
    public long credits 
    {
        get => _credits;
        private set
        {
            _credits = value;
            Debug.Log($"[CreditService] Credits changed to: {_credits}");
        }
    }

    public event Action<long> OnCreditsChanged;

    public CreditService()
    {
        _credits = 0;
        Debug.Log("[CreditService] CreditService created with initial credits: 0");
    }

    public void AddCredit(int credit)
    {
        Debug.Log($"[CreditService] AddCredit called: +{credit}");

        long oldValue = _credits;
        _credits += credit;

        Debug.Log($"[CreditService] Credits updated: {oldValue} → {_credits}");

        // 이벤트 발생
        OnCreditsChanged?.Invoke(_credits);

        if (OnCreditsChanged == null)
        {
            Debug.LogWarning("[CreditService] OnCreditsChanged has NO subscribers!");
        }
        else
        {
            Debug.Log($"[CreditService] OnCreditsChanged event fired to {OnCreditsChanged.GetInvocationList().Length} subscriber(s)");
        }
    }

    // 테스트용 메서드
    public void SetCredit(long amount)
    {
        Debug.Log($"[CreditService] SetCredit called: {amount}");
        _credits = amount;
        OnCreditsChanged?.Invoke(_credits);
    }

    public bool TrySpendCredit(long amount)
    {
        if (_credits < amount)
            return false;

        _credits -= amount;
        OnCreditsChanged?.Invoke(_credits);
        return true;
    }

}
