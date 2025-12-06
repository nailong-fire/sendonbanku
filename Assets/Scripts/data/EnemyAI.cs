// Assets/Scripts/AI/EnemyAI.cs
using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "EnemyAI_Default", menuName = "敌人AI配置")]
public class EnemyAI : ScriptableObject
{
    [Header("基本设置")]
    public float thinkTime = 1.0f;
    public int maxCardsOnBoard = 4;
    
    [Header("出牌策略")]
    [Range(0, 1)] public float cardPlayAggressiveness = 0.7f;
    public int minFaithToPlayCard = 2;
    public bool prioritizeHighCostCards = false;
    
    [Header("攻击策略")]
    [Range(0, 1)] public float attackAggressiveness = 0.8f;
    public bool targetLowHealthFirst = true;
    public bool targetFrontRowFirst = true;
    
    // 初始化方法（可选）
    public void Initialize(EnemyController enemy, PlayerController targetPlayer)
    {
        // 空实现，避免报错
    }
    
    // AI打出卡牌
    public IEnumerator PlayCards(EnemyController enemy)
    {
        // 空实现，避免报错
        yield break;
    }
    
    // AI攻击
    public IEnumerator Attack(EnemyController enemy)
    {
        // 空实现，避免报错
        yield break;
    }
    
    // 计算卡牌价值（用于AI决策）
    public float CalculateCardValue(CardEntity card)
    {
        // 空实现，避免报错
        return 0f;
    }
    
    // 选择要打出的卡牌（可选）
    private CardEntity SelectCardToPlay(EnemyController enemy)
    {
        // 空实现，避免报错
        return null;
    }
    
    // 选择攻击目标（可选）
    private CardEntity SelectAttackTarget(EnemyController enemy, CardEntity attacker)
    {
        // 空实现，避免报错
        return null;
    }
    
    // 执行攻击（可选）
    private IEnumerator ExecuteAttack(EnemyController enemy, CardEntity attacker, CardEntity target)
    {
        // 空实现，避免报错
        yield break;
    }
}