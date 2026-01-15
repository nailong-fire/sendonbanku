using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage6AnimTrigger : MonoBehaviour
{
    private static Stage6AnimTrigger instance;

    [Header("Configuration")]
    public Animator animator;
    public string animationStateName = "perform"; // 默认填入刚才看到的动画名

    private void Awake()
    {
        // 简单的单例设置
        instance = this;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    // 用户要求的静态触发函数
    public static void playani()
    {
        if (instance != null && instance.animator != null)
        {
            // 使用 Play 直接播放指定状态的动画
            // 如果你的 Animator 是通过 Parameter 触发的，可以将这里改为 instance.animator.SetTrigger(...)
            instance.animator.Play(instance.animationStateName);
        }
        else
        {
            Debug.LogWarning("Stage6AnimTrigger: 无法播放动画，请检查脚本是否挂载以及 Animator 是否赋值。");
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
