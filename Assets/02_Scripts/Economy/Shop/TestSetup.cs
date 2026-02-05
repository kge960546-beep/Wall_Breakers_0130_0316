using UnityEngine;

public class TestSetup : MonoBehaviour
{
    [Header("Item Data References")]
    [SerializeField] private ItemData iron;
    [SerializeField] private ItemData copper;
    [SerializeField] private ItemData silver;
    [SerializeField] private ItemData ironIngot;
    [SerializeField] private ItemData copperIngot;
    [SerializeField] private ItemData silverIngot;

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