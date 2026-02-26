using UnityEngine;

public partial class UIBillboard : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        // 메인 카메라의 트랜스폼을 캐싱합니다.
        if (Camera.main != null)
            camTransform = Camera.main.transform;
    }

    // UI 위치는 Update나 FixedUpdate보다 늦게 계산되는 LateUpdate가 적합합니다.
    void LateUpdate()
    {
        if (camTransform == null) return;

        // 방식 1: 카메라와 방향을 일치시키기 (가장 깔끔함)
        transform.rotation = camTransform.rotation;

        /* 방식 2: 카메라를 직접 쳐다보게 하기 (원근감에 따라 UI가 약간 휠 수 있음)
        transform.LookAt(transform.position + camTransform.forward); 
        */
    }
}