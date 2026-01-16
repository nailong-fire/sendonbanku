using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class PopupNoticeCreator
{
    [MenuItem("GameObject/UI/Create Popup Notice (Sendonbanku)", false, 10)]
    static void CreatePopup(MenuCommand menuCommand)
    {
        // 1. 寻找合适的父节点 (Canvas)
        GameObject parent = menuCommand.context as GameObject;
        if (parent == null || parent.GetComponentInParent<Canvas>() == null)
        {
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas != null) 
            {
                parent = canvas.gameObject;
            }
            else
            {
               // 如果场景里没有 Canvas，尝试创建一个
               EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
               parent = Selection.activeGameObject;
            }
        }

        // 2. 创建根节点 "PopupNotice_Root"
        GameObject root = new GameObject("PopupNotice_Root");
        GameObjectUtility.SetParentAndAlign(root, parent);
        
        // 设置全屏填充
        RectTransform rootRT = root.AddComponent<RectTransform>();
        rootRT.anchorMin = Vector2.zero;
        rootRT.anchorMax = Vector2.one;
        rootRT.offsetMin = Vector2.zero;
        rootRT.offsetMax = Vector2.zero;

        // 添加 CanvasGroup
        CanvasGroup cg = root.AddComponent<CanvasGroup>();
        
        // 添加 PopupNotice 核心脚本
        PopupNotice noticeScript = root.AddComponent<PopupNotice>();

        // 添加半透明背景 (Blocker)
        Image blockerImg = root.AddComponent<Image>();
        blockerImg.color = new Color(0, 0, 0, 0.5f); // 半透明黑色
        blockerImg.raycastTarget = true; // 阻挡点击

        // 3. 创建中间的面板 "MainPanel"
        GameObject panel = new GameObject("MainPanel");
        panel.transform.SetParent(root.transform, false);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = Color.white; // 简单的白色背景
        
        RectTransform panelRT = panel.GetComponent<RectTransform>();
        panelRT.sizeDelta = new Vector2(600, 350);
        panelRT.anchoredPosition = Vector2.zero;

        // 4. 创建标题 "TitleText"
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panel.transform, false);
        TextMeshProUGUI titleTxt = titleObj.AddComponent<TextMeshProUGUI>();
        titleTxt.text = "reminder";
        titleTxt.fontSize = 36;
        titleTxt.alignment = TextAlignmentOptions.Center;
        titleTxt.fontStyle = FontStyles.Bold;
        titleTxt.color = Color.black;
        
        RectTransform titleRT = titleObj.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0, 1);
        titleRT.anchorMax = new Vector2(1, 1);
        titleRT.pivot = new Vector2(0.5f, 1);
        titleRT.anchoredPosition = new Vector2(0, -25);
        titleRT.sizeDelta = new Vector2(0, 50);

        // 5. 创建正文 "BodyText"
        GameObject bodyObj = new GameObject("BodyText");
        bodyObj.transform.SetParent(panel.transform, false);
        TextMeshProUGUI bodyTxt = bodyObj.AddComponent<TextMeshProUGUI>();
        bodyTxt.text = "This is a popup notice message.";
        bodyTxt.fontSize = 24;
        bodyTxt.alignment = TextAlignmentOptions.TopLeft;
        bodyTxt.color = new Color(0.2f, 0.2f, 0.2f);
        
        RectTransform bodyRT = bodyObj.GetComponent<RectTransform>();
        bodyRT.anchorMin = Vector2.zero;
        bodyRT.anchorMax = Vector2.one;
        bodyRT.offsetMin = new Vector2(40, 80); // Left, Bottom
        bodyRT.offsetMax = new Vector2(-40, -80); // Right, Top

        // 6. 创建关闭按钮 (右上角 X)
        GameObject closeBtnObj = CreateButton("CloseButton", "X", panel.transform);
        RectTransform closeRT = closeBtnObj.GetComponent<RectTransform>();
        closeRT.anchorMin = Vector2.one;
        closeRT.anchorMax = Vector2.one;
        closeRT.anchoredPosition = new Vector2(-25, -25);
        closeRT.sizeDelta = new Vector2(40, 40);
        
        // 调整关闭按钮文字大小
        closeBtnObj.GetComponentInChildren<TextMeshProUGUI>().fontSize = 20;

        // 7. 创建确认按钮 (底部居中)
        GameObject confirmBtnObj = CreateButton("ConfirmButton", "confirm", panel.transform);
        RectTransform confirmRT = confirmBtnObj.GetComponent<RectTransform>();
        confirmRT.anchorMin = new Vector2(0.5f, 0);
        confirmRT.anchorMax = new Vector2(0.5f, 0);
        confirmRT.pivot = new Vector2(0.5f, 0);
        confirmRT.anchoredPosition = new Vector2(0, 25);
        confirmRT.sizeDelta = new Vector2(140, 45);

        // 8. 自动关联脚本引用 (使用 SerializedObject)
        SerializedObject so = new SerializedObject(noticeScript);
        so.Update();
        
        so.FindProperty("canvasGroup").objectReferenceValue = cg;
        so.FindProperty("panel").objectReferenceValue = panelRT;
        so.FindProperty("titleText").objectReferenceValue = titleTxt;
        so.FindProperty("bodyText").objectReferenceValue = bodyTxt;
        so.FindProperty("confirmButton").objectReferenceValue = confirmBtnObj.GetComponent<Button>();
        so.FindProperty("closeButton").objectReferenceValue = closeBtnObj.GetComponent<Button>();
        
        so.ApplyModifiedProperties();

        // 9. 完成
        Undo.RegisterCreatedObjectUndo(root, "Create Popup Notice");
        Selection.activeObject = root;
        
        Debug.Log("PopupNotice UI has been successfully created and references have been automatically assigned!");
    }

    static GameObject CreateButton(string name, string text, Transform parent)
    {
        // 创建标准按钮结构
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.9f, 0.9f, 0.9f); // 浅灰背景
        
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;
        
        // 按钮文字
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.black;
        
        // 文字填满按钮
        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        
        return btnObj;
    }
}
