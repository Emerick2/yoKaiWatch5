using System.Collections;
using UnityEngine;

/// <summary>Shared unscaled UI motion and opacity primitives.</summary>
public static class CombatUIAnimator
{
    public static IEnumerator Move(RectTransform target, Vector2 destination, float duration, bool overshoot,
        AnimationCurve easing = null)
    {
        if (target == null) yield break;
        Vector2 start = target.anchoredPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / Mathf.Max(0.001f, duration));
            float eased = Evaluate(easing, t,
                overshoot ? EaseOutBack(t) : 1f - Mathf.Pow(1f - t, 3f));
            target.anchoredPosition = Vector2.LerpUnclamped(start, destination, eased);
            yield return null;
        }
        target.anchoredPosition = destination;
    }

    public static IEnumerator Fade(CanvasGroup target, float alpha, float duration,
        AnimationCurve easing = null)
    {
        if (target == null) yield break;
        float start = target.alpha;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / Mathf.Max(0.001f, duration));
            target.alpha = Mathf.Lerp(start, alpha, Mathf.Clamp01(Evaluate(easing, t, t)));
            yield return null;
        }
        target.alpha = alpha;
    }

    public static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    private static float Evaluate(AnimationCurve easing, float time, float fallback)
    {
        return easing != null && easing.length > 1 ? easing.Evaluate(time) : fallback;
    }
}
