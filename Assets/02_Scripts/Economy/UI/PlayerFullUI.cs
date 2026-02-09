using UnityEngine;

/// <summary>
/// 인벤토리가 가득 찼을 때
/// 플레이어 머리 위에 FULL UI를 표시
/// </summary>
public class PlayerFullUI : MonoBehaviour
{
    [SerializeField] private GameObject fullUI;     // FULL UI 오브젝트
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.2f, 0f);

    private Transform target; // 플레이어

    private void Awake()
    {
        target = transform;
        fullUI.SetActive(false);
    }

    private void LateUpdate()
    {
        // 항상 플레이어 머리 위로 위치 고정
        fullUI.transform.position = target.position + offset;
        fullUI.transform.forward = Camera.main.transform.forward;
    }

    public void Show()
    {
        fullUI.SetActive(true);
    }

    public void Hide()
    {
        fullUI.SetActive(false);
    }
}
