using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor;

public class NPCInteract : MonoBehaviour
{
    [Header("References")]
    public DialogController dialogController;
    public Map.PlayerAnimController playerMovement;

    [Header("NPC Info")]
    public string npcName = "unknow";
    public string option1 = null;
    public string option2 = null;
    public string leave = null;
    public CardDatabaseSO enemyCardDatabase;
    public string card1;
    public string card2;

    [Header("Dialog Data")]
    public List<DialogData> dialogs;

    [Tooltip("用于战斗返回后定位到这个 NPC。建议每个 NPC 唯一")]
    public string npcId = "VillageChief";

    [Header("Battle")]
    public string battleSceneName = "cardbattle";

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    [Header("UI")]
    public GameObject talkHint;

    private bool playerInRange;
    private bool dialogOpen;
    private float originalY;
    private bool ismoving = false;

    private enum NPCStoryStage
    {
        None,
        FirstMeet,
        ReadyAsk,
        BattleLose,
        BattleWinMain,
        BattleWinMenu,
        BattleWinSubDialog,
        TheEnd
    }

    private NPCStoryStage currentStage = NPCStoryStage.None;

    void Start()
    {
        if (playerMovement == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerMovement = playerObj.GetComponent<Map.PlayerAnimController>();
            }
        }
    }

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
        if (dialogId == "BattleWin_Option2" && npcName == "leader")
            currentStage = NPCStoryStage.TheEnd;
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
                    "Ready",
                    OnChoiceReady,
                    "Not Ready",
                    OnChoiceNotReady
                );
                break;

            case NPCStoryStage.BattleLose:
                // 播放完失败对白：立刻关闭对话框；等玩家再次靠近并按 E 才进入 ReadyAsk
                GameState.Instance.story.battleLostOnce = false;
                dialogController.EndDialog();
                FinishDialog();
                break;

            case NPCStoryStage.BattleWinMain:
                currentStage = NPCStoryStage.BattleWinMenu;
                ShowBattleWinMenu(option1, option2, leave);
                break;

            case NPCStoryStage.BattleWinSubDialog:
                currentStage = NPCStoryStage.BattleWinMenu;
                ShowBattleWinMenu(option1, option2, leave);
                break;
            
            case NPCStoryStage.TheEnd:
                performending();
                dialogController.EndDialog();
                FinishDialog();
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
        GameState.Instance.story.lastBattleNpcId = npcId;
        EnemyImformation.instance.enemyName = npcName;
        EnemyImformation.instance.enemyCardDatabase = enemyCardDatabase;
        Debug.Log("[NPCInteract] 玩家准备好战斗，记录战斗来源 NPC ID: " + npcId);

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
    void ShowBattleWinMenu(string option1 = "option1", string option2 = "option2", string leave = "leave")
    {
        dialogController.ShowChoices(
            option1,
            () => EnterBattleWinSubDialog("BattleWin_Option1"),

            option2,
            () => EnterBattleWinSubDialog("BattleWin_Option2"),

            leave,
            () => EndBattleWinConversation()
        );
    }

    void FixedUpdate()
    {
        if(ismoving == true)
        {
            if (playerMovement != null)
                playerMovement.EnableMove(false);

            if(gameObject.transform.position.y - originalY >= 0.25f || gameObject.transform.position.y >= 0.25f)
            {
                ismoving = false;
                if (playerMovement != null)
                    playerMovement.EnableMove(true);
            }
            if (ismoving == true)
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y +0.025f, gameObject.transform.position.z); 
        }
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
        originalY = gameObject.transform.position.y;
        ismoving = true;

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

    private void performending()
    {
        EndTransition.Instance.LoadScene();
    }
}
