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

    private enum NPCStoryStage
    {
        None,
        FirstMeet,
        ReadyAsk,
        BattleLose,
        BattleWinMain
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
    // 玩家交互入口
    // =======================
    void OpenDialog()
    {
        if (dialogOpen)
            return;

        dialogOpen = true;

        if (playerMovement != null)
            playerMovement.EnableMove(false);

        if (talkHint != null)
            talkHint.SetActive(false);

        StartDialogByCurrentStage();
    }

    // =======================
    // 自动触发入口（给 Router 用）
    // =======================
    public void TriggerImmediateDialog()
    {
        if (dialogOpen)
            return;

        dialogOpen = true;

        if (playerMovement != null)
            playerMovement.EnableMove(false);

        if (talkHint != null)
            talkHint.SetActive(false);

        StartDialogByCurrentStage();
    }

    // =======================
    // 对外接口：战斗结果通知
    // =======================
    public void TriggerBattleWinMain()
    {
        if (GameState.Instance == null) return;

        GameState.Instance.story.battleWon = true;
        GameState.Instance.story.battleLostOnce = false;

        currentStage = NPCStoryStage.BattleWinMain;
    }

    public void TriggerBattleLose()
    {
        if (GameState.Instance == null) return;

        GameState.Instance.story.battleLostOnce = true;
        GameState.Instance.story.battleWon = false;

        currentStage = NPCStoryStage.BattleLose;
    }

    // =======================
    // 核心：根据当前状态选择对白
    // =======================
    void StartDialogByCurrentStage()
    {
        var story = GameState.Instance.story;

        // ① 战斗胜利（最高优先级）
        if (story.battleWon)
        {
            currentStage = NPCStoryStage.BattleWinMain;
            PlayDialog("BattleWinMain");
            return;
        }

        // ② 战斗失败
        if (story.battleLostOnce)
        {
            currentStage = NPCStoryStage.BattleLose;
            PlayDialog("BattleLose");
            return;
        }

        // ③ 第一次见面
        if (!story.metVillageChief)
        {
            story.metVillageChief = true;
            currentStage = NPCStoryStage.FirstMeet;
            PlayDialog("FirstMeet");
            return;
        }

        // ④ 默认询问是否准备好
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
        if (currentStage == NPCStoryStage.FirstMeet)
        {
            currentStage = NPCStoryStage.ReadyAsk;
            PlayDialog("ReadyAsk");
            return;
        }

        if (currentStage == NPCStoryStage.ReadyAsk)
        {
            dialogController.ShowChoices(
                "准备好了",
                OnChoiceReady,
                "还没准备好",
                OnChoiceNotReady
            );
        }
        else
        {
            FinishDialog();
        }
    }

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
