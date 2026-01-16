using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    [System.Serializable]
    public class GameSettings
    {
        [Header("游戏设置")]
        public string gameName = "云冈卡牌对战";
        
        [Header("玩家初始设置")]
        public int playerStartingHope = 8;
        public int playerStartingFaith = 2;
        public int playerStartingHandSize = 3;
        public int playerMaxHandSize = 5;
        
        [Header("敌人初始设置")]
        public int enemyStartingHope = 8;
        public int enemyStartingFaith = 2;
        public int enemyStartingHandSize = 3;
        public int enemyMaxHandSize = 5;
        
    }
    
    [Header("游戏设置")]
    public GameSettings settings = new GameSettings();
    
    [Header("预制体引用")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject cardPrefab;
    //public GameObject battlefieldZonePrefab;
    //public GameObject handZonePrefab;
    
    [Header("数据引用")]
    public CardDatabaseSO cardDatabase;
    
    [Header("场景引用")]
    public Camera mainCamera;
    public Transform gameBoardParent;
    public GameObject turnIndicatorPrefab;
    
    [Header("UI引用")]
    public Canvas gameCanvas;
    public GameObject loadingScreen;
    
    // 游戏实例
    private PlayerController _player;
    private EnemyController _enemy;
    private CardZone _playerBattlefield;
    private CardZone _playerHandZone;
    private CardZone _enemyBattlefield;
    private CardZone _enemyHandZone;
    
    // 单例
    private static GameInitializer _instance;
    public static GameInitializer Instance => _instance;
    
    // 游戏状态
    public bool IsInitialized { get; private set; }
    public bool IsGameStarted = false;
    public PlayerController Player => _player;
    public EnemyController Enemy => _enemy;
    
    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "cardbattle")
        {
            gameObject.SetActive(false);
            return;
        }

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
    
    private void Start()
    {
        // 启动游戏初始化
        StartCoroutine(InitializeGameCoroutine());
    }
    
    private IEnumerator InitializeGameCoroutine()
    {
        Debug.Log("开始游戏初始化...");
        
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        
        // 分步骤初始化
        yield return StartCoroutine(Step1_InitializeCoreSystems());
        yield return StartCoroutine(Step2_CreateGameBoard());
        yield return StartCoroutine(Step3_CreateCharacters());
        yield return StartCoroutine(Step4_SetupCameraAndUI());
        yield return StartCoroutine(Step5_FinalSetup());
        
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
        
        Debug.Log("游戏初始化完成！");
        IsInitialized = true;
        
        // 触发游戏开始事件
        OnGameInitialized?.Invoke();
    }
    
    // 步骤1：初始化核心系统
    private IEnumerator Step1_InitializeCoreSystems()
    {
        Debug.Log("步骤1：初始化核心系统...");
        
        // 确保必要组件存在
        EnsureEssentialComponents();
        
        // 初始化卡牌工厂
        InitializeCardFactory();
        
        yield return null;
    }
    
    // 步骤2：创建游戏棋盘
    private IEnumerator Step2_CreateGameBoard()
    {
        Debug.Log("步骤2：创建游戏棋盘...");
        
        yield return null;
    }
    
    // 步骤3：创建角色
    private IEnumerator Step3_CreateCharacters()
    {
        Debug.Log("步骤3：创建角色...");
        
        // 创建玩家
        if (playerPrefab != null)
        {
            GameObject playerObj = Instantiate(playerPrefab, gameBoardParent);
            playerObj.name = "Player";
            _player = playerObj.GetComponent<PlayerController>();

            _playerBattlefield = _player.battlefield;
            _playerHandZone = _player.handZone;
            
            if (_player == null)
            {
                Debug.LogError("玩家预制体上没有PlayerController组件！");
                yield break;
            }
            _player.Initialize(name: "玩家", isPlayer: true, startingHope: settings.playerStartingHope, startingFaith: settings.playerStartingFaith);
            Debug.Log($"玩家初始faith: {settings.playerStartingFaith}, hope: {settings.playerStartingHope}");

            _player.InitializeUI();
        }
        else
        {
            Debug.LogError("玩家预制体未设置！");
            yield break;
        }
        
        // 创建敌人
        if (enemyPrefab != null)
        {
            GameObject enemyObj = Instantiate(enemyPrefab, gameBoardParent);
            enemyObj.name = "Enemy";
            _enemy = enemyObj.GetComponent<EnemyController>();

            _enemyBattlefield = _enemy.battlefield;
            _enemyHandZone = _enemy.handZone;
            
            if (_enemy == null)
            {
                Debug.LogError("敌人预制体上没有EnemyController组件！");
                yield break;
            }
            _enemy.cardDatabase = EnemyImformation.instance.enemyCardDatabase;
            Debug.Log($"敌人卡牌数据库已设置为 {EnemyImformation.instance.enemyCardDatabase}");
            _enemy.Initialize(name: "敌人", isPlayer: false, startingHope: settings.enemyStartingHope, startingFaith: settings.enemyStartingFaith);
            Debug.Log($"敌人初始faith: {settings.enemyStartingFaith}, hope: {settings.enemyStartingHope}");

            _enemy.InitializeUI();
        }
        else
        {
            Debug.LogError("敌人预制体未设置！");
            yield break;
        }
        
        yield return null;
    }
    
    private IEnumerator InitializeDeckAndHand()
    {
        Debug.Log("备份卡组和分发手牌...");
        
        // 初始化玩家
        if (_player != null)
        {
            // 设置玩家区域引用
            //_player.handZone = _playerHandZone;
            //_player.battlefield = _playerBattlefield;
            
            
            // 初始化玩家资源
            //_player.Initialize(name: "玩家", isPlayer: true, startingHope: settings.playerStartingHope, startingFaith: settings.playerStartingFaith);
            
            // 抽初始手牌
            yield return StartCoroutine(DrawStartingHand(
                _player, 
                settings.playerStartingHandSize
            ));
        }

        yield return 0.1f;
        
        // 初始化敌人
        if (_enemy != null)
        {
            // 设置敌人区域引用
            //_enemy.handZone = _enemyHandZone;
            //_enemy.battlefield = _enemyBattlefield;
            
            // 初始化敌人资源
            //_enemy.Initialize(name: "敌人", isPlayer: false, startingHope: settings.enemyStartingHope, startingFaith: settings.enemyStartingFaith);
            
            // 抽初始手牌
            yield return StartCoroutine(DrawStartingHand(
                _enemy, 
                settings.enemyStartingHandSize
            ));
        }

        GameManager.Instance.StartGame();

        yield return null;
    }
    
    // 步骤5：设置相机和UI
    private IEnumerator Step4_SetupCameraAndUI()
    {
        Debug.Log("步骤4：设置相机和UI...");
        
        // 设置相机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        yield return null;
    }
    
    // 步骤6：最终设置
    private IEnumerator Step5_FinalSetup()
    {
        Debug.Log("步骤5：最终设置...");
        
        // 打印初始化信息
        PrintInitializationInfo();
        
        yield return new WaitForSeconds(0.5f);
        
        // 游戏准备就绪，可以开始回合
        Debug.Log("=== 游戏准备就绪 ===");
        Debug.Log("输入 'SPACE' 开始玩家回合");
        Debug.Log("输入 'R' 重新开始游戏");
    }
    
    // 创建战场区域
    //private CardZone CreateBattlefieldZone(string name, Vector3 position, bool isPlayerSide)
    //{
    //}
    
    // 创建手牌区域
    //private CardZone CreateHandZone(string name, Vector3 position, int maxCards)
    //{
    //}
    
    // 抽初始手牌
    private IEnumerator DrawStartingHand(UniversalController character, int handSize)
    {
        if (character == null) yield break;
        
        for (int i = 0; i < handSize; i++)
        {
            CardEntity drawnCard = character.DrawCard();
            if (drawnCard != null)
            {
                // 稍微延迟，让抽牌有视觉效果
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                Debug.LogWarning($"无法为 {character.characterName} 抽牌");
                break;
            }
        }
        
        Debug.Log($"{character.characterName} 初始手牌: {character.GetHandCount()} 张");
    }
    
    
    // 确保必要组件存在
    private void EnsureEssentialComponents()
    {
        // 确保有游戏棋盘父对象
        if (gameBoardParent == null)
        {
            gameBoardParent = new GameObject("GameBoard").transform;
        }
        
        // 确保有卡牌数据库
        if (cardDatabase == null)
        {
            cardDatabase = Resources.Load<CardDatabaseSO>("CardDatabase");
            if (cardDatabase == null)
            {
                Debug.LogError("无法找到卡牌数据库！");
            }
        }
    }
    
    // 初始化卡牌工厂
    private void InitializeCardFactory()
    {
        // 检查是否已有CardFactory
        CardFactory existingFactory = FindObjectOfType<CardFactory>();
        if (existingFactory == null)
        {
            // 创建卡牌工厂
            GameObject factoryObj = new GameObject("CardFactory");
            CardFactory cardFactory = factoryObj.AddComponent<CardFactory>();
            
            // 设置卡牌预制体
            if (cardPrefab != null)
            {
                // 这里需要根据你的CardFactory实现来设置
                // 假设CardFactory有一个cardPrefab字段
                var field = cardFactory.GetType().GetField("cardPrefab");
                if (field != null)
                {
                    field.SetValue(cardFactory, cardPrefab);
                }
            }
            
            // 设置卡牌数据库
            var databaseField = cardFactory.GetType().GetField("cardDatabase");
            if (databaseField != null && cardDatabase != null)
            {
                databaseField.SetValue(cardFactory, cardDatabase);
            }
        }
    }
    
    // 连接游戏管理器
    private void ConnectGameManager()
    {
        // 查找或创建游戏管理器
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManager = gameManagerObj.AddComponent<GameManager>();
        }
        
        // 设置游戏管理器引用
        gameManager.SetPlayers(_player, _enemy);
        gameManager.settings = settings;
    }
    
    // 打印初始化信息
    private void PrintInitializationInfo()
    {
        Debug.Log("=== 初始化信息 ===");
        Debug.Log($"玩家: {(_player != null ? "✓" : "✗")}");
        Debug.Log($"敌人: {(_enemy != null ? "✓" : "✗")}");
        Debug.Log($"玩家战场: {(_playerBattlefield != null ? "✓" : "✗")}");
        Debug.Log($"玩家手牌区域: {(_playerHandZone != null ? "✓" : "✗")}");
        Debug.Log($"敌人战场: {(_enemyBattlefield != null ? "✓" : "✗")}");
        Debug.Log($"敌人手牌区域: {(_enemyHandZone != null ? "✓" : "✗")}");
        Debug.Log($"玩家卡组: {(_player != null ? _player.GetDeckCount() : 0)} 张");
        Debug.Log($"玩家手牌: {(_player != null ? _player.GetHandCount() : 0)} 张");
        Debug.Log($"敌人卡组: {(_enemy != null ? _enemy.GetDeckCount() : 0)} 张");
        Debug.Log($"敌人手牌: {(_enemy != null ? _enemy.GetHandCount() : 0)} 张");
    }
    
    // 重新开始游戏
    public void RestartGame()
    {
        Debug.Log("重新开始游戏...");
        
        // 销毁现有对象
        DestroyExistingObjects();
        
        // 重新初始化
        StartCoroutine(InitializeGameCoroutine());
    }
    
    // 销毁现有对象
    public void DestroyExistingObjects()
    {
        if (_player != null) Destroy(_player.gameObject);
        if (_enemy != null) Destroy(_enemy.gameObject);
        if (_playerBattlefield != null) 
        {
            foreach(CardEntity card in _playerBattlefield.GetAllCards())
            Destroy(card.gameObject);
            Destroy(_playerBattlefield.gameObject);
        }
        if (_playerHandZone != null) 
        {
            foreach(CardEntity card in _playerHandZone.GetAllCards())
            Destroy(card.gameObject);
            Destroy(_playerHandZone.gameObject);
        }
        if (_enemyBattlefield != null) 
        {
            foreach(CardEntity card in _enemyBattlefield.GetAllCards())
            Destroy(card.gameObject);
            Destroy(_enemyBattlefield.gameObject);
        }
        if (_enemyHandZone != null) 
        {
            foreach(CardEntity card in _enemyHandZone.GetAllCards())
            Destroy(card.gameObject);
            Destroy(_enemyHandZone.gameObject);
        }
        
        _player = null;
        _enemy = null;
        _playerBattlefield = null;
        _playerHandZone = null;
        _enemyBattlefield = null;
        _enemyHandZone = null;
        
        IsInitialized = false;
    }
    
    
    // 事件：游戏初始化完成
    public event System.Action OnGameInitialized;
    
    private void Update()
    {
        // 快捷键
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsInitialized && !IsGameStarted)
            {
                ConnectGameManager();
                StartCoroutine(InitializeDeckAndHand());
                ConnectGameManager();
                IsGameStarted = true;
            }
            else 
            {
                Debug.Log("游戏未初始化或已开始");
            }
        }
    }
}