using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 回到主世界场景后，根据 StoryFlags 自动触发一次对应对话。
/// </summary>
public class BattleReturnDialogRouter : MonoBehaviour
{
    [Header("References")]
    [Tooltip("主世界可触发对话/战斗的 NPC 列表")]
    public List<NPCInteract> npcs = new List<NPCInteract>();

    [Header("Timing")]
    [Tooltip("回到地图后的淡入淡出时间，避免对话立刻弹出")]
    public float triggerDelay = 0.6f;

    private bool triggered = false;

    private void Start()
    {
        if (triggered)
            return;

        StartCoroutine(TriggerAfterDelay());
    }

    private System.Collections.IEnumerator TriggerAfterDelay()
    {
        if (triggerDelay > 0f)
            yield return new WaitForSeconds(triggerDelay);

        if (triggered)
            yield break;

        if (GameState.Instance == null || GameState.Instance.story == null)
            yield break;

        if (npcs == null || npcs.Count == 0)
            npcs = new List<NPCInteract>(FindObjectsOfType<NPCInteract>());

        if (npcs == null || npcs.Count == 0)
        {
            Debug.LogWarning("[BattleReturnDialogRouter] npcs is empty");
            yield break;
        }

        var story = GameState.Instance.story;

        // 必须要有战斗来源 NPC，才能精确路由
        var targetNpcId = story.lastBattleNpcId;
        if (string.IsNullOrEmpty(targetNpcId))
        {
            Debug.LogWarning("[BattleReturnDialogRouter] lastBattleNpcId is empty; skip auto dialog");
            yield break;
        }

        NPCInteract target = null;
        foreach (var candidate in npcs)
        {
            if (candidate == null) continue;
            if (candidate.npcId == targetNpcId)
            {
                target = candidate;
                break;
            }
        }

        if (target == null)
        {
            Debug.LogWarning($"[BattleReturnDialogRouter] can't find npc with id: {targetNpcId}");
            yield break;
        }

        if (story.battleWon)
        {
            target.TriggerBattleWinMain();
            target.TriggerImmediateDialog();
            triggered = true;
        }
        else if (story.battleLostOnce)
        {
            target.TriggerBattleLose();
            target.TriggerImmediateDialog();
            triggered = true;
        }
    }
}
