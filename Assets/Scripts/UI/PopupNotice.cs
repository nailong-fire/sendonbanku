using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupNotice : MonoBehaviour
{
    public static PopupNotice Instance { get; private set; }

    [Header("UI References")] // UI References
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;

    [Header("Animation")] // Animation
    [SerializeField] private float moveDuration = 0.22f;
    [SerializeField] private float fadeDuration = 0.18f;
    [SerializeField] private Vector2 hiddenOffset = new Vector2(0f, -520f);

    private Coroutine _animRoutine;
    private Coroutine _autoCloseRoutine;
    private Action _onConfirm;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        HideImmediate();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public static void Show(string message)
    {
        Show("Notice", message, null, 0f);
    }

    public static void Show(string title, string message, Action onConfirm, float autoCloseSeconds = 0f)
    {
        if (!EnsureInstance())
            return;

        Instance.ShowInternal(title, message, onConfirm, autoCloseSeconds);
    }

    private static bool EnsureInstance()
    {
        if (Instance != null)
            return true;

        Instance = FindObjectOfType<PopupNotice>();
        if (Instance != null)
            return true;

        Debug.LogWarning("PopupNotice: place one in the scene so Show() can work."); // There must be a PopupNotice instance in the scene
        return false;
    }

    private void ShowInternal(string title, string message, Action onConfirm, float autoCloseSeconds)
    {
        _onConfirm = onConfirm;

        if (titleText != null)
            titleText.text = title;
        if (bodyText != null)
            bodyText.text = message;

        if (_autoCloseRoutine != null)
            StopCoroutine(_autoCloseRoutine);
        if (_animRoutine != null)
            StopCoroutine(_animRoutine);

        WireButtons(autoCloseSeconds);
        _animRoutine = StartCoroutine(ShowRoutine(autoCloseSeconds));
    }

    private IEnumerator ShowRoutine(float autoCloseSeconds)
    {
        Vector2 targetPos = Vector2.zero;
        Vector2 startPos = targetPos + hiddenOffset;

        if (panel != null)
            panel.anchoredPosition = startPos;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / Mathf.Max(0.0001f, moveDuration));
            float eased = Smooth01(k);

            if (panel != null)
                panel.anchoredPosition = Vector2.LerpUnclamped(startPos, targetPos, eased);
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, k);

            yield return null;
        }

        if (panel != null)
            panel.anchoredPosition = targetPos;
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        if (autoCloseSeconds > 0f)
            _autoCloseRoutine = StartCoroutine(AutoClose(autoCloseSeconds));
    }

    private IEnumerator AutoClose(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Hide();
    }

    public void Hide()
    {
        if (_autoCloseRoutine != null)
        {
            StopCoroutine(_autoCloseRoutine);
            _autoCloseRoutine = null;
        }

        if (_animRoutine != null)
            StopCoroutine(_animRoutine);

        _animRoutine = StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        Vector2 startPos = Vector2.zero;
        Vector2 targetPos = startPos + hiddenOffset;

        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / Mathf.Max(0.0001f, moveDuration));
            float eased = Smooth01(k);

            if (panel != null)
                panel.anchoredPosition = Vector2.LerpUnclamped(startPos, targetPos, eased);
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, k);

            yield return null;
        }

        HideImmediate();
    }

    private void HideImmediate()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        if (panel != null)
            panel.anchoredPosition = Vector2.zero + hiddenOffset;
    }

    private void WireButtons(float autoCloseSeconds)
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                _onConfirm?.Invoke();
                Hide();
            });
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Hide);
        }
        else if (confirmButton == null && autoCloseSeconds <= 0f)
        {
            Debug.LogWarning("PopupNotice: no close button set; add one or enable auto close."); // No close button set
        }
    }

    private static float Smooth01(float x)
    {
        return x * x * (3f - 2f * x);
    }
}
