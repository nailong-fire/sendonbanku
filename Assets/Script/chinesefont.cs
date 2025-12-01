using TMPro;
using UnityEngine;

public class ChineseTextExample : MonoBehaviour
{
    public TMP_Text textComponent;

    void Start()
    {
        // 方法A：编辑器拖拽赋值
        // 在Inspector中直接拖拽字体资产到Font Asset字段

        // 方法B：代码动态设置
        TMP_FontAsset chineseFont = Resources.Load<TMP_FontAsset>("Fonts/Traditional");
        if (chineseFont != null)
        {
            textComponent.font = chineseFont;
        }

        // 设置汉字文本
        textComponent.text = "这是一段测试汉字";

        // 可选：调整字体样式
        textComponent.fontSize = 24;
        textComponent.color = Color.black;
        textComponent.alignment = TextAlignmentOptions.Center;
    }
}