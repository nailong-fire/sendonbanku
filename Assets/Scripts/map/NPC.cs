using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCInteract : MonoBehaviour
{
    [Header("References")]
    public DialogController dialogController;   // 对话控制器
    public PlayerMovement playerMovement;       // 玩家移动脚本

    [Header("Dialog")]
    public DialogLine[] dialogLines;            // 这个 NPC 的对话内容

    [Header("NPC Info")]
    public string npcName = "老村长";

    [Header("Battle Settings")]
    public bool showBattleChoice = true;        // 对话结束后是否显示战斗选项
    public string battleSceneName = "BattleScene";  // 战斗场景名称

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;
    private bool dialogOpen = false;

    void Update()
    {
        // 按 E 开始对话
        if (playerInRange && !dialogOpen && Input.GetKeyDown(interactKey))
        {
            OpenDialog();
        }

        // ESC 任何时候都能强制退出
        if (dialogOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ForceCloseDialog();
        }
    }

    // ESC 强制退出
    void ForceCloseDialog()
    {
        if (dialogController != null)
            dialogController.EndDialog();
        
        FinishDialog();
    }

    void OpenDialog()
    {
        dialogOpen = true;

        // 冻结玩家
        if (playerMovement != null)
            playerMovement.EnableMove(false);

        // 开始对话，并注册"结束回调"
        if (dialogController != null)
        {
            dialogController.StartDialog(
                npcName,       // 保留作为默认名字（如果 DialogLine 没填 speaker 可以用）
                dialogLines,
                OnDialogEnd    // 关键：统一出口
            );
        }
    }

    // 对话文本全部播完后调用
    void OnDialogEnd()
    {
        // 如果需要显示战斗选项
        if (showBattleChoice && dialogController != null)
        {
            // 对话框保持打开，显示选项按钮
            dialogController.ShowChoices(
                "准备好了",          // 选项1文字
                OnChoiceReady,       // 选项1回调
                "还需要准备",        // 选项2文字
                OnChoiceNotReady     // 选项2回调
            );
        }
        else
        {
            // 没有选项，直接关闭对话
            if (dialogController != null)
                dialogController.EndDialog();
            FinishDialog();
        }
    }

    // 选择"准备好了" → 进入战斗场景
    void OnChoiceReady()
    {
        // 关闭对话UI
        if (dialogController != null)
            dialogController.EndDialog();

        // 切换到战斗场景
        SceneManager.LoadScene(battleSceneName, LoadSceneMode.Single);
    }

    // 选择"还需要准备" → 结束对话，恢复移动
    void OnChoiceNotReady()
    {
        // 关闭对话UI
        if (dialogController != null)
            dialogController.EndDialog();
        
        FinishDialog();
    }

    // 真正结束对话的逻辑
    void FinishDialog()
    {
        dialogOpen = false;

        if (playerMovement != null)
            playerMovement.EnableMove(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
