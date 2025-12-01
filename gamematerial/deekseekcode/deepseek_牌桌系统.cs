// === 4. 牌桌和位置系统 ===
public struct BoardPosition
{
    public int Row;      // 0: 前排, 1: 后排
    public int Index;    // 位置索引
    public bool IsPlayerSide;
    
    public bool IsFrontRow => Row == 0;
    public bool IsBackRow => Row == 1;
    
    public override string ToString() => 
        $"{(IsPlayerSide ? "Player" : "Enemy")} {(IsFrontRow ? "Front" : "Back")}-{Index}";
}

public class GameBoard
{
    // 3x2布局：3后排，2前排
    private const int BACK_ROW_COUNT = 3;
    private const int FRONT_ROW_COUNT = 2;
    private const int MAX_CARDS_PER_SIDE = 4;
    
    private Dictionary<BoardPosition, CardEntity> _cardPositions = new();
    private List<CardEntity> _playerCards = new();
    private List<CardEntity> _enemyCards = new();
    
    public bool TryPlaceCard(CardEntity card, BoardPosition position, Player owner)
    {
        // 检查位置有效性
        if (!IsValidPosition(position, owner, card.Data))
            return false;
            
        // 检查场上数量限制
        if (GetCardsOnField(owner).Count >= MAX_CARDS_PER_SIDE)
            return false;
            
        // 检查位置是否被占用
        if (_cardPositions.ContainsKey(position))
            return false;
            
        // 放置卡牌
        card.SetPosition(position);
        _cardPositions[position] = card;
        
        if (owner.IsPlayer)
            _playerCards.Add(card);
        else
            _enemyCards.Add(card);
            
        return true;
    }
    
    private bool IsValidPosition(BoardPosition position, Player owner, CardData cardData)
    {
        if (position.IsPlayerSide != owner.IsPlayer)
            return false;
            
        // 检查卡牌是否可以放在该行
        if (position.IsFrontRow && !cardData.CanPlaceFront)
            return false;
        if (position.IsBackRow && !cardData.CanPlaceBack)
            return false;
            
        // 检查位置索引有效性
        if (position.IsFrontRow && position.Index >= FRONT_ROW_COUNT)
            return false;
        if (position.IsBackRow && position.Index >= BACK_ROW_COUNT)
            return false;
            
        return true;
    }
    
    public List<CardEntity> GetCardsOnField(Player owner)
    {
        return owner.IsPlayer ? _playerCards : _enemyCards;
    }
    
    public List<CardEntity> GetCardsInRow(Player owner, bool frontRow)
    {
        return GetCardsOnField(owner)
            .Where(card => card.Position.IsFrontRow == frontRow)
            .ToList();
    }
    
    public void RemoveCard(CardEntity card)
    {
        _cardPositions.Remove(card.Position);
        
        if (card.Owner.IsPlayer)
            _playerCards.Remove(card);
        else
            _enemyCards.Remove(card);
    }
    
    public CardEntity GetCardAtPosition(BoardPosition position)
    {
        _cardPositions.TryGetValue(position, out var card);
        return card;
    }
    
    public bool HasFrontRowProtection(Player owner)
    {
        return GetCardsInRow(owner, frontRow: true).Count > 0;
    }
}