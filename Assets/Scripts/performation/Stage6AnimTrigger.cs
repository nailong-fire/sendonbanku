using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage6AnimTrigger : MonoBehaviour
{
    private static Stage6AnimTrigger instance;

    [Header("Configuration")]
    public Animator animator;
    public string animationStateName = "perform"; // 默认填入刚才看到的动画名

    private bool _hasTriggered = false;

    private void Awake()
    {
        // 简单的单例设置
        instance = this;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        // 如果还没有被代码触发过，进场时强制暂停在第一帧
        if (!_hasTriggered && animator != null)
        {
            Debug.Log("[Stage6AnimTrigger] 初始化：暂停动画在第一帧等待触发");
            animator.Play(animationStateName, 0, 0f); // 定位到开头
            animator.speed = 0f; // 速度设为0（暂停）
        }
    }

    // 用户要求的静态触发函数
    public static void playani()
    {
        // 尝试自动查找（防呆设计）
        if (instance == null)
        {
            instance = FindObjectOfType<Stage6AnimTrigger>(true); // true 表示包括未激活的物体
        }

        if (instance != null)
        {
            if (!instance.gameObject.activeSelf)
            {
                Debug.Log("[Stage6AnimTrigger] 发现物体处于隐藏状态，正在自动激活...");
                instance.gameObject.SetActive(true);
            }

            if (instance.animator != null)
            {
                instance._hasTriggered = true; // 标记为已触发
                
                Debug.Log($"[Stage6AnimTrigger] 正在尝试播放动画状态: {instance.animationStateName}");
                
                // 恢复播放速度（之前被Start设为0了）
                instance.animator.speed = 1f;

                // 强制从头开始播放
                instance.animator.Play(instance.animationStateName, -1, 0f);
            }
            else
            {
                Debug.LogError("[Stage6AnimTrigger] 错误：找到了脚本但找不到 Animator 组件！");
            }
        }
        else
        {
            Debug.LogError("[Stage6AnimTrigger] 严重错误：场景里完全找不到挂载了 Stage6AnimTrigger 的物体！请创建一个空物体并挂载此脚本。");
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
