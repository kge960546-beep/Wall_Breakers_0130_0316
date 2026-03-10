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

        //if (SceneGameDataManager.instance != null)
        //{
        //    SceneGameDataManager.instance.currentGold += creditAmount;
        //}
        // 중복 지급형태가 될 수 있는거 같아 임시 주석 처리 3-10 Won Add
    }
}