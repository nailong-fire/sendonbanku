// === 7. 游戏管理器 ===
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public Player PlayerController;
    public Player EnemyController;
    public GameBoard Board;
    public TurnSystem TurnSystem;
    
    private bool _gameOver = false;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start()
    {
        InitializeGame();
        TurnSystem.StartPlayerTurn();
    }
    
    private void InitializeGame()
    {
        // 初始化资源
        PlayerController.Resources.CurrentHope = PlayerController.Resources.MaxHope;
        EnemyController.Resources.CurrentHope = EnemyController.Resources.MaxHope;
        
        // 订阅事件
        PlayerController.Resources.OnHopeChanged += OnPlayerHopeChanged;
        EnemyController.Resources.OnHopeChanged += OnEnemyHopeChanged;
        
        TurnSystem.OnTurnChanged += OnTurnChanged;
    }
    
    private void OnPlayerHopeChanged(int hope)
    {
        if (hope <= 0)
            GameOver(false); // 玩家失败
    }
    
    private void OnEnemyHopeChanged(int hope)
    {
        if (hope <= 0)
            GameOver(true); // 玩家胜利
    }
    
    private void OnTurnChanged(bool isPlayerTurn)
    {
        if (isPlayerTurn)
        {
            // 玩家回合开始
            PlayerController.EndTurn();
        }
        else
        {
            // 敌人回合开始
            EnemyController.EndTurn();
        }
    }
    
    private void GameOver(bool playerWon)
    {
        if (_gameOver) return;
        
        _gameOver = true;
        Debug.Log($"Game Over! Player {(playerWon ? "Won" : "Lost")}");
        
        // 显示游戏结束UI等
    }
}