using UnityEngine;

public class PlaySFXClick : MonoBehaviour
{
    public void OnClickSFX()
    {
        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlayOnSFX("Upgrade2", Camera.main.transform.position);
        }
    }

    public void OnClickSFXPOPSound()
    {
        if (SFXManager.instance != null)
        {
            SFXManager.instance.PlayOnSFX("683097__florianreichelt__bubble-bursting 2", Camera.main.transform.position);
        }
    }
}
