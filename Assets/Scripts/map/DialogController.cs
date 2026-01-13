using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogController : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialogUI;
    public TMP_Text nameText;
    public TMP_Text dialogText;
    public GameObject nextArrow;

    [Header("Choice Buttons")]
    public GameObject choicePanel;          // 选项面板（包含两个按钮）
    public Button choiceButton1;            // 第一个选项按钮
    public Button choiceButton2;            // 第二个选项按钮
    public TMP_Text choiceText1;            // 第一个选项文字
    public TMP_Text choiceText2;            // 第二个选项文字

    [Header("Typing")]
    public float typingSpeed = 0.04f;

    private DialogLine[] lines;
    private int index;
    private bool isTyping;
    private bool dialogActive;
    private bool waitingForChoice = false;  // 是否正在等待玩家选择

    public Action onDialogEnd;
    private Action onChoice1;
    private Action onChoice2;

    void Update()
    {
        if (!dialogActive) return;
        if (waitingForChoice) return;
        if (lines == null || lines.Length == 0) return;

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

            if (nextArrow != null)
                nextArrow.SetActive(true);
        }
        else
        {
            NextLine();
        }
    }

    public void StartDialog(string speaker, DialogLine[] dialogLines, Action onEnd = null)
    {
        if (dialogUI == null || nameText == null || dialogText == null)
        {
            Debug.LogError("DialogController：UI 引用未绑定");
            return;
        }

        dialogUI.SetActive(true);
        dialogActive = true;
        waitingForChoice = false;

        dialogText.text = "";

        lines = dialogLines;
        index = 0;
        isTyping = false;
        onDialogEnd = onEnd;

        if (nextArrow != null)
            nextArrow.SetActive(false);

        if (choicePanel != null)
            choicePanel.SetActive(false);

        StartCoroutine(DelayedShowFirstLine());
    }

    IEnumerator DelayedShowFirstLine()
    {
        yield return null;
        ShowCurrentLine();
    }

    void NextLine()
    {
        index++;

        if (nextArrow != null)
            nextArrow.SetActive(false);

        if (index < lines.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            OnAllLinesFinished();
        }
    }

    void OnAllLinesFinished()
    {
        onDialogEnd?.Invoke();
    }

    void ShowCurrentLine()
    {
        nameText.text = lines[index].speaker;
        StopAllCoroutines();
        StartCoroutine(TypeLine(lines[index].content));
    }

    IEnumerator TypeLine(string content)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char c in content)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        if (nextArrow != null)
            nextArrow.SetActive(true);
    }

    public void EndDialog()
    {
        StopAllCoroutines();
        dialogActive = false;
        waitingForChoice = false;
        isTyping = false;

        if (dialogUI != null)
            dialogUI.SetActive(false);

        if (choicePanel != null)
            choicePanel.SetActive(false);

        if (nextArrow != null)
            nextArrow.SetActive(false);
    }

    // 显示两个选项
    public void ShowChoices(string text1, Action callback1, string text2, Action callback2)
    {
        waitingForChoice = true;

        if (nextArrow != null)
            nextArrow.SetActive(false);

        if (choiceText1 != null)
            choiceText1.text = text1;
        if (choiceText2 != null)
            choiceText2.text = text2;

        onChoice1 = callback1;
        onChoice2 = callback2;

        if (choiceButton1 != null)
        {
            choiceButton1.onClick.RemoveAllListeners();
            choiceButton1.onClick.AddListener(OnClickChoice1);
        }
        if (choiceButton2 != null)
        {
            choiceButton2.onClick.RemoveAllListeners();
            choiceButton2.onClick.AddListener(OnClickChoice2);
        }

        if (choicePanel != null)
            choicePanel.SetActive(true);
    }

    void OnClickChoice1()
    {
        HideChoices();
        onChoice1?.Invoke();
    }

    void OnClickChoice2()
    {
        HideChoices();
        onChoice2?.Invoke();
    }

    void HideChoices()
    {
        waitingForChoice = false;
        if (choicePanel != null)
            choicePanel.SetActive(false);
    }
}
