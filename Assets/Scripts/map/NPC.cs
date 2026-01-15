using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class NPCInteract : MonoBehaviour
{
    [Header("References")]
    public DialogController dialogController;
    public PlayerMovement2D playerMovement;

    [Header("Dialog Data")]
    public List<DialogData> dialogs;

    [Header("NPC Info")]
    public string npcName = "VillageChief";

    [Header("Battle")]
    public string battleSceneName = "cardbattle";

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    [Header("UI")]
    public GameObject talkHint;

    private bool playerInRange;
    private bool dialogOpen;

    private enum NPCStoryStage
    {
        None,
        FirstMeet,
        ReadyAsk,
        BattleLose,
        BattleWinMain,
        BattleWinMenu,
        BattleWinSubDialog
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
    // 入口
    // =======================
    void OpenDialog()
    {
        if (dialogOpen) return;

        BeginDialog();
        StartDialogByCurrentStage();
    }

    public void TriggerImmediateDialog()
    {
        if (dialogOpen) return;

        BeginDialog();
        StartDialogByCurrentStage();
    }

    void BeginDialog()
    {
        dialogOpen = true;

        if (playerMovement != null)
            playerMovement.EnableMove(false);

        if (talkHint != null)
            talkHint.SetActive(false);
    }

    // =======================
    // 战斗结果通知（Router 调）
    // =======================
    public void TriggerBattleWinMain()
    {
        GameState.Instance.story.battleWon = true;
        GameState.Instance.story.battleLostOnce = false;
        currentStage = NPCStoryStage.BattleWinMain;
    }

    public void TriggerBattleLose()
    {
        GameState.Instance.story.battleLostOnce = true;
        GameState.Instance.story.battleWon = false;
        currentStage = NPCStoryStage.BattleLose;
    }

    // =======================
    // 根据状态选对白
    // =======================
    void StartDialogByCurrentStage()
    {
        var story = GameState.Instance.story;

        if (story.battleWon)
        {
            currentStage = NPCStoryStage.BattleWinMain;
            PlayDialog("BattleWinMain");
            return;
        }

        if (story.battleLostOnce)
        {
            currentStage = NPCStoryStage.BattleLose;
            PlayDialog("BattleLose");
            return;
        }

        if (!story.metVillageChief)
        {
            story.metVillageChief = true;
            currentStage = NPCStoryStage.FirstMeet;
            PlayDialog("FirstMeet");
            return;
        }

        currentStage = NPCStoryStage.ReadyAsk;
        PlayDialog("ReadyAsk");
    }

    // =======================
    // 播放对白
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

        dialogController.StartDialog(npcName, dialog.lines, OnDialogEnd);
    }

    // =======================
    // 对话结束推进
    // =======================
    void OnDialogEnd()
    {
        switch (currentStage)
        {
            case NPCStoryStage.FirstMeet:
                currentStage = NPCStoryStage.ReadyAsk;
                PlayDialog("ReadyAsk");
                break;

            case NPCStoryStage.ReadyAsk:
                dialogController.ShowChoices(
                    "准备好了",
                    OnChoiceReady,
                    "还没准备好",
                    OnChoiceNotReady
                );
                break;

            case NPCStoryStage.BattleWinMain:
                currentStage = NPCStoryStage.BattleWinMenu;
                ShowBattleWinMenu();
                break;

            case NPCStoryStage.BattleWinSubDialog:
                currentStage = NPCStoryStage.BattleWinMenu;
                ShowBattleWinMenu();
                break;

            default:
                FinishDialog();
                break;
        }
    }

    // =======================
    // ReadyAsk
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
    // 战斗胜利后的固定 3 按钮
    // =======================
    void ShowBattleWinMenu()
    {
        dialogController.ShowChoices(
            "询问过去",
            () => EnterBattleWinSubDialog("BattleWin_Option1"),

            "询问真相",
            () => EnterBattleWinSubDialog("BattleWin_Option2"),

            "离开",
            EndBattleWinConversation
        );
    }

    void EnterBattleWinSubDialog(string dialogId)
    {
        currentStage = NPCStoryStage.BattleWinSubDialog;
        PlayDialog(dialogId);
    }

    void EndBattleWinConversation()
    {
        // NPC 让路示例
        // GetComponent<Collider2D>().enabled = false;

        dialogController.EndDialog();
        FinishDialog();
    }

    // =======================
    // 清理
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
