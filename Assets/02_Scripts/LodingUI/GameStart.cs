using UnityEngine;

public class GameStart : MonoBehaviour
{
    void Start()
    {
        LoadingSceneController.Instance.LoadScene("MergeSceneMain");
    }
}
