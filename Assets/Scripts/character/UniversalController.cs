using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class UniversalController : MonoBehaviour
{
    [Header("基本信息")]
    public string characterName = "角色";
    public bool isPlayerControlled = true;
    
    [Header("资源系统")]
    public ResourceSystem resourceSystem = new ResourceSystem();
    
    [Header("区域引用")]
    public CardZone handZone;       // 手牌区域
    public CardZone battlefield;    // 战场区域
       
    [Header("卡牌管理")]
    public CardDatabaseSO cardDatabase; // 卡牌数据库
    protected List<CardEntity> _handCards = new List<CardEntity>(); // 当前手牌
    //public List<CardDataSO> deckCards = new List<CardDataSO>();  // 卡组中的卡牌
    //protected List<CardDataSO> _currentDeck = new List<CardDataSO>(); // 当前卡组（会变化）
    //protected List<CardDataSO> _discardPile = new List<CardDataSO>(); // 弃牌堆
    
    [Header("状态")]
    public bool isActive = true;
    public bool isMyTurn = false;
    public bool canPlayCards = true;
    
    [Header("游戏数据")]
    public int cardsPlayedThisTurn = 0;
    public int maxCardsPerTurn = 10;

     [Header("UI控制")]
    public CharacterUIController uiController;
    
    
    // 事件
    public System.Action<UniversalController> OnTurnStart;
    public System.Action<UniversalController> OnTurnEnd;
    public System.Action<CardEntity> OnCardDrawn;
    public System.Action<CardEntity> OnCardPlayed;
    public System.Action<CardEntity> OnCardDiscarded;
    
    protected virtual void Awake()
    {
        InitializeComponents();

        // 初始化UI控制器
        InitializeUI();
    }
    
    // 初始化组件
    private void InitializeComponents()
    {
        // 确保有必要的组件
        if (handZone == null) handZone = GetComponentInChildren<CardZone>();
        if (battlefield == null) battlefield = GetComponentInChildren<CardZone>();
        
        // 初始化区域
        if (handZone != null)
        {
            handZone.maxCards = 10;
        }
        
        if (battlefield != null)
        {
            battlefield.frontRowCount = 2;
            battlefield.backRowCount = 3;
        }
    }

    // 初始化UI
    private void InitializeUI()
    {
        // 如果未指定UI控制器，尝试自动查找
        if (uiController == null)
        {
            uiController = GetComponentInChildren<CharacterUIController>();
        }
        
        // 初始化UI控制器
        if (uiController != null)
        {
            uiController.Initialize(this);
        }
        else
        {
            Debug.LogWarning($"{characterName} 没有找到UI控制器");
        }
    }
    
    // 初始化角色
    public virtual void Initialize(string name, bool isPlayer, int startingHope, int startingFaith)
    {
        characterName = name;
        isPlayerControlled = isPlayer;
        
        resourceSystem = new ResourceSystem(startingHope, startingFaith);
        
        // 初始化卡组
        InitializeDeck();
        
        // 订阅战场事件
        if (battlefield != null)
        {
            battlefield.onCardAdded.AddListener(OnBattlefieldCardAdded);
            battlefield.onCardRemoved.AddListener(OnBattlefieldCardRemoved);
        }
        
        Debug.Log($"{characterName} 初始化完成 | Hope: {startingHope} | Faith: {startingFaith}");
    }
    
    // 初始化卡组
    protected virtual void InitializeDeck()
    {
        cardDatabase.BackupPlayerDeck();


        //_currentDeck.Clear();
        //_currentDeck.AddRange(deckCards);
        //ShuffleDeck();
        
        Debug.Log($"{characterName} 卡组初始化: {cardDatabase.playerDeckCardIds.Count} 张卡牌");
    }
    
    // 洗牌
    //public void ShuffleDeck()
    //{
    //    // Fisher-Yates 洗牌算法
    //    for (int i = _currentDeck.Count - 1; i > 0; i--)
    //    {
    //        int randomIndex = Random.Range(0, i + 1);
    //        CardDataSO temp = _currentDeck[i];
    //        _currentDeck[i] = _currentDeck[randomIndex];
    //        _currentDeck[randomIndex] = temp;
    //    }
    //}
    
    // 抽牌
    public virtual CardEntity DrawCard()
    {
        if (cardDatabase.playerDeckCardIds.Count == 0)
        {
            // 卡组空，尝试从弃牌堆回收
            RecycleDiscardPile();
            
            if (cardDatabase.playerDeckCardIds.Count == 0)
            {
                Debug.LogWarning($"{characterName} 的卡组已空，无法抽牌");
                return null;
            }
        }
        
        // 从卡组顶部抽牌
        CardDataSO cardData = cardDatabase.DrawCardsFromPlayerDeck(1,true)[0];
        //_currentDeck.RemoveAt(0);
        
        // 创建卡牌实体
        CardEntity card = CreateCardEntity(cardData);
        if (card != null)
        {
            AddCardToHand(card);
            OnCardDrawn?.Invoke(card);
            
            Debug.Log($"{characterName} 抽到: {cardData.cardName}");
            
            return card;
        }
        
        return null;
    }
    
    // 抽初始手牌
    public virtual IEnumerator DrawStartingHand(int handSize)
    {
        for (int i = 0; i < handSize; i++)
        {
            DrawCard();
            yield return new WaitForSeconds(0.1f);
        }
        
        Debug.Log($"{characterName} 初始手牌: {_handCards.Count} 张");
    }
    
    // 创建卡牌实体
    protected virtual CardEntity CreateCardEntity(CardDataSO cardData)
    {
        if (CardFactory.Instance == null || cardData == null) return null;
        
        return CardFactory.Instance.CreateCard(cardData, this);
    }
    
    // 添加卡牌到手牌
    public virtual void AddCardToHand(CardEntity card)
    {
        if (card == null || handZone == null) return;
        
        // 添加到手牌区域
        bool placed = handZone.AutoPlaceCard(card, true);
        if (placed)
        {
            _handCards.Add(card);
            
            // 设置卡牌状态
            card.SetOnBoard(false);
            card.SetPlayable(canPlayCards && CanPlayCard(card));
            
            // 订阅卡牌事件
            card.OnCardClicked += OnHandCardClicked;
        }
        else
        {
            Debug.LogWarning($"{characterName} 手牌区域已满，无法添加卡牌");
        }
    }
    
    // 从手牌移除卡牌
    public virtual void RemoveCardFromHand(CardEntity card)
    {
        if (!_handCards.Contains(card)) return;
        
        _handCards.Remove(card);
        card.OnCardClicked -= OnHandCardClicked;
        
        if (handZone != null)
        {
            handZone.RemoveCard(card);
        }
    }
    
    // 手牌卡牌被点击
    protected virtual void OnHandCardClicked(CardEntity card)
    {
        if (isMyTurn && canPlayCards && card.IsPlayable)
        {
            // 玩家控制：显示可放置位置
            // AI控制：自动处理
            TryPlayCard(card);
        }
    }
    
    // 尝试打出卡牌
    public virtual bool TryPlayCard(CardEntity card)
    {
        if (!CanPlayCard(card)) return false;
        
        // 寻找最佳位置
        CardZone.CardPosition bestPosition = FindBestPositionForCard(card);
        if (bestPosition == null) return false;
        
        return PlayCardToPosition(card, bestPosition);
    }
    
    // 检查是否可以打出卡牌
    public virtual bool CanPlayCard(CardEntity card)
    {
        if (card == null) return false;
        
        // 检查资源
        if (!resourceSystem.CanAfford(card.CardData.FaithCost))
        {
            return false;
        }
        
        // 检查是否在回合中
        if (!isMyTurn)
        {
            return false;
        }
        
        // 检查回合出牌限制
        if (cardsPlayedThisTurn >= maxCardsPerTurn)
        {
            return false;
        }
        
        // 检查战场是否有空位
        if (battlefield == null || battlefield.IsFull())
        {
            return false;
        }
        
        // 检查卡牌放置限制
        if (!CanPlaceCardAnywhere(card))
        {
            return false;
        }
        
        return true;
    }
    
    // 检查卡牌是否可以放置在战场上任何位置
    protected virtual bool CanPlaceCardAnywhere(CardEntity card)
    {
        if (battlefield == null) return false;
        
        var emptyPositions = battlefield.GetEmptyPositions();
        foreach (var position in emptyPositions)
        {
            if (battlefield.CanPlaceCardAtPosition(card, position.isFrontRow))
            {
                return true;
            }
        }
        
        return false;
    }
    
    // 寻找最佳位置
    protected virtual CardZone.CardPosition FindBestPositionForCard(CardEntity card)
    {
        if (battlefield == null) return null;
        
        // 优先前排
        var frontRowPositions = battlefield.GetEmptyPositions(true);
        foreach (var position in frontRowPositions)
        {
            if (battlefield.CanPlaceCardAtPosition(card, true))
            {
                return position;
            }
        }
        
        // 其次后排
        var backRowPositions = battlefield.GetEmptyPositions(false);
        foreach (var position in backRowPositions)
        {
            if (battlefield.CanPlaceCardAtPosition(card, false))
            {
                return position;
            }
        }
        
        return null;
    }
    
    // 在指定位置打出卡牌
    public virtual bool PlayCardToPosition(CardEntity card, CardZone.CardPosition position)
    {
        if (card == null || position == null || battlefield == null) return false;
        
        // 消耗资源
        if (!resourceSystem.SpendFaith(card.CardData.FaithCost)) return false;
        
        // 放置到战场
        bool placed = battlefield.PlaceCardAtPosition(
            card, 
            position.isFrontRow, 
            position.positionIndex
        );
        
        if (placed)
        {
            RemoveCardFromHand(card);
            cardsPlayedThisTurn++;
            
            OnCardPlayed?.Invoke(card);
            
            Debug.Log($"{characterName} 打出 {card.CardData.CardName}");
            
            return true;
        }
        
        return false;
    }
    
    // 开始回合
    public virtual void StartTurn()
    {
        isMyTurn = true;
        canPlayCards = true;
        cardsPlayedThisTurn = 0;
        
        // 回合开始获得Faith
        resourceSystem.EndTurnGainFaith();
        
        // 抽一张牌
        DrawCard();
        
        // 更新手牌可打出状态
        UpdateHandCardsPlayability();
        
        // 触发事件
        OnTurnStart?.Invoke(this);

        if (uiController != null)
        {
            uiController.ShowTurnIndicator(true);
        }
        
        Debug.Log($"{characterName} 的回合开始");
    }
    
    // 结束回合
    public virtual void EndTurn()
    {
        isMyTurn = false;
        canPlayCards = false;
        
        // 更新手牌状态
        foreach (var card in _handCards)
        {
            card.SetPlayable(false);
        }
        
        // 检查战场状态（Hope增减规则）
        CheckBattlefieldHopeRules();
        
        // 触发事件
        OnTurnEnd?.Invoke(this);

        if (uiController != null)
        {
            uiController.ShowTurnIndicator(false);
        }
        
        Debug.Log($"{characterName} 的回合结束");
    }
    
    // 检查战场Hope规则
    protected virtual void CheckBattlefieldHopeRules()
    {
        if (battlefield == null) return;
        
        int cardCount = battlefield.GetCardCount();
        
        if (cardCount == 0)
        {
            // 场上无卡牌，减少Hope
            resourceSystem.CurrentHope -= 2;
            Debug.Log($"{characterName} 场上无卡牌，Hope -2");
        }
        else
        {
            // 场上有卡牌，增加Hope
            resourceSystem.CurrentHope += 1;
            Debug.Log($"{characterName} 场上有卡牌，Hope +1");
        }
    }
    
    // 更新手牌可打出状态
    protected virtual void UpdateHandCardsPlayability()
    {
        foreach (var card in _handCards)
        {
            bool playable = CanPlayCard(card);
            card.SetPlayable(playable);
        }
    }
    
    // 从弃牌堆回收卡牌
    protected virtual void RecycleDiscardPile()
    {
        if (cardDatabase.playerDiscardPileCardIds.Count == 0) return;
        
        //_currentDeck.AddRange(_discardPile);
        //_discardPile.Clear();
        //ShuffleDeck();
        
        //Debug.Log($"{characterName} 从弃牌堆回收了 {_currentDeck.Count} 张卡牌");
    }
    
    // 弃牌
    public virtual void DiscardCard(CardEntity card)
    {
        if (card == null) return;
        
        // 移除卡牌实体
        RemoveCardFromHand(card);
        Destroy(card.gameObject);
        
        // 添加到弃牌堆
        string cardDataId = FindCardDataInDeck(card.CardData.CardId);
        if (cardDataId != null)
        {
            cardDatabase.playerDiscardPileCardIds.Add(cardDataId);
        }
        
        OnCardDiscarded?.Invoke(card);
    }
    
    // 在卡组中查找卡牌数据
    protected virtual string FindCardDataInDeck(string cardId)
    {
        foreach (var cardDataId in cardDatabase.playerDeckCardIds)
        {
            if (cardDataId != null && cardDataId == cardId)
            {
                return cardDataId;
            }
        }
        //{
        //    if (cardData.cardId == cardId)
        //    {
        //        return cardData;
        //    }
        //}
        return null;
    }
    
    // 战场卡牌添加事件
    protected virtual void OnBattlefieldCardAdded(CardEntity card)
    {
        Debug.Log($"{characterName} 的战场添加了卡牌: {card.CardData.CardName}");
    }
    
    // 战场卡牌移除事件
    protected virtual void OnBattlefieldCardRemoved(CardEntity card)
    {
        // 卡牌被击败时减少Hope
        int hopeLoss = CalculateHopeLossFromCard(card);
        resourceSystem.CurrentHope -= hopeLoss;
        
        Debug.Log($"{characterName} 的战场移除了卡牌: {card.CardData.CardName}, Hope -{hopeLoss}");
    }
    
    // 计算卡牌被击败时的Hope损失
    protected virtual int CalculateHopeLossFromCard(CardEntity card)
    {
        int baseLoss = 1;
        
        if (card.CardData.FaithCost >= 5)
        {
            baseLoss += 1; // 高消耗卡牌多减1点
        }
        
        if (card.CardData.Rarity == Rarity.Epic || card.CardData.Rarity == Rarity.Legendary)
        {
            baseLoss += 1; // 稀有卡牌多减1点
        }
        
        return baseLoss;
    }
    
    // 获取当前手牌
    public virtual List<CardEntity> GetHandCards()
    {
        return new List<CardEntity>(_handCards);
    }
    
    // 获取当前手牌数量
    public virtual int GetHandCount()
    {
        return _handCards.Count;
    }
    
    // 获取战场卡牌
    public virtual List<CardEntity> GetBattlefieldCards()
    {
        return battlefield?.GetAllCards() ?? new List<CardEntity>();
    }
    
    // 获取战场卡牌数量
    public virtual int GetBattlefieldCount()
    {
        return battlefield?.GetCardCount() ?? 0;
    }
    
    // 获取剩余卡组数量
    public virtual int GetDeckCount()
    {
        return cardDatabase.playerDeckCardIds.Count;
    }
    
    // 获取弃牌堆数量
    public virtual int GetDiscardCount()
    {
        return cardDatabase.playerDiscardPileCardIds.Count;
    }
    
    // 检查是否存活
    public virtual bool IsAlive()
    {
        return resourceSystem.IsAlive;
    }
    
    // 重置状态
    public virtual void ResetState()
    {
        isMyTurn = false;
        canPlayCards = true;
        cardsPlayedThisTurn = 0;
        
        // 清空手牌
        foreach (var card in _handCards)
        {
            Destroy(card.gameObject);
        }
        _handCards.Clear();
        
        // 清空战场
        if (battlefield != null)
        {
            battlefield.ClearZone();
        }
        
        // 重置卡组
        cardDatabase.RestorePlayerDeckFromBackup();
        
        // 重置资源
        resourceSystem.ResetResources();
    }

    // UI相关方法
    //protected virtual void ShowFloatingText(Vector3 position, string text, Color color)
    //{
    //    if (uiController != null)
    //    {
    //        uiController.ShowFloatingText(position, text, color);
    //    }
    //}

    //public virtual void ShowDamageEffect(CardEntity target, int damage)
    //{
    //    if (uiController != null)
    //    {
    //        uiController.ShowDamageEffect(target, damage);
    //    }
    //}
    
    //public virtual void ShowHealEffect(CardEntity target, int healAmount)
    //{
    //    if (uiController != null)
    //    {
    //        uiController.ShowHealEffect(target, healAmount);
    //    }
    //}



    // 编辑器工具：查看状态
    [ContextMenu("打印状态信息")]
    public virtual void PrintStatus()
    {
        Debug.Log($"=== {characterName} 状态 ===");
        Debug.Log($"Hope: {resourceSystem.CurrentHope}/{resourceSystem.maxHope}");
        Debug.Log($"Faith: {resourceSystem.CurrentFaith}/{resourceSystem.maxFaith}");
        Debug.Log($"手牌: {GetHandCount()} 张");
        Debug.Log($"战场: {GetBattlefieldCount()} 张");
        Debug.Log($"卡组: {GetDeckCount()} 张");
        Debug.Log($"弃牌: {GetDiscardCount()} 张");
        Debug.Log($"是否存活: {IsAlive()}");
        Debug.Log($"是否我的回合: {isMyTurn}");
    }
}