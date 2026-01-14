using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "New Dialog",
    menuName = "Dialog/Dialog Data"
)]
public class DialogData : ScriptableObject
{
    [Header("NPC 名字")]
    public string npcName;

    [Header("对白内容")]
    [TextArea(2, 4)]
    public string[] dialogLines;
}
