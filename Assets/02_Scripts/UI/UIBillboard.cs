using UnityEngine;

public partial class UIBillboard : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        // 메인 카메라의 트랜스폼을 캐싱
        if (Camera.main != null)
            camTransform = Camera.main.transform;
    }

    // UI 위치는 Update나 FixedUpdate보다 늦게 계산되는 LateUpdate가 적합
    void LateUpdate()
    {
        if (camTransform == null) return;

        // 카메라와 방향을 일치시키기
        transform.rotation = camTransform.rotation;
    }
}