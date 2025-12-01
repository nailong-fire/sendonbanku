// === 6. 玩家控制器 ===
public class Player : MonoBehaviour
{
    public bool IsPlayer = true; // 区分玩家和AI
    public ResourceSystem Resources = new();
    public List<CardData> HandCards = new();
    public GameBoard Board;
    
    private int _consecutiveEmptyRounds = 0;
    
    public bool TryPlayCard(CardData cardData, BoardPosition position)
    {
        // 检查资源是否足够
        if (!Resources.CanAfford(cardData.FaithCost))
            return false;
            
        // 创建卡牌实体
        var cardEntity = InstantiateCard(cardData);
        
        // 尝试放置在牌桌上
        if (Board.TryPlaceCard(cardEntity, position, this))
        {
            // 消耗资源
            Resources.SpendFaith(cardData.FaithCost);
            
            // 从手牌移除
            HandCards.Remove(cardData);
            
            return true;
        }
        
        Destroy(cardEntity.gameObject);
        return false;
    }
    
    private CardEntity InstantiateCard(CardData cardData)
    {
        // 这里应该从Prefab实例化卡牌
        var cardObj = new GameObject($"Card_{cardData.CardName}");
        var cardEntity = cardObj.AddComponent<CardEntity>();
        cardEntity.Initialize(cardData, this);
        return cardEntity;
    }
    
    public void OnCardDestroyed(CardEntity card)
    {
        // 卡牌被摧毁时减少hope
        int hopeLoss = CalculateHopeLossFromCard(card);
        Resources.CurrentHope -= hopeLoss;
        
        // 从牌桌移除
        Board.RemoveCard(card);
        
        // 销毁游戏对象
        Destroy(card.gameObject);
    }
    
    private int CalculateHopeLossFromCard(CardEntity card)
    {
        // 根据卡牌的faith消耗决定hope减少量
        // 基础为1点，高消耗卡牌可能更多
        int baseLoss = 1;
        
        if (card.Data.FaithCost >= 5) // 假设5为高消耗阈值
            baseLoss += 1;
            
        return baseLoss;
    }
    
    public void EndTurn()
    {
        // 检查场上是否有卡牌
        bool hasCards = Board.GetCardsOnField(this).Count > 0;
        
        if (hasCards)
        {
            _consecutiveEmptyRounds = 0;
            // 场上有卡牌，增加hope
            Resources.CurrentHope += 1;
        }
        else
        {
            _consecutiveEmptyRounds++;
            // 场上无卡牌，减少hope
            int hopeLoss = _consecutiveEmptyRounds == 1 ? 2 : 4;
            Resources.CurrentHope -= hopeLoss;
        }
        
        // 回合结束获得faith
        Resources.EndTurnGainFaith();
    }
}