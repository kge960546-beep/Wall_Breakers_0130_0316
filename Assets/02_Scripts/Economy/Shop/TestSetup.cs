using UnityEngine;

public class TestSetup : MonoBehaviour
{
    [Header("Item Data References")]
    [SerializeField] private ItemDataSO iron;
    [SerializeField] private ItemDataSO copper;
    [SerializeField] private ItemDataSO silver;
    [SerializeField] private ItemDataSO ironIngot;
    [SerializeField] private ItemDataSO copperIngot;
    [SerializeField] private ItemDataSO silverIngot;

    private void Start()
    {
        // 테스트용 아이템 추가
        PlayerInventory.Instance.AddItem(iron, 5);
        PlayerInventory.Instance.AddItem(copper, 3);
        PlayerInventory.Instance.AddItem(silver, 2);
        PlayerInventory.Instance.AddItem(ironIngot, 4);
        PlayerInventory.Instance.AddItem(copperIngot, 2);
        PlayerInventory.Instance.AddItem(silverIngot, 1);

        Debug.Log("Test inventory setup complete");
    }
}