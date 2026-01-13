using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleChoiceUI : MonoBehaviour
{
    public DialogController dialogController;

    public void OnReadyClicked()
    {
        GameState.Instance.story.readyForBattle = true;
        GameState.Instance.story.battleUnlocked = true;

        dialogController.EndDialog();
        SceneManager.LoadScene("cardbattle");
    }

    public void OnNotReadyClicked()
    {
        dialogController.EndDialog();
    }
}

