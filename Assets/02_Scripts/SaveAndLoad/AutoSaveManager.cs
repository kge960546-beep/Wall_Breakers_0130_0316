using System.Collections;
using UnityEngine;

public class AutoSaveManager : MonoBehaviour
{
    [SerializeField] float saveDelay = 300.0f;

    private void Start()
    {
        StopAllCoroutines();

        StartCoroutine(AutoSave());
    }

    IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(saveDelay);

            if(SceneGameDataManager.instance != null)
            {
                SceneGameDataManager.instance.SaveGame();
            }
        }
    }
}
