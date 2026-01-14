using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [Header("设置")]
    public string targetSceneName = "start"; // 目标场景名称
    public float fadeSpeed = 1.0f;            // 渐变速度

    private CanvasGroup canvasGroup;

    // 这个方法绑定到按钮点击事件
    public void FadeToStartScene()
    {
        // 关键一步：让挂载这个脚本的物体（MenuController）在切换场景时不被删除
        // 这样协程才能继续跑完淡入逻辑
        DontDestroyOnLoad(gameObject);
        
        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        // 1. 动态创建一个全屏的黑色遮罩 UI 
        // 这样你就不需要手动在每个场景里摆放黑图了
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000; // 确保在最顶层
        
        canvasGroup = canvasObj.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        
        GameObject imgObj = new GameObject("FadeImage");
        imgObj.transform.SetParent(canvasObj.transform);
        Image img = imgObj.AddComponent<Image>();
        img.color = Color.black; // 遮罩颜色
        
        RectTransform rt = img.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // 核心：在切换场景时不销毁这个遮罩，否则淡入效果会中断
        DontDestroyOnLoad(canvasObj);

        // 2. 开始淡出（屏幕慢慢变黑）
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // 3. 异步加载新场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("start");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 4. 开始淡入（在新场景中屏幕慢慢变亮）
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // 5. 完成后删除遮罩并销毁自身控制器组件，防止残留
        Destroy(canvasObj);
        Destroy(gameObject); 
    }
}