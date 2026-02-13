using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeEffect
{
    public UpgradeType upgradeType;
    public float value;
}

[CreateAssetMenu(menuName = "Upgrade/Upgrade Data")]
public class UpgradeDataSO : ScriptableObject
{
    [Header("기본 정보")]
    public string upgradeID;
    public string displayName;
    [TextArea]
    public string description;

    [Header("강화 효과 (복수 가능)")]
    public List<UpgradeEffect> effects = new List<UpgradeEffect>();

    [Header("공통 설정")]
    public int cost;
    public int section;
}
