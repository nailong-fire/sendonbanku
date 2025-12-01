// === 5. 回合系统 ===
public enum TurnPhase
{
    PlayerAction,
    EnemyAction,
    CardActions,
    EndTurn
}

public class TurnSystem : MonoBehaviour
{
    private TurnPhase _currentPhase = TurnPhase.PlayerAction;
    private bool _isPlayerTurn = true;
    
    private List<CardEntity> _actionQueue = new();
    
    public event Action<TurnPhase> OnPhaseChanged;
    public event Action<bool> OnTurnChanged;
    
    public void StartPlayerTurn()
    {
        _isPlayerTurn = true;
        _currentPhase = TurnPhase.PlayerAction;
        OnTurnChanged?.Invoke(true);
        OnPhaseChanged?.Invoke(_currentPhase);
    }
    
    public void EndPlayerActionPhase()
    {
        _currentPhase = TurnPhase.EnemyAction;
        OnPhaseChanged?.Invoke(_currentPhase);
        // 这里会触发AI行动
    }
    
    public void EndEnemyActionPhase()
    {
        _currentPhase = TurnPhase.CardActions;
        OnPhaseChanged?.Invoke(_currentPhase);
        
        // 构建行动队列（按速度排序）
        BuildActionQueue();
        ProcessCardActions();
    }
    
    private void BuildActionQueue()
    {
        _actionQueue.Clear();
        
        // 获取场上所有卡牌并按速度排序（速度值小的先行动）
        var allCards = FindObjectsOfType<CardEntity>();
        _actionQueue = allCards
            .Where(card => card.Data.IsAlive)
            .OrderBy(card => card.Data.Speed)
            .ToList();
    }
    
    private void ProcessCardActions()
    {
        if (_actionQueue.Count == 0)
        {
            EndCardActionPhase();
            return;
        }
        
        var card = _actionQueue[0];
        _actionQueue.RemoveAt(0);
        
        // 执行卡牌行动
        ExecuteCardAction(card, () =>
        {
            // 递归处理下一张卡牌
            ProcessCardActions();
        });
    }
    
    private void ExecuteCardAction(CardEntity card, Action onComplete)
    {
        // 这里实现卡牌的具体行动逻辑
        // 例如：攻击、治疗等
        
        // 简单示例：攻击前方敌人
        var target = FindTargetForCard(card);
        if (target != null)
        {
            // 执行攻击
            // card.Attack(target);
        }
        
        onComplete?.Invoke();
    }
    
    private CardEntity FindTargetForCard(CardEntity card)
    {
        // 根据卡牌类型和位置寻找目标
        // 实现攻击规则（前排保护后排等）
        return null;
    }
    
    private void EndCardActionPhase()
    {
        _currentPhase = TurnPhase.EndTurn;
        OnPhaseChanged?.Invoke(_currentPhase);
        
        // 检查hope减少规则
        CheckHopeReductionRules();
        
        // 结束回合，切换到对方回合
        _isPlayerTurn = !_isPlayerTurn;
        if (_isPlayerTurn)
            StartPlayerTurn();
        else
            // 开始敌人回合
            EndPlayerActionPhase(); // 触发敌人AI
    }
    
    private void CheckHopeReductionRules()
    {
        // 实现hope减少规则
        // 1. 场上无卡牌时减少hope
        // 2. 卡牌被击败时减少hope（在CardEntity.OnDeath中处理）
    }
}