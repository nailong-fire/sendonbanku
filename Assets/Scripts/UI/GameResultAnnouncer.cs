using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameResultAnnouncer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;
    [SerializeField] private Image resultImage;

    [Header("Sprites")]
    public Sprite winSprite;
    public Sprite loseSprite;

    [Header("Animation")]
    public float slideDuration = 0.35f;
    public float fadeDuration = 0.2f;
    public float holdDuration = 1.5f;
    public Vector2 anchoredTargetPos = Vector2.zero;
    public float startOffsetX = 900f;

    Coroutine _routine;

    void Reset()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        panel = GetComponentInChildren<RectTransform>();
        resultImage = GetComponentInChildren<Image>();
    }

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponentInChildren<CanvasGroup>(true);

        if (panel == null)
            panel = GetComponentInChildren<RectTransform>(true);

        if (resultImage == null)
            resultImage = GetComponentInChildren<Image>(true);

        HideImmediate();
    }

    public void ShowWin() => Show(winSprite);

    public void ShowLose() => Show(loseSprite);

    public void HideImmediate()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        if (panel != null)
            panel.anchoredPosition = anchoredTargetPos + new Vector2(startOffsetX, 0f);

        if (resultImage != null)
            resultImage.enabled = false;
    }

    public void Show(Sprite sprite)
    {
        if (sprite == null)
            return;

        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(ShowRoutine(sprite));
    }

    IEnumerator ShowRoutine(Sprite sprite)
    {
        if (resultImage != null)
        {
            resultImage.sprite = sprite;
            resultImage.enabled = true;
            resultImage.SetNativeSize();
        }

        Vector2 startPos = anchoredTargetPos + new Vector2(startOffsetX, 0f);
        if (panel != null)
            panel.anchoredPosition = startPos;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        float t = 0f;
        while (t < slideDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / Mathf.Max(0.0001f, slideDuration));
            k = Smooth01(k);
            if (panel != null)
                panel.anchoredPosition = Vector2.LerpUnclamped(startPos, anchoredTargetPos, k);
            yield return null;
        }

        float f = 0f;
        while (f < fadeDuration)
        {
            f += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(f / Mathf.Max(0.0001f, fadeDuration));
            if (canvasGroup != null)
                canvasGroup.alpha = k;
            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        yield return new WaitForSecondsRealtime(holdDuration);
    }

    static float Smooth01(float x) => x * x * (3f - 2f * x);
}
