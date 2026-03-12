using UnityEngine;

public class ResetButton : MonoBehaviour
{
    private void Update()
    {
        if (SceneGameDataManager.instance != null)
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                SceneGameDataManager.instance.ClearAllSaveData();
            }
        }
    }  
}
