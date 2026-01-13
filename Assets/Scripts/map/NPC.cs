using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCInteract : MonoBehaviour
{
    [Header("References")]
    public DialogController dialogController;
    public PlayerMovement playerMovement;

    [Header("Dialog Sets")]
    public DialogLine[] firstMeetDialog;   // 第一次：背景故事
    public DialogLine[] readyDialog;       // 准备询问：“你准备好了吗？”

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

    // ⭐ 用来区分：这次结束回调是“长剧情结束”还是“准备询问结束”
    private enum DialogStage { None, FirstStory, ReadyAsk }
    private DialogStage currentStage = DialogStage.None;

    void Update()
    {
        if (playerInRange && !dialogOpen && Input.GetKeyDown(interactKey))
            OpenDialog();

        if (dialogOpen && Input.GetKeyDown(KeyCode.Escape))
            ForceCloseDialog();
    }

    void OpenDialog()
    {
        dialogOpen = true;
        if (playerMovement != null) playerMovement.EnableMove(false);

        if (talkHint != null)
            talkHint.SetActive(false);

        bool isFirstMeet = !GameState.Instance.story.metVillageChief;

        if (isFirstMeet)
        {
            // 第一次：先播长剧情
            currentStage = DialogStage.FirstStory;

            // ⭐ 第一次打开就标记（避免任何情况下重复）
            GameState.Instance.story.metVillageChief = true;

            dialogController.StartDialog(npcName, firstMeetDialog, OnDialogEnd);
        }
        else
        {
            // 非第一次：直接播“准备好了吗？”
            currentStage = DialogStage.ReadyAsk;
            dialogController.StartDialog(npcName, readyDialog, OnDialogEnd);
        }
    }

    void OnDialogEnd()
    {
        // ⭐ 如果刚播完的是“第一次长剧情”，那就立刻接上 readyDialog
        if (currentStage == DialogStage.FirstStory)
        {
            currentStage = DialogStage.ReadyAsk;
            dialogController.StartDialog(npcName, readyDialog, OnDialogEnd);
            return;
        }

        // ⭐ 播完 readyDialog 后才弹选项
        if (currentStage == DialogStage.ReadyAsk)
        {
            dialogController.ShowChoices(
                "准备好了",
                OnChoiceReady,
                "还没准备好",
                OnChoiceNotReady
            );
            return;
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
        currentStage = DialogStage.None;
        if (playerMovement != null) playerMovement.EnableMove(true);
    }

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
}
