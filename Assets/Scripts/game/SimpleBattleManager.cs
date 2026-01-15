using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 一个用于快速验证“战斗胜/负 -> 返回主世界”的简易管理器。
/// 关键点：对外事件接口保持与现有 GameManager 一致：
/// - public System.Action<bool> OnGameOver;
/// - public System.Action<TurnPhase> OnPhaseChange;
/// - public static SimpleBattleManager Instance;
/// 
/// 之后把复杂战斗场景迁回时，只要继续触发 OnGameOver(bool) 即可。
/// </summary>
public class SimpleBattleManager : MonoBehaviour
{
    public enum TurnPhase
    {
        GameOver
    }

    [Header("Scene")]
    [Tooltip("战斗结束后要切回的主世界场景名（Build Settings 里要加入）")]
    public string worldSceneName = "map";

    [Header("Options")]
    [Tooltip("按钮点完是否立即切场景")]
    public bool loadWorldSceneOnGameOver = true;

    public bool isGameOver = false;

    private static SimpleBattleManager _instance;
    public static SimpleBattleManager Instance => _instance;

    public System.Action<TurnPhase> OnPhaseChange;
    public System.Action<bool> OnGameOver;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Win()
    {
        EndGame(true, "Button Win");
    }

    public void Lose()
    {
        EndGame(false, "Button Lose");
    }

    public void EndGame(bool playerWon, string reason)
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log($"[SimpleBattleManager] GameOver => {(playerWon ? "WIN" : "LOSE")}, reason: {reason}");

        OnPhaseChange?.Invoke(TurnPhase.GameOver);
        OnGameOver?.Invoke(playerWon);

        if (loadWorldSceneOnGameOver)
        {
            if (!string.IsNullOrWhiteSpace(worldSceneName))
            {
                if (GameState.Instance != null)
                {
                    if (playerWon)
                    {
                        GameState.Instance.story.battleWon = true;
                        GameState.Instance.story.battleLostOnce = false;
                    }
                    else
                    {
                        GameState.Instance.story.battleLostOnce = true;
                        GameState.Instance.story.battleWon = false;
                    }
                }

                SceneManager.LoadScene(worldSceneName);
            }
            else
            {
                Debug.LogWarning("[SimpleBattleManager] worldSceneName is empty, not loading scene.");
            }
        }
    }
}
