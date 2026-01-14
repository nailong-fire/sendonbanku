using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCInteract2D : MonoBehaviour
{
    [Header("References")]
    public DialogController dialogController;
    public PlayerMovement2D playerMovement;

    [Header("Dialog")]
    public DialogLine[] dialogLines;

    [Header("NPC Info")]
    public string npcName = "unknown";

    [Header("Battle Settings")]
    public bool showBattleChoice = true;
    public string battleSceneName = "cardbattle";

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;
    private bool dialogOpen = false;
    private bool ismoving = false;

    void Update()
    {
        // 按 E 开始对话
        if (playerInRange && !dialogOpen && Input.GetKeyDown(interactKey))
        {
            OpenDialog();
        }
    }

    void FixedUpdate()
    {
        if(ismoving == true)
        {
            if (playerMovement != null)
                playerMovement.EnableMove(false);
            
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y +0.025f, gameObject.transform.position.z); 

            if(gameObject.transform.position.y >= 0.25f)
            {
                ismoving = false;
                if (playerMovement != null)
                    playerMovement.EnableMove(true);
            }
        }
    }

    void OpenDialog()
    {
        dialogOpen = true;

        // 冻结玩家
        if (playerMovement != null)
            playerMovement.EnableMove(false);

        // 开始对话
        if (dialogController != null)
        {
            dialogController.StartDialog(
                npcName,
                dialogLines,
                OnDialogEnd
            );
        }
    }

    // 对话文本全部播完后调用
    void OnDialogEnd()
    {
        ismoving = true;
        
        if (dialogController != null)
            dialogController.EndDialog();
        FinishDialog();
        /*if (showBattleChoice && dialogController != null)
        {
            dialogController.ShowChoices(
                "准备好了",
                OnChoiceReady,
                "还需要准备",
                OnChoiceNotReady
            );
        }
        else
        {
            if (dialogController != null)
                dialogController.EndDialog();
            FinishDialog();
        }*/
    }

    // 选择“准备好了”
    void OnChoiceReady()
    {
        if (dialogController != null)
            dialogController.EndDialog();

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayBattleMusic();

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene(battleSceneName);
        else
            SceneManager.LoadScene(battleSceneName);
    }

    // 选择“还需要准备”
    void OnChoiceNotReady()
    {
        if (dialogController != null)
            dialogController.EndDialog();

        FinishDialog();
    }

    void FinishDialog()
    {
        dialogOpen = false;

        if (playerMovement != null)
            playerMovement.EnableMove(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
