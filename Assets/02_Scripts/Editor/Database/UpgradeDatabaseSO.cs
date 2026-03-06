using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UgradeData",menuName = "Game/Database/Upgrade Database")]
public class UpgradeDatabaseSO : ScriptableObject
{
    public List<UpgradeDataSO> upgrades = new();
}
