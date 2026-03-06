using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class UITween
{
    // ================================
    // Scale Tween
    // ================================
    public static IEnumerator Scale(Transform target, Vector3 start, Vector3 end, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            target.localScale = Vector3.Lerp(start, end, t);

            yield return null;
        }

        target.localScale = end;
    }

    // ================================
    // Fade Tween
    // ================================
    public static IEnumerator Fade(CanvasGroup canvas, float start, float end, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            canvas.alpha = Mathf.Lerp(start, end, t);

            yield return null;
        }

        canvas.alpha = end;
    }

    // ================================
    // Color Tween
    // ================================
    public static IEnumerator ColorTween(Image image, Color start, Color end, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            image.color = Color.Lerp(start, end, t);

            yield return null;
        }

        image.color = end;
    }

    // ================================
    // Bezier Move (곡선 이동)
    // ================================
    public static IEnumerator MoveBezier(Transform target, Vector3 p0, Vector3 p1, Vector3 p2, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            Vector3 pos =
                Mathf.Pow(1 - t, 2) * p0 +
                2 * (1 - t) * t * p1 +
                Mathf.Pow(t, 2) * p2;

            target.position = pos;

            yield return null;
        }

        target.position = p2;
    }

    // ================================
    // UI Move Tween (직선 이동)
    // ================================
    public static IEnumerator MoveUI(RectTransform target, Vector2 start, Vector2 end, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            // EaseOut 적용
            t = 1f - Mathf.Pow(1f - t, 3f);

            target.anchoredPosition = Vector2.Lerp(start, end, t);

            yield return null;
        }

        target.anchoredPosition = end;
    }

}