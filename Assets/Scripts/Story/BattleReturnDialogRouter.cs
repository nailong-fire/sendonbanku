using UnityEngine;

/// <summary>
/// 回到主世界场景后，根据 StoryFlags 自动触发一次对应对话。
/// </summary>
public class BattleReturnDialogRouter : MonoBehaviour
{
    [Header("References")]
    [Tooltip("主世界里的 NPCInteract（村长）")]
    public NPCInteract npc;

    private bool triggered = false;

    private void Start()
    {
        if (triggered)
            return;

        if (GameState.Instance == null || GameState.Instance.story == null)
            return;

        if (npc == null)
        {
            Debug.LogWarning("[BattleReturnDialogRouter] npc is null");
            return;
        }

        var story = GameState.Instance.story;

        // 必须是已经见过 NPC 才触发
        if (!story.metVillageChief)
            return;

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
