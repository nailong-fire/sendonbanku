using UnityEngine;
using System.Collections.Generic;

// 卡牌类型枚举
public enum CardType
{
    Buddha,        // 佛
    Bodhisattva,   // 菩萨
    Vajra,         // 金刚
    Apsara,        // 飞天
    Monk,          // 僧
    Effect         // 效果牌
}

// 特殊效果枚举
public enum SpecialEffect
{
    None,
    RangedAttack,      // 远程攻击
    IgnoreLowDamage,   // 低于2的伤害无效
    Healer,           // 治疗者
    Guardian,         // 护卫
    QuickStrike,      // 先攻
    Taunt            // 嘲讽
}

// 稀有度枚举
public enum Rarity
{
    Common,      // 普通
    Uncommon,    // 稀有
    Rare,        // 罕见
    Epic,        // 史诗
    Legendary    // 传说
}

[CreateAssetMenu(fileName = "Card_", menuName = "云冈卡牌/卡牌数据")]
public class CardDataSO : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("卡牌唯一ID，建议格式: card_001")]
    public string cardId = "card_001";

    [Tooltip("卡牌名称")]
    public string cardName = "新卡牌";

    [Tooltip("卡牌描述")]
    [TextArea(2, 4)]
    public string description = "卡牌描述";

    [Tooltip("卡牌类型")]
    public CardType cardType = CardType.Buddha;

    [Tooltip("卡牌稀有度")]
    public Rarity rarity = Rarity.Common;

    [Header("卡牌外观")]
    [Tooltip("卡牌正面图案")]
    public Sprite cardArt;

    [Tooltip("卡牌边框（根据稀有度不同）")]
    public Sprite cardFrame;

    [Tooltip("卡牌背景")]
    public Sprite cardBackground;

    [Header("战斗属性")]
    [Tooltip("生命值")]
    [Range(1, 20)]
    public int health = 5;

    [Tooltip("力量值（攻击/治疗量）")]
    [Range(0, 10)]
    public int power = 3;

    [Tooltip("行动速度（越大越快）")]
    [Range(1, 10)]
    public int speed = 5;

    [Tooltip("消耗的Faith值")]
    [Range(0, 10)]
    public int faithCost = 1;

    [Header("位置限制")]
    [Tooltip("是否可以放置在前排")]
    public bool canPlaceFront = true;

    [Tooltip("是否可以放置在后排")]
    public bool canPlaceBack = true;

    [Header("特殊效果")]
    [Tooltip("卡牌的特殊效果列表")]
    public List<SpecialEffect> specialEffects = new List<SpecialEffect>();

    [Header("卡牌故事")]
    [Tooltip("卡牌背后的云冈石窟故事")]
    [TextArea(3, 6)]
    public string story = "这张卡牌与云冈石窟的故事...";

    // 运行时使用的数据副本（避免修改原始数据）
    public CardRuntimeData CreateRuntimeData()
    {
        return new CardRuntimeData(this);
    }

    // 验证数据（在编辑器中调用）
    private void OnValidate()
    {
        // 确保ID不为空
        if (string.IsNullOrEmpty(cardId))
        {
            cardId = $"card_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        }

        // 确保生命值至少为1
        health = Mathf.Max(1, health);

        // 确保消耗不为负数
        faithCost = Mathf.Max(0, faithCost);
    }
}

// 运行时卡牌数据（从ScriptableObject创建的副本）
[System.Serializable]
public class CardRuntimeData
{
    public string CardId;
    public string CardName;
    public CardType Type;
    public Rarity Rarity;
    public int MaxHealth;
    public int CurrentHealth;
    public int Power;
    public int Speed;
    public int FaithCost;
    public List<SpecialEffect> SpecialEffects;
    public bool CanPlaceFront;
    public bool CanPlaceBack;
    public Sprite CardArt;
    public string Description;

    public bool IsAlive => CurrentHealth > 0;

    public CardRuntimeData(CardDataSO so)
    {
        CardId = so.cardId;
        CardName = so.cardName;
        Type = so.cardType;
        Rarity = so.rarity;
        MaxHealth = so.health;
        CurrentHealth = so.health;
        Power = so.power;
        Speed = so.speed;
        FaithCost = so.faithCost;
        SpecialEffects = new List<SpecialEffect>(so.specialEffects);
        CanPlaceFront = so.canPlaceFront;
        CanPlaceBack = so.canPlaceBack;
        CardArt = so.cardArt;
        Description = so.description;
    }

    public void TakeDamage(int damage)
    {
        // 检查是否有忽略低伤害的效果
        if (SpecialEffects.Contains(SpecialEffect.IgnoreLowDamage) && damage < 2)
            return;

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
}