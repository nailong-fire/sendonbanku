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

    public bool battleWon = false;        // ⭐ 战斗是否胜利过
    public bool battleLostOnce = false;   // ⭐ 是否至少失败过一次
}
