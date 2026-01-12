using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;


public class startdialogtwo : MonoBehaviour
{
    [Header("References")]
    public DialogController dialogController;
    public DialogLine[] dialogLines;
    public string npcName = "friend";
    void Start()
    {
        dialogController.StartDialog(
                npcName,       // 保留作为默认名字（如果 DialogLine 没填 speaker 可以用）
                dialogLines,
                OnDialogEnd    // 关键：统一出口
            );
        
    }
    void OnDialogEnd()
    {
        if (dialogController != null)
            dialogController.EndDialog();
        
        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.LoadScene("test");
        }
        else
        {
            // 如果没有过渡管理器，直接切换
            SceneManager.LoadScene("test");
        }
    }
}
