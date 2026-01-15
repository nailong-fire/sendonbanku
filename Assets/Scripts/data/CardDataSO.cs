using UnityEngine;
using System.Collections.Generic;

// 特殊效果枚举
public enum SpecialEffect
{
    MeleeAttack,       // 近战攻击
    RangedAttack,      // 远程攻击
    Tough,   // 减伤
    Healer,            // 治疗者
    Guardian,          // 守护
    MeleeAreaAttack,       // 近战范围攻击
    RangedAreaAttack,      // 远程范围攻击
    AllAreaAttack,        // 全体范围攻击

    Star,              // 星星
    Moon,             // 月亮
}

//持续效果枚举
public enum ContinuousEffect
{
    Poison,
    Regeneration
    
}

[CreateAssetMenu(fileName = "Card_", menuName = "卡牌/卡牌数据")]
public class CardDataSO : ScriptableObject
{
    [Header("卡牌基本信息")]
    [Tooltip("卡牌唯一ID，建议格式: card_001")]
    public string cardId = "card_001";

    [Tooltip("卡牌名称")]
    public string cardName = "示例卡牌";

    [Tooltip("卡牌描述")]
    [TextArea(2, 4)]
    public string description = "卡牌描述";

    [Header("视觉资源")]
    [Tooltip("卡牌头像或立绘")]
    public Sprite cardArt;

    [Tooltip("卡牌背景图")]
    public Sprite cardBackground;

    [Header("属性数值")]
    [Tooltip("生命值上限")]
    [Range(1, 20)]
    public int health = 5;

    [Tooltip("攻击力/效果数值")]
    [Range(0, 10)]
    public int power = 3;

    [Tooltip("速度（回合/行动顺序相关）")]
    [Range(1, 10)]
    public int speed = 5;

    [Tooltip("打出该卡所需的 Faith 值")]
    [Range(0, 10)]
    public int faithCost = 1;

    [Header("位置限制")]
    [Tooltip("是否允许放在前排")]
    public bool canPlaceFront = true;

    [Tooltip("是否允许放在后排")]
    public bool canPlaceBack = true;

    [Header("特殊效果")]
    [Tooltip("该卡包含的特殊效果列表")]
    public List<SpecialEffect> specialEffects = new List<SpecialEffect>();

    [Header("持续效果")]
    public List<ContinuousEffect> continuousEffects = new List<ContinuousEffect>();

    [Header("背景故事")]
    [Tooltip("卡牌的背景故事或描述")]
    [TextArea(3, 6)]
    public string story = "这是卡牌的背景故事示例...";

    // 创建运行时数据副本（用于游戏运行时修改，不影响原始ScriptableObject）
    public CardRuntimeData CreateRuntimeData()
    {
        return new CardRuntimeData(this);
    }

    // 编辑器校验，确保数据合理
    private void OnValidate()
    {
        // 如果没有 ID，则生成一个随机短 ID
        if (string.IsNullOrEmpty(cardId))
        {
            cardId = $"card_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        // 确保生命至少为1
        health = Mathf.Max(1, health);

        // 确保花费不为负
        faithCost = Mathf.Max(0, faithCost);
    }
}

// 运行时使用的数据副本（用于在游戏中修改而不影响SO）
[System.Serializable]
public class CardRuntimeData
{
    public string CardId;
    public string CardName;
    public int MaxHealth;
    public int CurrentHealth;
    public int Power;
    public int Speed;
    public int FaithCost;
    public List<SpecialEffect> SpecialEffects;
    public List<ContinuousEffect> ContinuousEffects;
    public bool CanPlaceFront;
    public bool CanPlaceBack;
    public Sprite CardArt;
    public string Description;

    public bool IsAlive => CurrentHealth > 0;

    public CardRuntimeData(CardDataSO so)
    {
        CardId = so.cardId;
        CardName = so.cardName;
        MaxHealth = so.health;
        CurrentHealth = so.health;
        Power = so.power;
        Speed = so.speed;
        FaithCost = so.faithCost;
        SpecialEffects = new List<SpecialEffect>(so.specialEffects);
        ContinuousEffects = new List<ContinuousEffect>(so.continuousEffects);
        CanPlaceFront = so.canPlaceFront;
        CanPlaceBack = so.canPlaceBack;
        CardArt = so.cardArt;
        Description = so.description;
    }

    public void TakeDamage(int damage)
    {
        if (SpecialEffects.Contains(SpecialEffect.Tough))
        {
            damage -= Power;
            if (damage <= 0)
                return;
        }

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Max(0, CurrentHealth);
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }

    public bool HasEffect(SpecialEffect effect)
    {
        return SpecialEffects.Contains(effect);
    }

    public bool HasStatusEffect(ContinuousEffect effect)
    {
        return ContinuousEffects.Contains(effect);
        
    }
}