using UnityEngine;

/// <summary>
/// Settings 패널 ON / OFF 제어용
/// </summary>
public class SettingsPanelController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    public void Open()
    {
        Debug.Log("Settings Open Clicked");
        settingsPanel.SetActive(true);
    }

    public void Close()
    {
        settingsPanel.SetActive(false);
    }
}
