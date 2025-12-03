# PlayerPrefab 搭建教程

## 1. 现有 PlayerPrefab 分析

### 1.1 当前结构
目前的 `playerprefab.prefab` 仅包含最基础的组件：
- **GameObject**: 设置了名称、标签("Player")和激活状态
- **Transform**: 设置了基础变换属性(位置、旋转、缩放)

### 1.2 不足之处
1. 缺少游戏核心数据管理组件(Hope血量、Faith资源) - 注意：玩家的Hope是血量，卡牌的Health是卡牌血量
2. 没有手牌和场上卡牌的容器结构
3. 缺少UI元素用于显示玩家状态
4. 缺乏玩家控制器和游戏逻辑脚本

## 2. PlayerPrefab 完整结构设计

### 2.1 层级结构设计
```
PlayerPrefab
├── PlayerCore         # 核心组件容器
├── HandCards          # 手牌区域
├── FieldCards         # 场上卡牌区域
└── UI                 # UI元素容器
    ├── StatusPanel    # 状态面板
    └── ResourceBar    # 资源条
```

### 2.2 设计要点
1. **模块化结构**: 清晰分离不同功能模块
2. **扩展性**: 便于后续添加新功能
3. **可维护性**: 合理的组件组织和命名
4. **性能优化**: 避免不必要的组件和复杂计算

## 3. 所需组件和脚本规划

### 3.1 核心组件
- **PlayerController.cs**: 玩家核心控制器，管理玩家状态和行为
- **PlayerData.cs**: 玩家数据模型，存储Hope血量(玩家血量)、Faith等数据
- **CardZone.cs**: 卡牌区域管理脚本(手牌/场上) - 卡牌的Health是卡牌血量

### 3.2 UI组件
- **UI Manager**: 管理玩家UI元素
- **Slider**: 用于资源条显示
- **TextMeshProUGUI**: 用于文本信息显示
- **Image**: 用于背景和图标显示

### 3.3 其他组件
- **BoxCollider2D**: 用于碰撞检测
- **Rigidbody2D**: 用于物理交互(可选)
- **Animator**: 用于角色动画(可选)

## 4. 详细搭建步骤

### 4.1 基础结构搭建
1. 在Unity编辑器中打开Prefabs文件夹
2. 选择现有的`playerprefab.prefab`进行编辑
3. 添加以下空GameObject作为子节点：
   - PlayerCore
   - HandCards
   - FieldCards
   - UI

### 4.2 核心组件添加
1. **PlayerData.cs 脚本创建** (放置在 `Assets/Script/core/` 文件夹下):
   ```csharp
   using System.Collections;
   using System.Collections.Generic;
   using UnityEngine;

   [System.Serializable]
   public class PlayerData
   {
       public string playerName;
       public int playerID;
       public int hope; // 玩家的Hope作为血量
       public int maxHope;
       public int faith;
       public int maxFaith;
   }
   ```

2. **PlayerController.cs 脚本创建** (放置在 `Assets/Script/core/` 文件夹下):
   ```csharp
   using System.Collections;
   using System.Collections.Generic;
   using UnityEngine;

   public class PlayerController : MonoBehaviour
   {
       public PlayerData playerData;
       public CardZone handCardsZone;
       public CardZone fieldCardsZone;

       // Start is called before the first frame update
       void Start()
       {
           InitializePlayer();
       }

       // 初始化玩家数据
       private void InitializePlayer()
       {
           playerData.hope = playerData.maxHope; // Hope作为玩家血量
           playerData.faith = playerData.maxFaith;
       }

       // 更新玩家Hope血量
       public void UpdateHope(int amount)
       {
           playerData.hope = Mathf.Clamp(playerData.hope + amount, 0, playerData.maxHope);
           // 触发UI更新事件
       }

       // 更新Faith资源
       public void UpdateFaith(int amount)
       {
           playerData.faith = Mathf.Clamp(playerData.faith + amount, 0, playerData.maxFaith);
           // 触发UI更新事件
       }

       // 抽卡
       public void DrawCard(CardDataSO card)
       {
           handCardsZone.AddCard(card);
       }

       // 出牌到场上
       public bool PlayCardToField(CardDataSO card)
       {
           if (fieldCardsZone.CanAddCard(card) && handCardsZone.RemoveCard(card))
           {
               fieldCardsZone.AddCard(card);
               return true;
           }
           return false;
       }
   }
   ```

3. **CardZone.cs 脚本创建** (放置在 `Assets/Script/gameplay/` 文件夹下):
   ```csharp
   using System.Collections;
   using System.Collections.Generic;
   using UnityEngine;

   public class CardZone : MonoBehaviour
   {
       public string zoneName;
       public int maxCards = 7;
       public List<CardDataSO> cards = new List<CardDataSO>();

       // 添加卡牌
       public bool AddCard(CardDataSO card)
       {
           if (cards.Count < maxCards)
           {
               cards.Add(card);
               // 触发卡牌添加事件
               return true;
           }
           return false;
       }

       // 移除卡牌
       public bool RemoveCard(CardDataSO card)
       {
           if (cards.Contains(card))
           {
               cards.Remove(card);
               // 触发卡牌移除事件
               return true;
           }
           return false;
       }

       // 检查是否可以添加卡牌
       public bool CanAddCard(CardDataSO card)
       {
           return cards.Count < maxCards;
       }

       // 获取区域内的卡牌数量
       public int GetCardCount()
       {
           return cards.Count;
       }
   }
   ```

4. 将这些脚本添加到PlayerPrefab的相应节点 (在Unity编辑器中的详细操作步骤):

   ### 4.2.1 添加 PlayerController 脚本
   1. 在 Unity 编辑器中，双击 `Assets/Prefabs/playerprefab.prefab` 打开预制体编辑模式
   2. 选择 `PlayerCore` 节点
   3. 在 Inspector 面板中，点击 "Add Component" 按钮
   4. 在搜索框中输入 "PlayerController" 并选择该脚本
   5. PlayerData 将作为 PlayerController 的序列化字段自动显示在 Inspector 面板中

   ### 4.2.2 添加 CardZone 脚本到 HandCards 节点
   1. 在预制体编辑模式中，选择 `HandCards` 节点
   2. 点击 "Add Component" 按钮
   3. 搜索并添加 "CardZone" 脚本
   4. 在 Inspector 面板中设置参数：
      - zoneName: "HandCards"
      - maxCards: 7

   ### 4.2.3 添加 CardZone 脚本到 FieldCards 节点
   1. 在预制体编辑模式中，选择 `FieldCards` 节点
   2. 点击 "Add Component" 按钮
   3. 搜索并添加 "CardZone" 脚本
   4. 在 Inspector 面板中设置参数：
      - zoneName: "FieldCards"
      - maxCards: 5 (根据游戏设计调整)

   ### 4.2.4 设置脚本引用关系
   1. 选择 `PlayerCore` 节点 (包含 PlayerController 脚本的节点)
   2. 在 Inspector 面板中找到 PlayerController 组件
   3. 点击 `handCardsZone` 字段右侧的小圆圈图标
   4. 在弹出的选择窗口中选择 `HandCards` 节点
   5. 同样地，点击 `fieldCardsZone` 字段右侧的小圆圈图标，选择 `FieldCards` 节点

   这样就完成了脚本的添加和引用设置，PlayerController 将能够管理手牌和场上卡牌区域。

### 4.3 UI 组件搭建
1. 在UI节点下创建以下UI元素：
   - StatusPanel: 使用Panel组件，包含玩家名称、生命值显示
   - ResourceBar: 使用Panel组件，包含Hope和Faith资源条

2. 设置UI元素的属性：
   - StatusPanel: 调整大小和位置，添加TextMeshProUGUI显示玩家名称和Hope血量
   - ResourceBar: 添加一个Slider组件用于Faith显示(玩家的Hope作为血量已在StatusPanel显示)

3. 创建UI管理器脚本，用于更新UI显示：
   ```csharp
   using System.Collections;
   using System.Collections.Generic;
   using UnityEngine;
   using TMPro;

   public class PlayerUIManager : MonoBehaviour
   {
       public TextMeshProUGUI playerNameText;
       public TextMeshProUGUI hopeHealthText;
       public UnityEngine.UI.Slider faithSlider;

       private PlayerController playerController;

       // 设置玩家控制器
       public void SetPlayerController(PlayerController controller)
       {
           playerController = controller;
           UpdateUI();
       }

       // 更新UI显示
       public void UpdateUI()
       {
           if (playerController == null) return;

           playerNameText.text = playerController.playerData.playerName;
           // Hope作为玩家血量显示
           hopeHealthText.text = $"Hope: {playerController.playerData.hope}/{playerController.playerData.maxHope}";
           faithSlider.value = (float)playerController.playerData.faith / playerController.playerData.maxFaith;
       }
   ```

## 6. 脚本文件夹结构建议

为了保持项目的模块化和可维护性，建议按照以下结构组织脚本文件：

```
   Assets/Script/
   ├── core/             # 核心游戏系统
   │   ├── PlayerController.cs
   │   └── PlayerData.cs    # 玩家数据：Hope为玩家血量
   ├── data/             # 数据相关脚本(已有)
   │   ├── CardDataSO.cs    # 卡牌数据：Health为卡牌血量
   │   └── CardDatabaseSO.cs
   ├── gameplay/         # 游戏玩法相关脚本
   │   └── CardZone.cs
   └── ui/               # UI相关脚本
       └── PlayerUIManager.cs
   ```

### 6.1 文件夹用途说明
1. **core/**: 存放游戏核心系统和玩家控制器等基础组件
2. **data/**: 存放数据模型和资源管理脚本(已有)
3. **gameplay/**: 存放游戏玩法逻辑相关脚本
4. **ui/**: 存放UI管理和交互相关脚本

### 6.2 创建文件夹的方法
在Unity编辑器中：
1. 右键点击 `Assets/Script/` 文件夹
2. 选择 `Create` → `Folder`
3. 输入文件夹名称(如 core、gameplay、ui)
4. 将对应脚本拖拽到新建的文件夹中

这种模块化的文件夹结构有助于提高代码的可维护性和可读性，便于团队协作和后续功能扩展。

### 4.4 配置和测试
1. 设置PlayerController的初始参数：
   - playerName: 玩家名称
   - maxHope: 最大Hope血量 (玩家的血量)
   - maxFaith: 最大Faith资源

2. 配置CardZone的参数：
   - HandCards: 设置maxCards为7
   - FieldCards: 设置maxCards为5(根据游戏设计调整)

3. 测试PlayerPrefab的功能：
   - 检查组件是否正确添加
   - 测试数据更新功能
   - 验证UI是否正常显示

## 5. 与其他系统的交互

### 5.1 与游戏管理器的交互
- **GameManager**: 负责初始化玩家、管理游戏状态和回合
- **交互方式**: 通过事件系统或直接引用进行通信

### 5.2 与卡牌系统的交互
- **卡牌系统**: 卡牌的Health属性代表卡牌的血量
- **交互方式**: PlayerController通过CardZone管理卡牌，卡牌的Health在战斗中会被修改

```csharp
// GameManager 中初始化玩家示例
public class GameManager : MonoBehaviour
{
    public PlayerController player1Prefab;
    public PlayerController player2Prefab;
    
    private PlayerController player1;
    private PlayerController player2;

    void Start()
    {
        // 实例化玩家
        player1 = Instantiate(player1Prefab, new Vector3(-5, 0, 0), Quaternion.identity);
        player2 = Instantiate(player2Prefab, new Vector3(5, 0, 0), Quaternion.identity);
        
        // 设置玩家名称
        player1.playerData.playerName = "Player 1";
        player2.playerData.playerName = "Player 2";
        
        // 初始化卡组
        InitializeDecks();
    }
    
    // 初始化卡组
    private void InitializeDecks()
    {
        // 从CardDatabase获取初始卡组
        List<CardDataSO> player1Deck = CardDatabaseSO.Instance.GetInitialDeck(1);
        List<CardDataSO> player2Deck = CardDatabaseSO.Instance.GetInitialDeck(2);
        
        // 给玩家发初始手牌
        foreach (CardDataSO card in player1Deck.GetRange(0, 5))
        {
            player1.DrawCard(card);
        }
        
        foreach (CardDataSO card in player2Deck.GetRange(0, 5))
        {
            player2.DrawCard(card);
        }
    }
}
```

### 5.2 与卡牌系统的交互
- **CardSystem**: 管理卡牌的生成、显示和交互
- **交互方式**: 通过CardZone组件进行卡牌的添加和移除

### 5.3 与回合系统的交互
- **TurnSystem**: 管理游戏回合
- **交互方式**: 通过事件系统通知玩家回合开始和结束

## 6. 优化和扩展建议

### 6.1 性能优化
1. 使用对象池管理卡牌实例
2. 减少UI更新频率
3. 使用ScriptableObject存储配置数据

### 6.2 功能扩展
1. 添加玩家动画和特效
2. 实现玩家技能系统
3. 添加玩家个性化设置
4. 支持多人游戏模式

### 6.3 调试和维护
1. 添加日志和调试信息
2. 使用状态机管理玩家状态
3. 实现数据持久化功能

## 7. 总结

通过以上步骤，我们可以搭建一个功能完整的PlayerPrefab，包含玩家核心数据管理、卡牌区域管理和UI显示功能。这个PlayerPrefab将能够与游戏管理器、卡牌系统和回合系统等其他系统进行良好的交互，为游戏的核心玩法提供支持。

在实际开发过程中，可以根据游戏设计的具体需求对PlayerPrefab的结构和功能进行调整和扩展，确保其能够满足游戏的核心玩法和用户体验要求。