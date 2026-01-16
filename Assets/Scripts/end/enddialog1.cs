using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class enddialog1 : MonoBehaviour
{
    [Header("References")]
    public DialogController dialogController;
    public DialogLine[] dialogLines;
    public string npcName = "unknow";
    public GameObject nextdialog = null;

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

        StartCoroutine(PlayAnim());
    }

    IEnumerator PlayAnim()
    {
       yield return new WaitForSeconds(1f);
       if (nextdialog != null)
            nextdialog.SetActive(true);

        gameObject.SetActive(false);
    }
}
