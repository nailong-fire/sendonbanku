// GameManager.cs 的修改/扩展版本
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 新增：回合阶段枚举
    public enum TurnPhase
    {
        PlayerAction,      // 玩家行动阶段
        EnemyAction,       // 敌人行动阶段
        CardActionPhase,   // 卡牌行动阶段
        TurnEnd,           // 回合结束阶段
        GameOver           // 游戏结束
    }
    
    [Header("游戏引用")]
    public PlayerController player;
    public EnemyController enemy;
    public CardZone playerBattlefield;
    public CardZone enemyBattlefield;

    [Header("游戏设置")]
    public GameInitializer.GameSettings settings;
    
    [Header("回合状态")]
    public int currentTurn = 0;
    public TurnPhase currentPhase = TurnPhase.PlayerAction;
    public bool isPlayerTurn = true;
    public float turnTimer = 0f;
    public bool isGameOver = false;
    
    [Header("阶段设置")]
    public float playerActionTime = 300f;   // 玩家行动时间
    public float enemyActionTime = 150f;    // 敌人行动时间
    public float cardActionDelay = 1f;     // 卡牌行动间隔

    // 新增：卡牌行动队列
    private List<CardEntity> actionQueue = new List<CardEntity>();
    private int currentActionIndex = 0;
    
    // 单例
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    
    // 事件（新增阶段事件）
    public System.Action<TurnPhase> OnPhaseChange;          // 阶段变化
    public System.Action<CardEntity> OnCardAction;          // 卡牌行动
    public System.Action<List<CardEntity>> OnActionQueueReady; // 行动队列准备好
    
    // 原有事件
    public System.Action OnPlayerTurnStart;
    public System.Action OnEnemyTurnStart;
    public System.Action OnTurnEnd;
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
        }
    }

    public void SetPlayers(PlayerController playerController, EnemyController enemyController)
    {
        player = playerController;
        enemy = enemyController;
        
        playerBattlefield = player.battlefield;
        enemyBattlefield = enemy.battlefield;
    }
    
    // 开始游戏
    public void StartGame()
    {
        if (isGameOver) return;
        if(currentTurn != 0)
        {
            Debug.LogWarning("游戏已经开始，不能重复开始！");
            return;
        }

        Debug.Log("=== 游戏开始 ===");
        currentTurn = 1;
        currentPhase = TurnPhase.PlayerAction;
        
        // 开始玩家行动阶段
        //StartPlayerActionPhase();
        StartEnemyActionPhase();
    }
    
    // 开始玩家行动阶段
    public void StartPlayerActionPhase()
    {
        if (isGameOver) return;
        
        currentPhase = TurnPhase.PlayerAction;
        turnTimer = playerActionTime;
        
        Debug.Log($"=== 第{currentTurn}回合 - 玩家行动阶段 ===");
        
        // 触发事件
        OnPhaseChange?.Invoke(TurnPhase.PlayerAction);
        OnPlayerTurnStart?.Invoke();
        
        // 启动计时器
        if (playerActionTime > 0)
        {
            StartCoroutine(PlayerActionTimer());
        }
    }
    
    // 玩家行动计时器
    private IEnumerator PlayerActionTimer()
    {
        while (turnTimer > 0 && currentPhase == TurnPhase.PlayerAction && !isGameOver)
        {
            turnTimer -= Time.deltaTime;
            yield return null;
        }
        
        // 时间到，自动结束玩家行动
        if (turnTimer <= 0 && currentPhase == TurnPhase.PlayerAction && !isGameOver)
        {
            Debug.Log("玩家行动时间到！");
            EndPlayerActionPhase();
        }
    }
    
    // 结束玩家行动阶段
    public void EndPlayerActionPhase()
    {
        if (currentPhase != TurnPhase.PlayerAction || isGameOver) return;
        
        Debug.Log("玩家行动阶段结束");
        
        // 切换到敌人行动阶段
        //StartEnemyActionPhase();
        StartCardActionPhase();
    }
    
    // 开始敌人行动阶段
    public void StartEnemyActionPhase()
    {
        if (isGameOver) return;
        
        currentPhase = TurnPhase.EnemyAction;
        turnTimer = enemyActionTime;
        
        Debug.Log($"=== 第{currentTurn}回合 - 敌人行动阶段 ===");
        
        // 触发事件
        OnPhaseChange?.Invoke(TurnPhase.EnemyAction);
        OnEnemyTurnStart?.Invoke();
        
        // 敌人AI自动行动
        StartCoroutine(EnemyAIAction());
    }
    
    // 敌人AI行动协程
    private IEnumerator EnemyAIAction()
    {
        Debug.Log("敌人AI开始行动...");
        
        // 敌人出牌逻辑（简单示例）
        if(enemy.handZone.GetCardCount() != 0)
        {
            Debug.Log("敌人出牌");
            CardEntity card = null;
            bool isFrontRow = true;
            while(card == null)
            {
                card = enemy.handZone.GetCardAtPosition(true, UnityEngine.Random.Range(0, 5));
                yield return new WaitForSeconds(0.05f);
            }

            if(UnityEngine.Random.Range(0, 2) == 0)
            {
                isFrontRow = false;
            }
            yield return new WaitForSeconds(0.05f);

            while(!enemy.battlefield.PlaceCardAtPosition(card, isFrontRow, isFrontRow?UnityEngine.Random.Range(0, 3):UnityEngine.Random.Range(0, 2), enemy.handZone))
            {
                if(UnityEngine.Random.Range(0, 2) == 0)
                {
                    isFrontRow = false;
                }
                else
                {
                    isFrontRow = true;
                }
                yield return new WaitForSeconds(0.05f);
            }
            // 等待一点时间，然后结束敌人阶段
            yield return new WaitForSeconds(1f);
        }
        
        EndEnemyActionPhase();
    }
    
    // 结束敌人行动阶段
    public void EndEnemyActionPhase()
    {
        if (currentPhase != TurnPhase.EnemyAction || isGameOver) return;
        
        Debug.Log("敌人行动阶段结束");
        
        // 切换到卡牌行动阶段
        //StartCardActionPhase();
        StartPlayerActionPhase();
    }
    
    // 开始卡牌行动阶段
    public void StartCardActionPhase()
    {
        if (isGameOver) return;
        
        currentPhase = TurnPhase.CardActionPhase;
        
        Debug.Log($"=== 第{currentTurn}回合 - 卡牌行动阶段 ===");
        
        // 触发事件
        OnPhaseChange?.Invoke(TurnPhase.CardActionPhase);
        
        // 准备行动队列
        PrepareActionQueue();
        
        // 开始卡牌行动
        StartCoroutine(ExecuteCardActions());
        StartPlayerActionPhase();
    }
    
    // 准备行动队列（按速度排序）
    private void PrepareActionQueue()
    {
        actionQueue.Clear();
        currentActionIndex = 0;
        
        // 收集所有场上卡牌
        List<CardEntity> allBattlefieldCards = new List<CardEntity>();
        
        if (playerBattlefield != null)
        {
            allBattlefieldCards.AddRange(playerBattlefield.GetAllCards());
        }
        
        if (enemyBattlefield != null)
        {
            allBattlefieldCards.AddRange(enemyBattlefield.GetAllCards());
        }
        
        Debug.Log($"场上共有 {allBattlefieldCards.Count} 张卡牌");
        
        // 按速度排序（速度高的先行动）
        actionQueue = allBattlefieldCards;
        actionQueue.Sort((a, b) => 
        {
            int speedCompare = b.CardData.Speed.CompareTo(a.CardData.Speed);
            if (speedCompare == 0)
            {
                // 如果速度相同，随机排序
                return Random.Range(0, 2) == 0 ? -1 : 1;
            }
            return speedCompare;
        });

        // 触发队列准备好事件
        OnActionQueueReady?.Invoke(actionQueue);
    }
    
    // 执行卡牌行动
    private IEnumerator ExecuteCardActions()
    {
        Debug.Log("开始卡牌行动...");
        
        for (currentActionIndex = 0; currentActionIndex < actionQueue.Count; currentActionIndex++)
        {
            CardEntity card = actionQueue[currentActionIndex];
            
            if (card == null || !card.CardData.IsAlive || card.HasActedThisTurn) continue;
            
            Debug.Log($"[行动 {currentActionIndex + 1}/{actionQueue.Count}] " +
                     $"{card.CardData.CardName} (速度: {card.CardData.Speed}) 行动");
            
            // 触发卡牌行动事件
            OnCardAction?.Invoke(card);
            
            // 执行卡牌行动逻辑
            yield return StartCoroutine(PerformCardAction(card));
            
            // 标记卡牌已行动
            card.HasActedThisTurn = true;
            
            // 卡牌行动间隔
            yield return new WaitForSeconds(cardActionDelay);
            
            // 检查是否有卡牌死亡
            RemoveDeadCards();
            
            // 检查游戏是否结束
            if (isGameOver) break;
        }
        
        Debug.Log("卡牌行动阶段结束");
        
        // 切换到回合结束阶段
        StartTurnEndPhase();
    }
    
    // 执行单张卡牌行动
    private IEnumerator PerformCardAction(CardEntity card)
    {
        // 这里调用卡牌自身的行动逻辑
        if (card.HasActionAbility)
        {
            if(card.CardData.HasEffect(SpecialEffect.MeleeAttack))
            {
                StartCoroutine(PerformMeleeAttack(card));
            }
            if(card.CardData.HasEffect(SpecialEffect.RangedAttack))
            {
                StartCoroutine(PerformRangedAttack(card));
            }
            if(card.CardData.HasEffect(SpecialEffect.Healer))
            {
                StartCoroutine(PerformHealAction(card));
            }
            if(card.CardData.HasEffect(SpecialEffect.MeleeAreaAttack))
            {
                StartCoroutine(PerformMeleeAreaAttack(card));
            }
            if(card.CardData.HasEffect(SpecialEffect.RangedAreaAttack))
            {
                StartCoroutine(PerformRangeAreaAttack(card));
            }
            if(card.CardData.HasEffect(SpecialEffect.AllAreaAttack))
            {
                StartCoroutine(PerformAllAreaAttack(card));
            }
            if(card.CardData.HasEffect(SpecialEffect.Guardian))
            {
                StartCoroutine(PerformGuardAction(card));
            }
        }
        yield return null;
    }
// =============================================================================================
//      ================================= 卡牌行动具体实现 =================================
// =============================================================================================
    // 近战攻击行动
    private IEnumerator PerformMeleeAttack(CardEntity attacker)
    {
        // 确定攻击目标
        CardEntity target = FindMeleeAttackTarget(attacker);
        
        if (target != null)
        {
            Debug.Log($"{attacker.CardData.CardName} 攻击 {target.CardData.CardName}");
            
            // 计算伤害
            int damage = CalculateDamage(attacker, target);
            
            // 应用伤害
            target.TakeDamage(damage);
            
            yield return new WaitForSeconds(0.5f);
        }

    }

    // 查找攻击目标
    private CardEntity FindMeleeAttackTarget(CardEntity attacker)
    {
        // 确定攻击方是玩家还是敌人
        bool isAttackerPlayer = attacker.Owner == player;
        
        // 获取对方战场
        CardZone opponentBattlefield = isAttackerPlayer ? enemyBattlefield : playerBattlefield;
        
        if (opponentBattlefield == null || opponentBattlefield.GetAllCards().Count == 0)
            return null;
        
        List<CardEntity> targets = new List<CardEntity>(opponentBattlefield.GetAllCards());
        
        // 移除已经死亡的卡牌
        targets.RemoveAll(card => !card.CardData.IsAlive);
        
        if (targets.Count == 0) return null;

        CardEntity target = null;

        if(attacker.IsInFrontRow)
        {
            if(attacker.positionindex == 0)
            {
                target = targets.Find(card => !card.IsInFrontRow && card.positionindex == 1);
                if(target != null)
                return target;

                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 2);
                if(target != null)
                return target;
            }
            if(attacker.positionindex == 1)
            {
                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 2);
                if(target != null)
                return target;
                
                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 1);
                if(target != null)
                return target;

                target = targets.Find(card => !card.IsInFrontRow && card.positionindex == 0);
                if(target != null)
                return target;

                target = targets.Find(card => !card.IsInFrontRow && card.positionindex == 1);
                if(target != null)
                return target;
            }
            if(attacker.positionindex == 2)
            {
                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 2);
                if(target != null)
                return target;

                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 1);
                if(target != null)
                return target;
                
                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 0);
                if(target != null)
                return target;

                target = targets.Find(card => !card.IsInFrontRow && card.positionindex == 0);
                if(target != null)
                return target;

                target = targets.Find(card => !card.IsInFrontRow && card.positionindex == 1);
                if(target != null)
                return target;
            }
        }

        return target;
    }
    
    // 治疗行动
    private IEnumerator PerformHealAction(CardEntity healer)
    {
        // 确定治疗目标
        CardEntity target = FindHealTarget(healer);
        
        if (target != null)
        {
            Debug.Log($"{healer.CardData.CardName} 治疗 {target.CardData.CardName}");
            
            // 计算治疗量
            int healAmount = healer.CardData.Power; // 示例：使用卡牌的力量作为治疗量
            
            // 应用治疗
            target.Heal(healAmount);
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    // 查找治疗目标
    private CardEntity FindHealTarget(CardEntity healer)
    {
        // 确定治疗方是玩家还是敌人
        bool isHealerPlayer = healer.Owner == player;
        
        // 获取己方战场
        CardZone allyBattlefield = isHealerPlayer ? playerBattlefield : enemyBattlefield;
        
        if (allyBattlefield == null || allyBattlefield.GetAllCards().Count == 0)
            return null;
        
        List<CardEntity> targets = new List<CardEntity>(allyBattlefield.GetAllCards());
        
        // 移除已经满血的卡牌
        targets.RemoveAll(card => card.CardData.CurrentHealth >= card.CardData.MaxHealth);
        
        if (targets.Count == 0) return null;

        CardEntity target = null;
        
        if(!healer.IsInFrontRow)
        {
            if(healer.positionindex == 0)
            {
                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 1);
                if(target != null)
                return target;

                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 2);
                if(target != null)
                return target;
            }
            if(healer.positionindex == 1)
            {
                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 2);
                if(target != null)
                return target;
                
                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 1);
                if(target != null)
                return target;
            }
        }
        
        return target;
    }

    // 远程攻击行动
    private IEnumerator PerformRangedAttack(CardEntity attacker)
    {
        // 确定攻击目标
        CardEntity target = FindRangedAttackTarget(attacker);
        
        if (target != null)
        {
            Debug.Log($"{attacker.CardData.CardName} 远程攻击 {target.CardData.CardName}");
            
            // 计算伤害
            int damage = CalculateDamage(attacker, target);
            
            // 应用伤害
            target.TakeDamage(damage);
            
            yield return new WaitForSeconds(0.5f);
        }
    }
    // 查找远程攻击目标
    private CardEntity FindRangedAttackTarget(CardEntity attacker)
    {
        // 确定攻击方是玩家还是敌人
        bool isAttackerPlayer = attacker.Owner == player;
        
        // 获取对方战场
        CardZone opponentBattlefield = isAttackerPlayer ? enemyBattlefield : playerBattlefield;
        
        if (opponentBattlefield == null || opponentBattlefield.GetAllCards().Count == 0)
            return null;
        
        List<CardEntity> targets = new List<CardEntity>(opponentBattlefield.GetAllCards());
        
        // 移除已经死亡的卡牌
        targets.RemoveAll(card => !card.CardData.IsAlive);
        
        if (targets.Count == 0) return null;

        
        CardEntity target = null;

        if(!attacker.IsInFrontRow)
        {
            if(attacker.positionindex == 0)
            {
                target = targets.Find(card => !card.IsInFrontRow && card.positionindex == 1);
                if(target != null)
                return target;

                target = targets.Find(card => !card.IsInFrontRow && card.positionindex == 0);
                if(target != null)
                return target;

                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 1);
                if(target != null)
                return target;

                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 2);
                if(target != null)
                return target;
            }
            if(attacker.positionindex == 1)
            {
                target = targets.Find(card => !card.IsInFrontRow && card.positionindex == 1);
                if(target != null)
                return target;
                
                target = targets.Find(card => !card.IsInFrontRow && card.positionindex == 0);
                if(target != null)
                return target;

                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 0);
                if(target != null)
                return target;

                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 1);
                if(target != null)
                return target;

                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 2);
                if(target != null)
                return target;
            }
        }

        return target;
        
    }

    // 近战范围攻击行动
    private IEnumerator PerformMeleeAreaAttack(CardEntity attacker)
    {
        // 确定攻击目标
        // 确定攻击方是玩家还是敌人

        List<CardEntity> targets = new List<CardEntity>();
        bool isAttackerPlayer = attacker.Owner == player;
        
        // 获取对方战场
        CardZone opponentBattlefield = isAttackerPlayer ? enemyBattlefield : playerBattlefield;
        
        if (opponentBattlefield == null || opponentBattlefield.GetAllCards().Count == 0)
            targets = null;
        else
            targets = opponentBattlefield.GetAllCards();
        
        // 移除已经死亡的卡牌
        targets.RemoveAll(card => !card.CardData.IsAlive);
        
        if (targets != null)
        {
            foreach(CardEntity target in targets)
            {
                if(target.IsInFrontRow)
                {
                    Debug.Log($"{attacker.CardData.CardName} 近战范围攻击 {target.CardData.CardName}");
                
                    // 计算伤害
                    int damage = CalculateDamage(attacker, target);
                
                    // 应用伤害
                    target.TakeDamage(damage);
                }
                yield return new WaitForSeconds(0.5f);
            }         
        }
    }

    // 远程范围攻击行动
    private IEnumerator PerformRangeAreaAttack(CardEntity attacker)
    {
        // 确定攻击目标
        // 确定攻击方是玩家还是敌人

        List<CardEntity> targets = new List<CardEntity>();
        bool isAttackerPlayer = attacker.Owner == player;
        
        // 获取对方战场
        CardZone opponentBattlefield = isAttackerPlayer ? enemyBattlefield : playerBattlefield;
        
        if (opponentBattlefield == null || opponentBattlefield.GetAllCards().Count == 0)
            targets = null;
        else
            targets = opponentBattlefield.GetAllCards();
        
        // 移除已经死亡的卡牌
        targets.RemoveAll(card => !card.CardData.IsAlive);
        
        if (targets != null)
        {
            foreach(CardEntity target in targets)
            {
                if(!target.IsInFrontRow)
                {
                    Debug.Log($"{attacker.CardData.CardName} 远程范围攻击 {target.CardData.CardName}");
                
                    // 计算伤害
                    int damage = CalculateDamage(attacker, target);
                
                    // 应用伤害
                    target.TakeDamage(damage);
                }
                yield return new WaitForSeconds(0.5f);
            }         
        }
    }

    // 全体范围攻击行动
    private IEnumerator PerformAllAreaAttack(CardEntity attacker)
    {
        // 确定攻击目标
        // 确定攻击方是玩家还是敌人

        List<CardEntity> targets = new List<CardEntity>();
        bool isAttackerPlayer = attacker.Owner == player;
        
        // 获取对方战场
        CardZone opponentBattlefield = isAttackerPlayer ? enemyBattlefield : playerBattlefield;
        
        if (opponentBattlefield == null || opponentBattlefield.GetAllCards().Count == 0)
            targets = null;
        else
            targets = opponentBattlefield.GetAllCards();
        
        // 移除已经死亡的卡牌
        targets.RemoveAll(card => !card.CardData.IsAlive);
        
        if (targets != null)
        {
            foreach(CardEntity target in targets)
            {
                Debug.Log($"{attacker.CardData.CardName} 全体范围攻击 {target.CardData.CardName}");
                
                // 计算伤害
                int damage = CalculateDamage(attacker, target);
                
                // 应用伤害
                target.TakeDamage(damage);
                
                yield return new WaitForSeconds(0.5f);
            }         
        }         
        
    }

    // 保护行动
    private IEnumerator PerformGuardAction(CardEntity guardian)
    {
        guardian.guardedCard.guardian = null;
        guardian.guardedCard.isguarded = false;

        CardEntity target = FindGuardTarget(guardian);
        if (target != null)
        {
            Debug.Log($"{guardian.CardData.CardName} 保护 {target.CardData.CardName}");
            guardian.guardedCard = target;
            target.isguarded = true;
            target.guardian = guardian;
        
            yield return new WaitForSeconds(0.5f);
        }
    }

    // 寻找保护目标
    private CardEntity FindGuardTarget(CardEntity guardian)
    {
        // 确定保护方是玩家还是敌人
        bool isGuardianPlayer = guardian.Owner == player;
        
        // 获取己方战场
        CardZone allyBattlefield = isGuardianPlayer ? playerBattlefield : enemyBattlefield;
        
        if (allyBattlefield == null || allyBattlefield.GetAllCards().Count == 0)
            return null;
        
        List<CardEntity> targets = new List<CardEntity>(allyBattlefield.GetAllCards());
        
        if (targets.Count == 0) return null;

        CardEntity target = null;
        
        if(guardian.IsInFrontRow)
        {
            if(guardian.positionindex == 1)
            {
                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 0);
                if(target != null)
                return target;

                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 1);
                if(target != null)
                return target;
            }
            if(guardian.positionindex == 2)
            {
                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 1);
                if(target != null)
                return target;
                
                target = targets.Find(card => card.IsInFrontRow && card.positionindex == 0);
                if(target != null)
                return target;
            }
        }
        
        return target;
    }
// =============================================================================================
// =============================================================================================
// =============================================================================================

    // 计算伤害
    private int CalculateDamage(CardEntity attacker, CardEntity defender)
    {
        int attackPower = attacker.CardData.Power;

        int damage = attackPower;

        if (defender.isguarded)
        {
            int guardeddamage = damage - damage / 2;
            damage /= guardeddamage;
            defender.guardian.TakeDamage(guardeddamage);
        }
        
        return damage;
    }
    
    // 移除死亡的卡牌
    private void RemoveDeadCards()
    {
        List<CardEntity> cardsToRemove = new List<CardEntity>();
        
        // 检查玩家战场
        if (playerBattlefield != null)
        {
            foreach (CardEntity card in playerBattlefield.GetAllCards())
            {
                if (card != null && !card.CardData.IsAlive)
                {
                    cardsToRemove.Add(card);
                }
            }
            
            foreach (CardEntity deadCard in cardsToRemove)
            {
                Debug.Log(player.name + "的" + deadCard.name + "损坏");
                playerBattlefield.RemoveCard(deadCard);
                Destroy(deadCard.gameObject);
            }
        }
        
        cardsToRemove.Clear();
        
        // 检查敌人战场
        if (enemyBattlefield != null)
        {
            foreach (CardEntity card in enemyBattlefield.GetAllCards())
            {
                if (card != null && !card.CardData.IsAlive)
                {
                    cardsToRemove.Add(card);
                }
            }
            
            foreach (CardEntity deadCard in cardsToRemove)
            {
                Debug.Log(enemy.name + "的" + deadCard.name + "损坏");
                enemyBattlefield.RemoveCard(deadCard);
                Destroy(deadCard.gameObject);
            }
        }
    }
    
    // 开始回合结束阶段
    public void StartTurnEndPhase()
    {
        if (isGameOver) return;
        
        currentPhase = TurnPhase.TurnEnd;
        
        Debug.Log($"=== 第{currentTurn}回合结束 ===");
        
        // 触发事件
        OnPhaseChange?.Invoke(TurnPhase.TurnEnd);
        OnTurnEnd?.Invoke();
        
        // 执行回合结束逻辑
        ExecuteTurnEndEffects();
        
        // 检查游戏结束条件
        CheckGameEndCondition();
        
        if (!isGameOver)
        {
            // 准备下一回合
            StartCoroutine(PrepareNextTurn());
        }
    }
    
    // 执行回合结束效果
    private void ExecuteTurnEndEffects()
    {
        // 重置卡牌行动状态
        ResetCardActionStates();
        
        // 处理持续效果（如中毒、灼烧等）
        ProcessContinuousEffects();
        
        // 更新资源（如每回合恢复Faith等）
        if(currentTurn >= 1)
            UpdateResources();
    }
    
    // 重置卡牌行动状态
    private void ResetCardActionStates()
    {
        // 重置所有卡牌的"已行动"状态
        foreach (CardEntity card in actionQueue)
        {
            if (card != null)
            {
                card.HasActedThisTurn = false;
                card.HasActionAbility = true;
            }
        }
    }
    
    // 处理持续效果
    private void ProcessContinuousEffects()
    {   
        // 处理场上卡牌的持续效果
        List<CardEntity> allCards = new List<CardEntity>();
        if (playerBattlefield != null) allCards.AddRange(playerBattlefield.GetAllCards());
        if (enemyBattlefield != null) allCards.AddRange(enemyBattlefield.GetAllCards());
        
        foreach (CardEntity card in allCards)
        {
            if (card != null && card.CardData.IsAlive)
            {
                card.ProcessEndTurnEffects();
            }
        }
    }
    
    // 更新资源
    private void UpdateResources()
    {
        // 示例：每回合恢复少量Faith
        if (player != null && player.resourceSystem != null)
        {
            player.resourceSystem.CurrentFaith += player.resourceSystem.CalculateFaithGain();
            Debug.Log("玩家恢复Faith");
            
            if(player.battlefield.GetCardCount() == 0)
            {
                player.resourceSystem.CurrentHope -= 3;
            }
        }
        
        if (enemy != null && enemy.resourceSystem != null)
        {
            enemy.resourceSystem.CurrentFaith += enemy.resourceSystem.CalculateFaithGain();
            Debug.Log("敌人恢复Faith");
            
            if(enemy.battlefield.GetCardCount() == 0)
            {
                enemy.resourceSystem.CurrentHope -= 3;
            }
        }
    }
    
    // 准备下一回合
    private IEnumerator PrepareNextTurn()
    {
        yield return new WaitForSeconds(1f); // 给玩家一点时间看回合结束信息
        
        // 增加回合数
        currentTurn++;
        
        // 抽卡（如果规则需要）
        if (player != null)
        {
            player.DrawCard();
        }
        
        if (enemy != null)
        {
            enemy.DrawCard();
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // 开始下一回合的玩家行动阶段
        StartEnemyActionPhase();
    }
    
    // 检查游戏结束条件（修改版）
    private void CheckGameEndCondition()
    {
        if (player == null || enemy == null) return;
        
        // 检查玩家Hope值
        if (player.resourceSystem.CurrentHope <= 0)
        {
            EndGame(false, "玩家Hope值归零");
            return;
        }
        
        // 检查敌人Hope值
        if (enemy.resourceSystem.CurrentHope <= 0)
        {
            EndGame(true, "敌人Hope值归零");
            return;
        }
        
        // 检查回合限制（如果有）
        if (currentTurn > 100) // 可以配置
        {
            EndGameByTurnLimit();
            return;
        }
    }
    
    // 回合限制结束游戏
    private void EndGameByTurnLimit()
    {
        if (player == null || enemy == null) return;
        
        int playerHope = player.resourceSystem.CurrentHope;
        int enemyHope = enemy.resourceSystem.CurrentHope;
        
        if (playerHope > enemyHope)
        {
            EndGame(true, $"回合限制到达，玩家Hope值更高 ({playerHope} vs {enemyHope})");
        }
        else if (enemyHope > playerHope)
        {
            EndGame(false, $"回合限制到达，敌人Hope值更高 ({enemyHope} vs {playerHope})");
        }
        else
        {
            EndGame(false, $"回合限制到达，Hope值相同 ({playerHope}) - 判敌人胜利");
        }
    }
    
    // 结束游戏
    private void EndGame(bool playerWon, string reason)
    {
        if (isGameOver) return;
        
        isGameOver = true;
        currentPhase = TurnPhase.GameOver;
        
        Debug.Log($"=== 游戏结束 ===");
        Debug.Log($"{(playerWon ? "玩家胜利！" : "敌人胜利！")}");
        Debug.Log($"原因: {reason}");
        Debug.Log($"总回合数: {currentTurn}");
        Debug.Log($"玩家剩余Hope: {player?.resourceSystem.CurrentHope ?? 0}");
        Debug.Log($"敌人剩余Hope: {enemy?.resourceSystem.CurrentHope ?? 0}");
        
        // 触发游戏结束事件
        OnPhaseChange?.Invoke(TurnPhase.GameOver);
        OnGameOver?.Invoke(playerWon);

        // 播放胜利/失败播报UI
        var announcer = FindObjectOfType<GameResultAnnouncer>();
        if (announcer != null)
        {
            if (playerWon)
                announcer.ShowWin();
            else
                announcer.ShowLose();
        }

        Debug.Log("游戏结束，摧毁游戏管理器");
        player.cardDatabase.RestorePlayerDeckFromBackup();
        enemy.cardDatabase.RestorePlayerDeckFromBackup();
        GameInitializer gameInitializer = FindObjectOfType<GameInitializer>();
        gameInitializer.DestroyExistingObjects();
        gameInitializer.gameObject.SetActive(false);

        if (SceneTransition.Instance == null)
        {
            Debug.LogError("SceneTransition not found. Scene change aborted.");
            return;
        }
        SceneTransition.Instance.LoadScene("map");

        Destroy(gameObject);
    }
    
    // 公共方法：获取当前阶段信息
    public string GetPhaseInfo()
    {
        switch (currentPhase)
        {
            case TurnPhase.PlayerAction:
                return $"玩家行动阶段 | 剩余时间: {Mathf.CeilToInt(turnTimer)}秒";
            case TurnPhase.EnemyAction:
                return $"敌人行动阶段 | 剩余时间: {Mathf.CeilToInt(turnTimer)}秒";
            case TurnPhase.CardActionPhase:
                return $"卡牌行动阶段 ({currentActionIndex + 1}/{actionQueue.Count})";
            case TurnPhase.TurnEnd:
                return $"第{currentTurn}回合结束";
            case TurnPhase.GameOver:
                return "游戏结束";
            default:
                return "未知阶段";
        }
    }
    
    // 快捷方法：玩家手动结束行动阶段
    public void ForceEndPlayerAction()
    {
        if (currentPhase == TurnPhase.PlayerAction && !isGameOver)
        {
            EndPlayerActionPhase();
        }
    }
    
    // 快捷方法：重新开始游戏
    public void RestartGame()
    {
        // 重置状态
        currentTurn = 0;
        currentPhase = TurnPhase.PlayerAction;
        isGameOver = false;
        actionQueue.Clear();
        currentActionIndex = 0;
        
        // 重新开始游戏
        StartGame();
    }
    
    // 新增：获取行动队列（用于UI显示）
    public List<CardEntity> GetActionQueue()
    {
        return new List<CardEntity>(actionQueue);
    }
    
    // 新增：获取当前行动卡牌
    public CardEntity GetCurrentActionCard()
    {
        if (currentActionIndex >= 0 && currentActionIndex < actionQueue.Count)
        {
            return actionQueue[currentActionIndex];
        }
        return null;
    }
    
    private void Update()
    {
        // 快捷键
        if (Input.GetKeyDown(KeyCode.Space) && !isGameOver && currentTurn > 0)
        {
            if (currentPhase == TurnPhase.PlayerAction)
            {
                ForceEndPlayerAction();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndGame(false,"玩家主动退出");
        }
    }

}
