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

            if (SceneGameDataManager.instance != null)
            {
                if (SceneGameDataManager.instance.isRefreshing)
                {
                    Utils.DebugLog("복구중이라 자동저장 못함");
                    continue;
                }

                SceneGameDataManager.instance.SaveGame();
                Utils.DebugLog("자동 저장 완료");
            }
        }
    }
}
