using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewDialog",
    menuName = "Dialog/Dialog Data"
)]
public class DialogData : ScriptableObject
{
    [Header("对白标识（逻辑用）")]
    public string dialogId;

    [Header("对白内容（逐句）")]
    public DialogLine[] lines;
}

