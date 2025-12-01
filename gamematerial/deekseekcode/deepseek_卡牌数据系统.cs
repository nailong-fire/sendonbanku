// === 2. 卡牌数据系统 ===
using System.Collections.Generic;

public enum CardType
{
    Buddha,
    Bodhisattva,
    Vajra,
    Apsara,
    Monk,
    Effect // 效果牌
}

public enum SpecialEffect
{
    None,
    RangedAttack,      // 远程攻击
    IgnoreLowDamage,   // 低于2的伤害无效
    Healer,           // 治疗者
    Guardian,         // 护卫
    QuickStrike       // 先攻
}

[System.Serializable]
public class CardData
{
    public string CardId;
    public string CardName;
    public CardType Type;
    
    // 基础属性
    public int Health;
    public int Power;
    public int Speed; // 行动速度，值越小行动越快
    public int FaithCost;
    
    // 特殊效果
    public List<SpecialEffect> SpecialEffects = new();
    public int CurrentHealth { get; set; }
    
    // 位置限制
    public bool CanPlaceFront = true;
    public bool CanPlaceBack = true;
    
    public bool IsAlive => CurrentHealth > 0;
    
    public CardData Clone()
    {
        return new CardData
        {
            CardId = this.CardId,
            CardName = this.CardName,
            Type = this.Type,
            Health = this.Health,
            Power = this.Power,
            Speed = this.Speed,
            FaithCost = this.FaithCost,
            SpecialEffects = new List<SpecialEffect>(this.SpecialEffects),
            CurrentHealth = this.Health,
            CanPlaceFront = this.CanPlaceFront,
            CanPlaceBack = this.CanPlaceBack
        };
    }
}