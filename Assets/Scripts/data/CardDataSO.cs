using UnityEngine;
using System.Collections.Generic;

// 卡牌类型枚举
public enum CardType
{
    Buddha,        // 佛陀
    Bodhisattva,   // 菩萨
    Vajra,         // 金刚
    Apsara,        // 飞天（仙女）
    Monk,          // 僧侣
    Effect         // 效果类
}

// 特殊效果枚举
public enum SpecialEffect
{
    None,
    RangedAttack,      // 远程攻击
    IgnoreLowDamage,   // 忽略低伤害（小于2的伤害无效）
    Healer,            // 治疗者
    Guardian,          // 守护
    QuickStrike,       // 迅捷
    Taunt              // 嘲讽
}

// 稀有度枚举
public enum Rarity
{
    Common,      // 普通
    Uncommon,    // 非凡
    Rare,        // 稀有
    Epic,        // 史诗
    Legendary    // 传说
}

[CreateAssetMenu(fileName = "Card_", menuName = "卡牌系统/卡牌数据")]
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

    [Tooltip("卡牌类型")]
    public CardType cardType = CardType.Buddha;

    [Tooltip("卡牌稀有度")]
    public Rarity rarity = Rarity.Common;

    [Header("视觉资源")]
    [Tooltip("卡牌头像或立绘")]
    public Sprite cardArt;

    [Tooltip("卡牌边框（可根据稀有度更换）")]
    public Sprite cardFrame;

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
        // 如果包含忽略小额伤害效果且伤害小于2，则不减少生命
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