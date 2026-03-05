using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUIDescPanel : MonoBehaviour
{
    [SerializeField] private Image nodeIcon;
    [SerializeField] private TMP_Text nodeName;
    [SerializeField] private TMP_Text nodeDesc;
    [SerializeField] private TMP_Text nodeCost;

    public void SetInfo(Sprite icon, string name, string desc, int cost)
    {
        nodeIcon.sprite = icon;
        nodeName.text = name;
        nodeDesc.text = desc;
        nodeCost.text = cost.ToString();
    }
}