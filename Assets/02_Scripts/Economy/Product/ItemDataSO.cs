using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item/Item Data")]
public class ItemDataSO : ScriptableObject
{
    public int Id;
    public string itemName;
    public ItemType itemType;
    public int sellPrice;
    public float sellDuration = 1f; // 기본 판매 시간

    public int inputAmountPerProcess; //가공시 필요한 원재료 수량
    public ItemDataSO processedResult; //이 광물을 가공했을시 나오는 가공품SO

    public Vector3 backPackRotationOffset; //가방에 담길때 회전 오프셋

    public GameObject mineralPrefab;
    public Sprite icon;
}

public enum ItemType
{
    RawMaterial, //원석
    processed    //가공품
}