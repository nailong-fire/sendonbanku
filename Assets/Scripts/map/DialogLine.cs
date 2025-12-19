using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogLine
{
    public string speaker;     // 说话人名字
    [TextArea(2, 4)]
    public string content;     // 对话内容
}

