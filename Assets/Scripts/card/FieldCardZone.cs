using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FieldCardZone : MonoBehaviour
{
    [System.Serializable]
    public class CardPosition
    {
        public bool isOccupied = false;
        public CardEntity occupiedCard;
        public Transform positionTransform; // 可选：用于视觉定位
        
        // 是否是前排位置
        public bool isFrontRow;
        
        // 位置索引（前排0-1，后排0-2）
        public int positionIndex;
    }
    
    [Header("区域基本信息")]
    public string zoneName = "战场区域";
    public int maxCards = 5; // 2前排 + 3后排
    public bool isPlayerZone = true; // true=玩家区域，false=敌人区域
    
    [Header("前后排划分")]
    public int frontRowCount = 2;
    public int backRowCount = 3;
    
    [Header("位置点（可选）")]
    public Transform[] frontRowPositions; // 前排位置点
    public Transform[] backRowPositions;  // 后排位置点
    
    // 位置管理
    private List<CardPosition> _cardPositions = new List<CardPosition>();
    
    // 事件
    public UnityEvent<CardEntity> onCardAdded;
    public UnityEvent<CardEntity> onCardRemoved;
    public UnityEvent onZoneUpdated;
    
    private void Awake()
    {
        InitializePositions();
    }
    
    // 初始化位置
    private void InitializePositions()
    {
        _cardPositions.Clear();
        
        // 创建前排位置
        for (int i = 0; i < frontRowCount; i++)
        {
            CardPosition position = new CardPosition
            {
                isFrontRow = true,
                positionIndex = i,
                positionTransform = frontRowPositions != null && i < frontRowPositions.Length 
                    ? frontRowPositions[i] 
                    : null
            };
            _cardPositions.Add(position);
        }
        
        // 创建后排位置
        for (int i = 0; i < backRowCount; i++)
        {
            CardPosition position = new CardPosition
            {
                isFrontRow = false,
                positionIndex = i,
                positionTransform = backRowPositions != null && i < backRowPositions.Length 
                    ? backRowPositions[i] 
                    : null
            };
            _cardPositions.Add(position);
        }
    }
    
    // 获取所有位置
    public List<CardPosition> GetAllPositions()
    {
        return new List<CardPosition>(_cardPositions);
    }
    
    // 获取前排位置
    public List<CardPosition> GetFrontRowPositions()
    {
        return _cardPositions.FindAll(p => p.isFrontRow);
    }
    
    // 获取后排位置
    public List<CardPosition> GetBackRowPositions()
    {
        return _cardPositions.FindAll(p => !p.isFrontRow);
    }
    
    // 获取空位置
    public List<CardPosition> GetEmptyPositions(bool frontRowOnly = false)
    {
        if (frontRowOnly)
        {
            return _cardPositions.FindAll(p => p.isFrontRow && !p.isOccupied);
        }
        return _cardPositions.FindAll(p => !p.isOccupied);
    }
    
    // 获取指定位置的卡牌
    public CardEntity GetCardAtPosition(bool isFrontRow, int positionIndex)
    {
        CardPosition position = FindPosition(isFrontRow, positionIndex);
        return position?.occupiedCard;
    }
    
    // 查找指定位置
    private CardPosition FindPosition(bool isFrontRow, int positionIndex)
    {
        return _cardPositions.Find(p => p.isFrontRow == isFrontRow && p.positionIndex == positionIndex);
    }
    
    // 在指定位置放置卡牌
    public bool PlaceCardAtPosition(CardEntity card, bool isFrontRow, int positionIndex)
    {
        CardPosition position = FindPosition(isFrontRow, positionIndex);
        
        if (position == null)
        {
            Debug.LogWarning($"位置不存在: {(isFrontRow ? "前排" : "后排")}_{positionIndex}");
            return false;
        }
        
        if (position.isOccupied)
        {
            Debug.LogWarning($"位置已被占用: {(isFrontRow ? "前排" : "后排")}_{positionIndex}");
            return false;
        }
        
        // 检查卡牌是否可以放置在这个位置
        if (!CanPlaceCardAtPosition(card, isFrontRow))
        {
            Debug.LogWarning($"卡牌不能放置在此位置: {card.CardData.CardName}");
            return false;
        }
        
        // 放置卡牌
        position.isOccupied = true;
        position.occupiedCard = card;
        
        // 移动卡牌到位置点
        if (position.positionTransform != null)
        {
            card.transform.position = position.positionTransform.position;
            card.transform.rotation = position.positionTransform.rotation;
        }
        
        // 设置卡牌状态
        card.SetOnBoard(true);
        
        // 触发事件
        onCardAdded?.Invoke(card);
        onZoneUpdated?.Invoke();
        
        return true;
    }
    
    // 自动放置卡牌（找到第一个合适的位置）
    public bool AutoPlaceCard(CardEntity card, bool preferFrontRow = true)
    {
        // 先尝试首选行
        var positions = preferFrontRow ? GetFrontRowPositions() : GetBackRowPositions();
        positions = positions.FindAll(p => !p.isOccupied && CanPlaceCardAtPosition(card, p.isFrontRow));
        
        if (positions.Count == 0)
        {
            // 尝试另一行
            positions = preferFrontRow ? GetBackRowPositions() : GetFrontRowPositions();
            positions = positions.FindAll(p => !p.isOccupied && CanPlaceCardAtPosition(card, p.isFrontRow));
        }
        
        if (positions.Count == 0)
        {
            Debug.LogWarning($"没有可用位置放置卡牌: {card.CardData.CardName}");
            return false;
        }
        
        // 选择第一个可用位置
        CardPosition selectedPosition = positions[0];
        return PlaceCardAtPosition(card, selectedPosition.isFrontRow, selectedPosition.positionIndex);
    }
    
    // 检查卡牌是否可以放置在指定行
    public bool CanPlaceCardAtPosition(CardEntity card, bool isFrontRow)
    {
        // 检查卡牌数据是否允许放置在该行
        if (isFrontRow && !card.CardData.CanPlaceFront)
            return false;
            
        if (!isFrontRow && !card.CardData.CanPlaceBack)
            return false;
            
        return true;
    }
    
    // 移除指定位置的卡牌
    public CardEntity RemoveCardFromPosition(bool isFrontRow, int positionIndex)
    {
        CardPosition position = FindPosition(isFrontRow, positionIndex);
        
        if (position == null || !position.isOccupied)
        {
            return null;
        }
        
        CardEntity card = position.occupiedCard;
        
        position.isOccupied = false;
        position.occupiedCard = null;
        
        // 触发事件
        onCardRemoved?.Invoke(card);
        onZoneUpdated?.Invoke();
        
        return card;
    }
    
    // 移除指定的卡牌（通过卡牌引用）
    public bool RemoveCard(CardEntity card)
    {
        CardPosition position = _cardPositions.Find(p => p.occupiedCard == card);
        
        if (position == null)
        {
            return false;
        }
        
        position.isOccupied = false;
        position.occupiedCard = null;
        
        // 触发事件
        onCardRemoved?.Invoke(card);
        onZoneUpdated?.Invoke();
        
        return true;
    }
    
    // 获取区域中所有卡牌
    public List<CardEntity> GetAllCards()
    {
        List<CardEntity> allCards = new List<CardEntity>();
        
        foreach (var position in _cardPositions)
        {
            if (position.isOccupied && position.occupiedCard != null)
            {
                allCards.Add(position.occupiedCard);
            }
        }
        
        return allCards;
    }
    
    // 获取前排所有卡牌
    public List<CardEntity> GetFrontRowCards()
    {
        List<CardEntity> cards = new List<CardEntity>();
        
        foreach (var position in GetFrontRowPositions())
        {
            if (position.isOccupied && position.occupiedCard != null)
            {
                cards.Add(position.occupiedCard);
            }
        }
        
        return cards;
    }
    
    // 获取后排所有卡牌
    public List<CardEntity> GetBackRowCards()
    {
        List<CardEntity> cards = new List<CardEntity>();
        
        foreach (var position in GetBackRowPositions())
        {
            if (position.isOccupied && position.occupiedCard != null)
            {
                cards.Add(position.occupiedCard);
            }
        }
        
        return cards;
    }
    
    // 检查是否有前排保护（前排有卡牌时，后排不能被攻击）
    public bool HasFrontRowProtection()
    {
        return GetFrontRowCards().Count > 0;
    }
    
    // 获取卡牌所在的位置信息
    public CardPosition GetCardPosition(CardEntity card)
    {
        return _cardPositions.Find(p => p.occupiedCard == card);
    }
    
    // 查找可以攻击的目标
    public List<CardEntity> GetAttackableTargets(CardEntity attacker, bool canAttackBackRow = false)
    {
        List<CardEntity> targets = new List<CardEntity>();
        
        // 如果攻击者有远程攻击能力，可以攻击任何目标
        if (canAttackBackRow || (attacker.CardData.HasEffect(SpecialEffect.RangedAttack)))
        {
            targets = GetAllCards();
        }
        else
        {
            // 只能攻击前排
            targets = GetFrontRowCards();
        }
        
        // 过滤掉死亡或无效的卡牌
        targets = targets.FindAll(c => c != null && c.CardData.IsAlive);
        
        return targets;
    }
    
    // 获取位置索引（用于UI显示等）
    public int GetPositionIndex(bool isFrontRow, int rowIndex)
    {
        // 前排：0-1，后排：2-4
        if (isFrontRow)
        {
            return rowIndex; // 0-1
        }
        else
        {
            return frontRowCount + rowIndex; // 2-4
        }
    }
    
    // 检查是否已满
    public bool IsFull()
    {
        return GetEmptyPositions().Count == 0;
    }
    
    // 获取区域中的卡牌数量
    public int GetCardCount()
    {
        return GetAllCards().Count;
    }
    
    // 获取区域容量
    public int GetCapacity()
    {
        return frontRowCount + backRowCount;
    }
    
    // 清空区域（移除所有卡牌）
    public List<CardEntity> ClearZone()
    {
        List<CardEntity> removedCards = new List<CardEntity>();
        
        foreach (var position in _cardPositions)
        {
            if (position.isOccupied && position.occupiedCard != null)
            {
                removedCards.Add(position.occupiedCard);
                position.isOccupied = false;
            }
        }
        
        // 触发事件
        if (removedCards.Count > 0)
        {
            onZoneUpdated?.Invoke();
        }
        
        return removedCards;
    }
    
    // 交换两个位置的卡牌
    public bool SwapCards(CardPosition position1, CardPosition position2)
    {
        if (position1 == null || position2 == null)
            return false;
            
        if (!position1.isOccupied || !position2.isOccupied)
            return false;
            
        // 交换卡牌
        CardEntity tempCard = position1.occupiedCard;
        position1.occupiedCard = position2.occupiedCard;
        position2.occupiedCard = tempCard;
        
        // 更新位置
        if (position1.positionTransform != null)
        {
            position2.occupiedCard.transform.position = position1.positionTransform.position;
        }
        
        if (position2.positionTransform != null)
        {
            position1.occupiedCard.transform.position = position2.positionTransform.position;
        }
        
        onZoneUpdated?.Invoke();
        return true;
    }
    
    // 移动卡牌到新位置
    public bool MoveCardToPosition(CardEntity card, bool newFrontRow, int newPositionIndex)
    {
        CardPosition currentPosition = GetCardPosition(card);
        CardPosition targetPosition = FindPosition(newFrontRow, newPositionIndex);
        
        if (currentPosition == null || targetPosition == null)
            return false;
            
        if (targetPosition.isOccupied)
            return false;
            
        // 移动卡牌
        currentPosition.isOccupied = false;
        currentPosition.occupiedCard = null;
        
        targetPosition.isOccupied = true;
        targetPosition.occupiedCard = card;
        
        // 更新位置
        if (targetPosition.positionTransform != null)
        {
            card.transform.position = targetPosition.positionTransform.position;
        }
        
        onZoneUpdated?.Invoke();
        return true;
    }
    
    // 在编辑器中可视化位置点
    private void OnDrawGizmosSelected()
    {
        if (_cardPositions == null || _cardPositions.Count == 0)
            InitializePositions();
            
        Gizmos.color = Color.blue;
        
        // 绘制前排位置
        foreach (var position in GetFrontRowPositions())
        {
            Vector3 pos;
            if (position.positionTransform != null)
            {
                pos = position.positionTransform.position;
            }
            else
            {
                pos = transform.position + new Vector3(position.positionIndex - 0.5f, 0, 0);
            }
            
            Gizmos.DrawWireSphere(pos, 0.3f);
            if (position.isOccupied)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(pos, 0.2f);
                Gizmos.color = Color.blue;
            }
        }
        
        Gizmos.color = Color.green;
        
        // 绘制后排位置
        foreach (var position in GetBackRowPositions())
        {
            Vector3 pos;
            if (position.positionTransform != null)
            {
                pos = position.positionTransform.position;
            }
            else
            {
                pos = transform.position + new Vector3(position.positionIndex - 1, 0, -1);
            }
            
            Gizmos.DrawWireSphere(pos, 0.3f);
            if (position.isOccupied)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(pos, 0.2f);
                Gizmos.color = Color.green;
            }
        }
    }
    
    
}