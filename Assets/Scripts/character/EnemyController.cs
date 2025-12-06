using System.Collections;
using UnityEngine;

public class EnemyController : UniversalController
{
    [Header("AI设置")]
    public EnemyAI aiBehavior;
    public float thinkTimeMin = 0.5f;
    public float thinkTimeMax = 1.5f;
    
    [Header("攻击策略")]
    public bool prioritizeHighThreatTargets = true;
    public bool focusOnPlayerHope = false;
    
    [Header("状态")]
    public bool isThinking = false;
    
    private PlayerController _targetPlayer;
    
    protected override void Awake()
    {
        base.Awake();
        
        characterName = "敌人";
        isPlayerControlled = false;
    }
    
    public override void Initialize(string name, bool isPlayer, int startingHope, int startingFaith)
    {
        base.Initialize("敌人", false, startingHope, startingFaith);
        
        // 查找目标玩家
        _targetPlayer = FindObjectOfType<PlayerController>();
        
        // 初始化AI
        if (aiBehavior == null)
        {
            aiBehavior = CreateDefaultAI();
        }
    }
    
    // 创建默认AI
    private EnemyAI CreateDefaultAI()
    {
        EnemyAI defaultAI = ScriptableObject.CreateInstance<EnemyAI>();
        defaultAI.thinkTime = 1.0f;
        defaultAI.maxCardsOnBoard = 4;
        defaultAI.cardPlayAggressiveness = 0.7f;
        defaultAI.attackAggressiveness = 0.8f;
        
        return defaultAI;
    }
    
    // 开始回合（敌人特有）
    public override void StartTurn()
    {
        base.StartTurn();
        
        // 开始AI思考
        StartCoroutine(AITurnCoroutine());
    }
    
    // AI回合协程
    private IEnumerator AITurnCoroutine()
    {
        isThinking = true;
        
        Debug.Log($"{characterName} 开始思考...");
        
        // 思考时间
        float thinkTime = Random.Range(thinkTimeMin, thinkTimeMax);
        yield return new WaitForSeconds(thinkTime);
        
        // 执行AI行动
        yield return StartCoroutine(ExecuteAIActions());
        
        isThinking = false;
        
        // 自动结束回合
        EndTurn();
    }
    
    // 执行AI行动
    private IEnumerator ExecuteAIActions()
    {
        // 1. 尝试打出卡牌
        yield return StartCoroutine(AITryPlayCards());
        
        // 2. 尝试攻击
        yield return StartCoroutine(AITryAttack());
        
        Debug.Log($"{characterName} AI行动完成");
    }
    
    // AI尝试打出卡牌
    private IEnumerator AITryPlayCards()
    {
        if (aiBehavior == null) yield break;
        
        // 决定是否出牌
        if (Random.value > aiBehavior.cardPlayAggressiveness) yield break;
        
        // 如果场上卡牌已达上限，不出牌
        if (GetBattlefieldCount() >= aiBehavior.maxCardsOnBoard) yield break;
        
        // 如果有足够Faith，尝试出牌
        if (resourceSystem.CurrentFaith < aiBehavior.minFaithToPlayCard) yield break;
        
        // 选择要打出的卡牌
        CardEntity cardToPlay = AISelectCardToPlay();
        if (cardToPlay == null) yield break;
        
        // 尝试自动打出
        bool played = TryPlayCard(cardToPlay);
        if (played)
        {
            Debug.Log($"{characterName} 打出卡牌: {cardToPlay.CardData.CardName}");
            yield return new WaitForSeconds(0.3f); // 出牌间隔
            
            // 可能再出一张
            if (resourceSystem.CurrentFaith >= aiBehavior.minFaithToPlayCard && 
                GetBattlefieldCount() < aiBehavior.maxCardsOnBoard)
            {
                yield return StartCoroutine(AITryPlayCards());
            }
        }
    }
    
    // AI选择要打出的卡牌
    private CardEntity AISelectCardToPlay()
    {
        var handCards = GetHandCards();
        if (handCards.Count == 0) return null;
        
        // 过滤出可以打出的卡牌
        var playableCards = new System.Collections.Generic.List<CardEntity>();
        foreach (var card in handCards)
        {
            if (CanPlayCard(card))
            {
                playableCards.Add(card);
            }
        }
        
        if (playableCards.Count == 0) return null;
        
        // AI策略选择
        if (aiBehavior.prioritizeHighCostCards)
        {
            // 优先高消耗卡牌
            playableCards.Sort((a, b) => 
                b.CardData.FaithCost.CompareTo(a.CardData.FaithCost));
        }
        else
        {
            // 优先低消耗卡牌
            playableCards.Sort((a, b) => 
                a.CardData.FaithCost.CompareTo(b.CardData.FaithCost));
        }
        
        // 根据AI侵略性随机选择
        if (Random.value < 0.3f) // 30%几率随机选择
        {
            return playableCards[Random.Range(0, playableCards.Count)];
        }
        
        return playableCards[0];
    }
    
    // AI尝试攻击
    private IEnumerator AITryAttack()
    {
        if (_targetPlayer == null || aiBehavior == null) yield break;
        
        // 决定是否攻击
        if (Random.value > aiBehavior.attackAggressiveness) yield break;
        
        var attackers = GetBattlefieldCards();
        if (attackers.Count == 0) yield break;
        
        var targetBattlefield = _targetPlayer.battlefield;
        if (targetBattlefield == null) yield break;
        
        // 按顺序攻击
        foreach (var attacker in attackers)
        {
            if (!attacker.CardData.IsAlive) continue;
            
            // 寻找攻击目标
            CardEntity target = AISelectAttackTarget(attacker, targetBattlefield);
            if (target == null) continue;
            
            // 执行攻击
            yield return StartCoroutine(AIExecuteAttack(attacker, target));
            
            // 攻击间隔
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    // AI选择攻击目标
    private CardEntity AISelectAttackTarget(CardEntity attacker, CardZone targetBattlefield)
    {
        if (attacker == null || targetBattlefield == null) return null;
        
        // 获取可攻击的目标
        var attackableTargets = targetBattlefield.GetAttackableTargets(
            attacker,
            attacker.CardData.HasEffect(SpecialEffect.RangedAttack)
        );
        
        if (attackableTargets.Count == 0) return null;
        
        // AI策略选择目标
        System.Collections.Generic.List<CardEntity> sortedTargets = 
            new System.Collections.Generic.List<CardEntity>(attackableTargets);
        
        if (aiBehavior.targetLowHealthFirst)
        {
            // 优先低生命值目标
            sortedTargets.Sort((a, b) => 
                a.CardData.CurrentHealth.CompareTo(b.CardData.CurrentHealth));
        }
        
        // 优先前排目标（如果攻击者不是远程）
        if (aiBehavior.targetFrontRowFirst && !attacker.CardData.HasEffect(SpecialEffect.RangedAttack))
        {
            // 这里可以添加前排优先逻辑
        }
        
        // 根据策略随机选择
        if (Random.value < 0.2f) // 20%几率随机选择
        {
            return sortedTargets[Random.Range(0, sortedTargets.Count)];
        }
        
        return sortedTargets[0];
    }
    
    // AI执行攻击
    private IEnumerator AIExecuteAttack(CardEntity attacker, CardEntity target)
    {
        if (attacker == null || target == null) yield break;
        
        Debug.Log($"{characterName} 攻击: {attacker.CardData.CardName} → {target.CardData.CardName}");
        
        // 攻击动画
        Vector3 originalPos = attacker.transform.position;
        Vector3 targetPos = target.transform.position;
        
        float duration = 0.2f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            attacker.transform.position = Vector3.Lerp(originalPos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 造成伤害
        int damage = attacker.GetAttackPower();
        target.TakeDamage(damage);
        
        // 返回原位
        elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            attacker.transform.position = Vector3.Lerp(targetPos, originalPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        attacker.transform.position = originalPos;
    }
    
    // 查找最佳位置（敌人特有策略）
    protected override CardZone.CardPosition FindBestPositionForCard(CardEntity card)
    {
        if (battlefield == null) return null;
        
        // 敌人策略：优先放置在后排（保护自己）
        var backRowPositions = battlefield.GetEmptyPositions(false);
        foreach (var position in backRowPositions)
        {
            if (battlefield.CanPlaceCardAtPosition(card, false))
            {
                return position;
            }
        }
        
        // 其次前排
        var frontRowPositions = battlefield.GetEmptyPositions(true);
        foreach (var position in frontRowPositions)
        {
            if (battlefield.CanPlaceCardAtPosition(card, true))
            {
                return position;
            }
        }
        
        return null;
    }
}