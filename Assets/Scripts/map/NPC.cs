using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class NPCInteract : MonoBehaviour
{
    [Header("References")]
    public DialogController dialogController;
    public PlayerMovement2D playerMovement;

    [Header("Dialog Data（可拖多个 DialogData.asset）")]
    public List<DialogData> dialogs;

    [Header("NPC Info")]
    public string npcName = "VillageChief";

    [Header("Battle")]
    public string battleSceneName = "cardbattle";

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    [Header("UI")]
    public GameObject talkHint;

    private bool playerInRange = false;
    private bool dialogOpen = false;

    /// <summary>
    /// NPC 当前所处的剧情阶段
    /// </summary>
    private enum NPCStoryStage
    {
        None,
        FirstMeet,          // 初见剧情
        ReadyAsk,           // 是否准备好
        BattleLose,         // 战斗失败后
        BattleWinMain,      // 战斗胜利后的主对话
        BattleWinChoice     // 战斗胜利后的可循环选项
    }

    private NPCStoryStage currentStage = NPCStoryStage.None;

    void Update()
    {
        if (playerInRange && !dialogOpen && Input.GetKeyDown(interactKey))
            OpenDialog();

        if (dialogOpen && Input.GetKeyDown(KeyCode.Escape))
            ForceCloseDialog();
    }

    // =======================
    // 对话入口
    // =======================
    void OpenDialog()
    {
        dialogOpen = true;

        if (playerMovement != null)
            playerMovement.EnableMove(false);

        if (talkHint != null)
            talkHint.SetActive(false);

        var story = GameState.Instance.story;

        // ① 第一次见面
        if (!story.metVillageChief)
        {
            story.metVillageChief = true;
            currentStage = NPCStoryStage.FirstMeet;
            PlayDialog("FirstMeet");
            return;
        }

        // ② 战斗胜利后
        if (story.battleWon)
        {
            currentStage = NPCStoryStage.BattleWinMain;
            PlayDialog("BattleWin_A");
            return;
        }

        // ③ 战斗失败后
        if (story.battleLostOnce)
        {
            currentStage = NPCStoryStage.BattleLose;
            PlayDialog("BattleLose");
            return;
        }

        // ④ 默认：询问是否准备好
        currentStage = NPCStoryStage.ReadyAsk;
        PlayDialog("ReadyAsk");
    }

    // =======================
    // 播放一段对白
    // =======================
    void PlayDialog(string dialogId)
    {
        DialogData dialog = GetDialog(dialogId);

        if (dialog == null)
        {
            Debug.LogError($"[NPCInteract] 找不到 DialogData: {dialogId}");
            FinishDialog();
            return;
        }

        dialogController.StartDialog(
            npcName,
            dialog.lines,
            OnDialogEnd
        );
    }

    // =======================
    // 对话结束后的流程推进
    // =======================
    void OnDialogEnd()
    {
        switch (currentStage)
        {
            // 初见剧情 → 接 ReadyAsk
            case NPCStoryStage.FirstMeet:
                currentStage = NPCStoryStage.ReadyAsk;
                PlayDialog("ReadyAsk");
                break;

            // ReadyAsk → 弹出“准备好了 / 未准备好”
            case NPCStoryStage.ReadyAsk:
                dialogController.ShowChoices(
                    "准备好了",
                    OnChoiceReady,
                    "还没准备好",
                    OnChoiceNotReady
                );
                break;

            // 战斗失败对白 → 结束即可
            case NPCStoryStage.BattleLose:
                FinishDialog();
                break;

            // 战斗胜利主对白 → 进入可循环选项
            case NPCStoryStage.BattleWinMain:
                currentStage = NPCStoryStage.BattleWinChoice;
                ShowWinChoices();
                break;
        }
    }

    // =======================
    // ReadyAsk 选项
    // =======================
    void OnChoiceReady()
    {
        GameState.Instance.story.readyForBattle = true;
        GameState.Instance.story.battleUnlocked = true;

        dialogController.EndDialog();
        SceneTransition.Instance.LoadScene(battleSceneName);
    }

    void OnChoiceNotReady()
    {
        dialogController.EndDialog();
        FinishDialog();
    }

    // =======================
    // 战斗胜利后的可循环选项
    // =======================
    void ShowWinChoices()
    {
        dialogController.ShowChoices(
            "询问过去",
            () => PlayWinSubDialog("AfterWin_Option1"),
            "询问真相",
            () => PlayWinSubDialog("AfterWin_Option2")
        );
    }

    void PlayWinSubDialog(string dialogId)
    {
        DialogData dialog = GetDialog(dialogId);
        if (dialog == null)
        {
            Debug.LogError($"[NPCInteract] 找不到 DialogData: {dialogId}");
            FinishDialog();
            return;
        }

        dialogController.StartDialog(
            npcName,
            dialog.lines,
            ShowWinChoices   // ⭐ 结束后回到选项
        );
    }

    // =======================
    // 强制结束 / 清理
    // =======================
    void ForceCloseDialog()
    {
        dialogController.EndDialog();
        FinishDialog();
    }

    void FinishDialog()
    {
        dialogOpen = false;
        currentStage = NPCStoryStage.None;

        if (playerMovement != null)
            playerMovement.EnableMove(true);
    }

    // =======================
    // 触发器
    // =======================
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (talkHint != null)
                talkHint.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (talkHint != null)
                talkHint.SetActive(false);
        }
    }

    // =======================
    // 工具：按 dialogId 查找对白
    // =======================
    DialogData GetDialog(string id)
    {
        foreach (var d in dialogs)
        {
            if (d != null && d.dialogId == id)
                return d;
        }
        return null;
    }
}
