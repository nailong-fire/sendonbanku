using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif

public class enddialog1 : MonoBehaviour
{
    [Header("References")]
    public DialogController dialogController;
    public DialogLine[] dialogLines;
    public string npcName = "unknow";
    public GameObject nextdialog = null;

    [Header("Audio")]
    public AudioClip rockCollapseSound; // 拖入 Audio/rock-break-hard-184891.mp3
    [Tooltip("声音播放后等待多久才开始对话")]
    public float delayBeforeDialog = 2.0f; 

    void Start()
    {
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        Debug.Log("[enddialog1] 序列开始");

        // 强制关闭可能意外开启的第二段对话
        if (nextdialog != null && nextdialog.activeSelf)
        {
            Debug.LogWarning("[enddialog1] 检测到 nextdialog (enddialog2) 已经在运行，正在强制关闭它以保证顺序。");
            nextdialog.SetActive(false);
        }

        // 1. 先播放音效
        if (rockCollapseSound != null)
        {
            AudioSource.PlayClipAtPoint(rockCollapseSound, Camera.main != null ? Camera.main.transform.position : transform.position);
            
            // 2. 等待一段时间，让声音先飞一会儿
            yield return new WaitForSeconds(delayBeforeDialog);
        }

        // 3. 声音播了一会儿后，再弹出对话框
        Debug.Log("[enddialog1] 启动对话");
        dialogController.StartDialog(
                npcName,       
                dialogLines,
                OnDialogEnd    
            );
    }
    void OnDialogEnd()
    {
        Debug.Log("[enddialog1] 对话结束，准备播放动画");

        if (dialogController != null)
            dialogController.EndDialog();

        StartCoroutine(PlayAnim());
    }

    IEnumerator PlayAnim()
    {
        // 播放动画
        Stage6AnimTrigger.playani();

        // 等待动画时长
        // 使用 Realtime 以防对话系统修改了 TimeScale
        Debug.Log("[enddialog1] 等待动画播放 (10.5秒)...");
        yield return new WaitForSecondsRealtime(10.5f);
       
       if (nextdialog != null)
       {
            Debug.Log("[enddialog1] 激活下一段对话");
            nextdialog.SetActive(true);
       } 

        gameObject.SetActive(false);
    }
}
