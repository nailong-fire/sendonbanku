using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景过渡管理器 - 淡入淡出效果
/// 使用方法：SceneTransition.Instance.LoadScene("场景名");
/// </summary>
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [Header("Settings")]
    public float fadeDuration = 0.5f;       // 淡入淡出时长
    public Color fadeColor = Color.black;   // 过渡颜色（黑色/白色）

    private GameObject player;
    private Vector3 playerPosition = new Vector3(0, -1, 0);
    private Vector3 cameraPosition = new Vector3(0, 0, 0);
    private Image fadeImage;
    private Canvas fadeCanvas;
    private bool isTransitioning = false;

    void Awake()
    {
        // 单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 创建过渡用的 UI
        CreateFadeUI();
    }

    void CreateFadeUI()
    {
        // 创建 Canvas
        GameObject canvasObj = new GameObject("FadeCanvas");
        canvasObj.transform.SetParent(transform);
        fadeCanvas = canvasObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 9999; // 确保在最上层

        // 添加 CanvasScaler
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // 创建全屏 Image
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform);
        fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
        fadeImage.raycastTarget = false;

        // 设置为全屏
        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// 带过渡效果加载场景
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionToScene(sceneName));
        }
    }

    /// <summary>
    /// 带过渡效果加载场景（通过索引）
    /// </summary>
    public void LoadScene(int sceneIndex)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionToSceneByIndex(sceneIndex));
        }
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        isTransitioning = true;

        if(sceneName == "Tmp Battle")
        {
            player = GameObject.FindWithTag("Player");
            playerPosition = player.transform.position;
            cameraPosition = Camera.main.transform.position;
        }

        // 淡出（画面变黑）
        yield return StartCoroutine(Fade(0, 1));

        // ⭐ 在黑屏时切BGM（听感最好）
        if (MusicManager.Instance != null)
        {
            if (sceneName == "Tmp Battle")
                MusicManager.Instance.PlayBattleMusic();
            else
                MusicManager.Instance.PlayMapMusic();
        }

        // 加载新场景
        SceneManager.LoadScene(sceneName);

        // 等待一帧确保场景加载完成
        yield return null;

        if(sceneName != "Tmp Battle" && playerPosition != new Vector3(0, -1, 0))
        {
            // 将玩家传送回原位置
            player = GameObject.FindWithTag("Player");
            Camera.main.transform.position = cameraPosition;
            player.transform.position = playerPosition;
        }

        // 淡入（画面恢复）
        yield return StartCoroutine(Fade(1, 0));

        isTransitioning = false;
    }

    IEnumerator TransitionToSceneByIndex(int sceneIndex)
    {
        isTransitioning = true;

        yield return StartCoroutine(Fade(0, 1));
        SceneManager.LoadScene(sceneIndex);
        yield return null;
        yield return StartCoroutine(Fade(1, 0));

        isTransitioning = false;
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            
            // 使用平滑插值
            t = t * t * (3f - 2f * t); // SmoothStep
            
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }

    /// <summary>
    /// 只执行淡出效果（不切换场景）
    /// </summary>
    public void FadeOut(System.Action onComplete = null)
    {
        StartCoroutine(FadeOutCoroutine(onComplete));
    }

    IEnumerator FadeOutCoroutine(System.Action onComplete)
    {
        yield return StartCoroutine(Fade(0, 1));
        onComplete?.Invoke();
    }

    /// <summary>
    /// 只执行淡入效果
    /// </summary>
    public void FadeIn(System.Action onComplete = null)
    {
        StartCoroutine(FadeInCoroutine(onComplete));
    }

    IEnumerator FadeInCoroutine(System.Action onComplete)
    {
        yield return StartCoroutine(Fade(1, 0));
        onComplete?.Invoke();
    }
}
