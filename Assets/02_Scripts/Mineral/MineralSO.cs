using UnityEngine;

public enum MineralType 
{
    RawMaterial, //원석
    processed    //가공품
}

[CreateAssetMenu(fileName = "MineralData", menuName = "SO/MineralData")]
public class MineralSO : ScriptableObject
{
    public int Id;
    public string mineralName;
    public MineralType mineralType;
    public int price;

    public GameObject muneralPrefab;
    public Sprite icon;

    public MineralSO processedResult; //이 광물을 가공했을시 나오는 가공품SO
}
