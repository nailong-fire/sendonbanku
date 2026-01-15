using UnityEngine;

/// <summary>
/// 回到主世界场景后，根据 StoryFlags 自动触发一次对应对话。
/// 设计为“挂在 map 场景里一个常驻/场景物体上”，在 Start 里只触发一次。
/// </summary>
public class BattleReturnDialogRouter : MonoBehaviour
{
    [Header("References")]
    [Tooltip("主世界里的 NPCInteract（村长）对象")]
    public NPCInteract npc;

    [Tooltip("可选：演出对象，如果要强制关闭演出，可在这里引用")]
    public MonoBehaviour performationToDisable;

    private void Start()
    {
        if (GameState.Instance == null || GameState.Instance.story == null)
        {
            Debug.LogWarning("[BattleReturnDialogRouter] GameState not found; skip routing.");
            return;
        }

        var story = GameState.Instance.story;

        // 仅在已经见面过的前提下，才走胜负回流对话
        if (!story.metVillageChief)
            return;

        // 如果你希望返回后不重复演出，这里也可以额外保险禁用
        if (performationToDisable != null)
            performationToDisable.enabled = false;

        if (npc == null)
        {
            Debug.LogWarning("[BattleReturnDialogRouter] npc reference is null.");
            return;
        }

        // 胜利优先
        if (story.battleWon)
        {
            Debug.Log("[BattleReturnDialogRouter] Routing dialog: BattleWinMain");
            npc.TriggerBattleWinMain();
        }
        else if (story.battleLostOnce)
        {
            Debug.Log("[BattleReturnDialogRouter] Routing dialog: BattleLose");
            npc.TriggerBattleLose();
        }
    }
}
