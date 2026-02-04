using UnityEngine;

/// <summary>
/// 카메라 FocusOnTarget 테스트용 스크립트
/// </summary>
public class TestCameraFocus : MonoBehaviour
{
    [SerializeField] private Transform focusTarget;

    private void Update()
    {
        // T 키 누르면 포커스 테스트
        if (Input.GetKeyDown(KeyCode.T))
        {
            CameraManager.Instance.FocusOnTarget(focusTarget, 2f);
        }
    }
}
