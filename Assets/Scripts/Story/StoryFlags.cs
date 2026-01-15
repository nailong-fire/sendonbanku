using System;
using UnityEngine;

[Serializable]
public class StoryFlags
{
    [Header("Village Chief")]
    public bool metVillageChief = false;

    [Header("Battle Progress")]
    public bool readyForBattle = false;
    public bool battleUnlocked = false;

    public bool battleWon = false;        // ⭐ 战斗是否胜利过（针对最近一次战斗）
    public bool battleLostOnce = false;   // ⭐ 是否至少失败过一次（针对最近一次战斗）

    [Header("Battle Return Routing")]
    [Tooltip("最近一次战斗对应的 NPC 标识（用于回到地图后路由到正确 NPC）")]
    public string lastBattleNpcId = "";
}
