using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogController : MonoBehaviour
{
    [Header("Dialog UI")]
    public GameObject dialogUI;
    public TMP_Text nameText;
    public TMP_Text dialogText;
    public GameObject nextArrow;

    [Header("Choice UI")]
    public GameObject choicePanel;

    public Button choiceButton1;
    public Button choiceButton2;
    public Button choiceButton3;

    public TMP_Text choiceText1;
    public TMP_Text choiceText2;
    public TMP_Text choiceText3;

    [Header("Typing")]
    public float typingSpeed = 0.04f;

    private DialogLine[] lines;
    private int index;
    private bool isTyping;
    private bool dialogActive;
    private bool waitingForChoice;

    private Action onChoice1;
    private Action onChoice2;
    private Action onChoice3;

    private Action onDialogEnd;

    void Update()
    {
        if (!dialogActive || waitingForChoice) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogText.text = lines[index].content;
            isTyping = false;
            nextArrow.SetActive(true);
        }
        else
        {
            NextLine();
        }
    }

    public void StartDialog(string speaker, DialogLine[] dialogLines, Action onEnd = null)
    {
        dialogUI.SetActive(true);
        dialogActive = true;
        waitingForChoice = false;

        lines = dialogLines;
        index = 0;
        onDialogEnd = onEnd;

        nextArrow.SetActive(false);
        choicePanel.SetActive(false);

        ShowLine();
    }

    void NextLine()
    {
        index++;
        nextArrow.SetActive(false);

        if (index < lines.Length)
        {
            ShowLine();
        }
        else
        {
            onDialogEnd?.Invoke();
        }
    }

    void ShowLine()
    {
        nameText.text = lines[index].speaker;
        StopAllCoroutines();
        StartCoroutine(TypeLine(lines[index].content));
    }

    System.Collections.IEnumerator TypeLine(string content)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char c in content)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        nextArrow.SetActive(true);
    }

    public void EndDialog()
    {
        dialogActive = false;
        waitingForChoice = false;
        dialogUI.SetActive(false);
        choicePanel.SetActive(false);
    }

    // =========================
    // 2 个按钮（ReadyAsk）
    // =========================
    public void ShowChoices(
        string text1, Action callback1,
        string text2, Action callback2)
    {
        waitingForChoice = true;
        nextArrow.SetActive(false);

        choicePanel.SetActive(true);

        choiceButton1.gameObject.SetActive(true);
        choiceButton2.gameObject.SetActive(true);
        choiceButton3.gameObject.SetActive(false);

        choiceText1.text = text1;
        choiceText2.text = text2;

        choiceButton1.onClick.RemoveAllListeners();
        choiceButton2.onClick.RemoveAllListeners();

        choiceButton1.onClick.AddListener(() => { CloseChoice(); callback1?.Invoke(); });
        choiceButton2.onClick.AddListener(() => { CloseChoice(); callback2?.Invoke(); });
    }

    // =========================
    // 3 个按钮（战斗胜利）
    // =========================
    public void ShowChoices(
        string text1, Action callback1,
        string text2, Action callback2,
        string text3, Action callback3)
    {
        waitingForChoice = true;
        nextArrow.SetActive(false);

        choicePanel.SetActive(true);

        choiceButton1.gameObject.SetActive(true);
        choiceButton2.gameObject.SetActive(true);
        choiceButton3.gameObject.SetActive(true);

        choiceText1.text = text1;
        choiceText2.text = text2;
        choiceText3.text = text3;

        choiceButton1.onClick.RemoveAllListeners();
        choiceButton2.onClick.RemoveAllListeners();
        choiceButton3.onClick.RemoveAllListeners();

        choiceButton1.onClick.AddListener(() => { CloseChoice(); callback1?.Invoke(); });
        choiceButton2.onClick.AddListener(() => { CloseChoice(); callback2?.Invoke(); });
        choiceButton3.onClick.AddListener(() => { CloseChoice(); callback3?.Invoke(); });
    }

    void CloseChoice()
    {
        waitingForChoice = false;
        choicePanel.SetActive(false);
    }
}
