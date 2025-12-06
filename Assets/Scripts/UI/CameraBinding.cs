using UnityEngine;

public class CanvasCameraBinder : MonoBehaviour
{
    [Header("自动绑定选项")]
    public bool autoBindMainCamera = true;
    public bool bindOnAwake = true;
    public bool bindOnStart = true;
    
    private Canvas canvas;
    
    void Awake()
    {
        canvas = GetComponent<Canvas>();
        
        if (bindOnAwake)
        {
            BindCamera();
        }
    }
    
    void Start()
    {
        if (bindOnStart)
        {
            BindCamera();
        }
    }
    
    void BindCamera()
    {
        if (canvas == null || !canvas.isActiveAndEnabled) 
            return;
            
        // 只在Screen Space - Camera模式下需要绑定
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera || 
            canvas.renderMode == RenderMode.WorldSpace)
        {
            if (canvas.worldCamera == null && autoBindMainCamera)
            {
                canvas.worldCamera = Camera.main;
                Debug.Log($"已绑定主摄像机到Canvas: {gameObject.name}");
            }
        }
    }
    
    // 手动调用绑定
    public void BindToCamera(Camera targetCamera = null)
    {
        if (canvas == null) return;
        
        canvas.worldCamera = targetCamera != null ? targetCamera : Camera.main;
    }
    
    // 清空绑定
    public void UnbindCamera()
    {
        if (canvas != null)
        {
            canvas.worldCamera = null;
        }
    }
}