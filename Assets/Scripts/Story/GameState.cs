using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    public StoryFlags story = new StoryFlags();

    void Awake()
    {
        // 单例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ⭐ 每次启动游戏时，重置剧情
        // ResetStory();
    }

    public void ResetStory()
    {
        story.metVillageChief = false;
        story.readyForBattle = false;
        story.battleUnlocked = false;

        story.battleWon = false;
        story.battleLostOnce = false;

        Debug.Log("Story reset: new game started");
    }
}
