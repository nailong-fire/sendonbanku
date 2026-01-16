using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif


public class mapdialog1 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CardDatabaseSO playercardDatabase;
    public DialogController dialogController;
    public DialogLine[] dialogLines;
    public string npcName = "leader";
    public GameObject nextdialog = null;

    void Start()
    {
        // ⭐ 已经见过 leader，就不再播放开场对话
        if (GameState.Instance != null &&
            GameState.Instance.story != null &&
            GameState.Instance.story.metVillageChief)
        {
            gameObject.SetActive(false);
            return;
        }

        dialogController.StartDialog(
            npcName,
            dialogLines,
            OnDialogEnd
        );
    }

    void OnDialogEnd()
    {
        if (dialogController != null)
            dialogController.EndDialog();

        GameObject leader = GameObject.Find("leader");

        leader.transform.position = new Vector3(-205.0f, 0.25f, leader.transform.position.z);

        playercardDatabase.AddCardToPlayerOwnedPile("001", 5);
        playercardDatabase.AddCardToPlayerOwnedPile("002", 3);
        playercardDatabase.AddCardToPlayerOwnedPile("010", 2);
        
        if (nextdialog != null)
            nextdialog.SetActive(true);

        gameObject.SetActive(false);
    }
}
