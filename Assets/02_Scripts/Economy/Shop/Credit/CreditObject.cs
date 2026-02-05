using UnityEngine;

public class CreditObject : MonoBehaviour
{
    [SerializeField] private int creditAmount;
    [SerializeField] private float collectDelay = 0.1f; // 수집 간격

    public int CreditAmount => creditAmount;
    public bool IsCollected { get; private set; } = false;

    public void Initialize(int amount)
    {
        creditAmount = amount;
    }
    public void Collect()
    {
        if (IsCollected) return;
        IsCollected = true;
    }
}