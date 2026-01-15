using UnityEngine;

/// <summary>
/// 回到主世界场景后，根据 StoryFlags 自动触发一次对应对话。
/// </summary>
public class BattleReturnDialogRouter : MonoBehaviour
{
    [Header("References")]
    [Tooltip("主世界里的 NPCInteract（村长）")]
    public NPCInteract npc;

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

        if (npc == null)
        {
            Debug.LogWarning("[BattleReturnDialogRouter] npc is null");
            yield break;
        }

        var story = GameState.Instance.story;

        // 必须是已经见过 NPC 才触发
        if (!story.metVillageChief)
            yield break;

        if (story.battleWon)
        {
            npc.TriggerBattleWinMain();
            npc.TriggerImmediateDialog();
            triggered = true;
        }
        else if (story.battleLostOnce)
        {
            npc.TriggerBattleLose();
            npc.TriggerImmediateDialog();
            triggered = true;
        }
    }
}
