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
    private Action onChoice1;               // 选项1的回调
    private Action onChoice2;               // 选项2的回调

    void Update()
    {
        if (!dialogActive) return;
        if (waitingForChoice) return;  // 等待选择时不响应点击
        if (lines == null || lines.Length == 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        if (isTyping)
        {
            // 直接显示完整一句
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
        // ===== 安全检查 =====
        if (dialogUI == null || nameText == null || dialogText == null)
        {
            Debug.LogError("DialogController：UI 引用未绑定");
            return;
        }

        dialogUI.SetActive(true);
        dialogActive = true;

        dialogText.text = "";

        lines = dialogLines;
        index = 0;
        isTyping = false;
        onDialogEnd = onEnd;

        if (nextArrow != null)
            nextArrow.SetActive(false);
        
        if (choicePanel != null)
            choicePanel.SetActive(false);

        // 延迟一帧再显示，确保UI已激活
        StartCoroutine(DelayedShowFirstLine());
    }

    IEnumerator DelayedShowFirstLine()
    {
        yield return null;  // 等待一帧
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
            // 对话内容全部说完，通知外部
            OnAllLinesFinished();
        }
    }

    // 对话内容全部说完时调用（不关闭UI，让外部决定下一步）
    void OnAllLinesFinished()
    {
        // 通知外部：对话文本全部播完
        onDialogEnd?.Invoke();
    }

    // 显示当前对话行（更新说话人名字 + 开始打字）
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

    // 强制关闭对话（ESC 或选择后调用）
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

    // ⭐ 显示选项按钮（对话UI保持打开）
    public void ShowChoices(string text1, Action callback1, string text2, Action callback2)
    {
        waitingForChoice = true;

        // 隐藏继续箭头
        if (nextArrow != null)
            nextArrow.SetActive(false);

        // 隐藏对话文本（可选，如果你想保留最后一句可以注释掉）
        // dialogText.text = "";
        // nameText.text = "";

        // 设置按钮文字
        if (choiceText1 != null)
            choiceText1.text = text1;
        if (choiceText2 != null)
            choiceText2.text = text2;

        // 保存回调
        onChoice1 = callback1;
        onChoice2 = callback2;

        // 绑定按钮事件
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

        // 显示选项面板
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
