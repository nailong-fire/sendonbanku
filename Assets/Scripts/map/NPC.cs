using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class NPCInteract : MonoBehaviour
{
    [Header("References")]
    public DialogController dialogController;
    public PlayerMovement2D playerMovement;

    [Header("Dialog Data (可拖多个)")]
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
        if (talkHint != null) talkHint.SetActive(false);

        bool isFirstMeet = !GameState.Instance.story.metVillageChief;

        if (isFirstMeet)
        {
            currentStage = DialogStage.FirstStory;
            GameState.Instance.story.metVillageChief = true;

            var dialog = GetDialog("FirstMeet");
            dialogController.StartDialog(npcName, dialog.lines, OnDialogEnd);
        }
        else
        {
            currentStage = DialogStage.ReadyAsk;

            var dialog = GetDialog("ReadyAsk");
            dialogController.StartDialog(npcName, dialog.lines, OnDialogEnd);
        }
    }

    void OnDialogEnd()
    {
        if (currentStage == DialogStage.FirstStory)
        {
            currentStage = DialogStage.ReadyAsk;
            var dialog = GetDialog("ReadyAsk");
            dialogController.StartDialog(npcName, dialog.lines, OnDialogEnd);
            return;
        }

        if (currentStage == DialogStage.ReadyAsk)
        {
            dialogController.ShowChoices(
                "准备好了",
                OnChoiceReady,
                "还没准备好",
                OnChoiceNotReady
            );
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
