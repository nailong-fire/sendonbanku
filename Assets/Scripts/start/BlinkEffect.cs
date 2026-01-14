using UnityEngine;

public class BlinkEffect : MonoBehaviour
{
    [Header("闪烁设置")]
    [Tooltip("闪烁速度，数值越大闪得越快")]
    public float speed = 2.0f; 
    
    [Tooltip("最小透明度 (0-1)，0为完全透明，1为不透明")]
    [Range(0f, 1f)]
    public float minAlpha = 0.5f;

    [Tooltip("最大透明度 (0-1)")]
    [Range(0f, 1f)]
    public float maxAlpha = 1.0f;

    private CanvasGroup canvasGroup;

    void Start()
    {
        // 尝试获取 CanvasGroup 组件，如果没有则自动添加
        // CanvasGroup 是控制 UI 整体透明度（包括子物体如文字）的最佳方式
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Update()
    {
        if (canvasGroup != null)
        {
            // 使用 PingPong 函数实现数值的往复运动
            // 结果会在 minAlpha 到 maxAlpha 之间平滑过渡
            float length = maxAlpha - minAlpha;
            float currentAlpha = minAlpha + Mathf.PingPong(Time.time * speed, length);
            
            canvasGroup.alpha = currentAlpha;
        }
    }
}
