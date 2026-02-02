using System.Collections;
using UnityEngine;

/// <summary>
/// 카메라의 모든 이동 / 연출을 전담하는 매니저
/// </summary>
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [Header("Follow Target")]
    [SerializeField] private Transform player;

    [Header("Follow Offset")]
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 15f, -15f);

    [Header("Follow Speed")]
    [SerializeField] private float followSmooth = 5f;

    [Header("Focus Camera Settings")]
    [SerializeField] private float focusMoveTime = 0.6f;   // 타겟으로 이동하는 데 걸리는 시간
    [SerializeField] private float returnMoveTime = 0.6f;  // 플레이어로 복귀하는 데 걸리는 시간
    [SerializeField] private float focusHeightOffset = 2f; // 살짝 위에서 보여주기

    private bool isFollowing = true;
    private Coroutine focusRoutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void LateUpdate()
    {
        if (!isFollowing || player == null)
            return;

        Vector3 targetPos = player.position + followOffset;
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * followSmooth
        );
    }

    public void FocusOnTarget(Transform target, float focusDuration = 2f)
    {
        if (target == null)
            return;

        if (focusRoutine != null)
            StopCoroutine(focusRoutine);

        focusRoutine = StartCoroutine(FocusRoutine(target, focusDuration));
    }

    private IEnumerator FocusRoutine(Transform target, float duration)
    {
        isFollowing = false;

        Vector3 startPos = transform.position;
        Vector3 focusPos = target.position + followOffset + Vector3.up * focusHeightOffset;

        // ===== 타겟으로 이동 =====
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / focusMoveTime;
            transform.position = Vector3.Lerp(startPos, focusPos, t);
            yield return null;
        }

        // ===== 잠시 보여주기 =====
        yield return new WaitForSeconds(duration);

        // ===== 플레이어로 복귀 =====
        startPos = transform.position;
        Vector3 returnPos = player.position + followOffset;
        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / returnMoveTime;
            transform.position = Vector3.Lerp(startPos, returnPos, t);
            yield return null;
        }

        isFollowing = true;
        focusRoutine = null;
    }
}
