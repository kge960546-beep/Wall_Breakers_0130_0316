using UnityEngine;

[CreateAssetMenu(fileName = "New Region", menuName = "Game/Region System/Region Data")]
public class RegionData : ScriptableObject
{
    [Header("Region Info")]
    public int regionId;
    public string regionName;
    public Sprite regionIcon;

    [Header("Unlock Requirments")]
    public RegionRequirment unlockRequirement;

    [Header("Descrition")]
    [TextArea(3, 5)]
    public string description;
}

