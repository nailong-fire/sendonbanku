// === 3. 卡牌实体类 ===
using UnityEngine;

public class CardEntity : MonoBehaviour
{
    public CardData Data { get; private set; }
    public BoardPosition Position { get; private set; }
    public Player Owner { get; private set; }
    
    public void Initialize(CardData data, Player owner)
    {
        Data = data.Clone();
        Owner = owner;
    }
    
    public void SetPosition(BoardPosition position)
    {
        Position = position;
    }
    
    public bool TakeDamage(int damage)
    {
        // 检查特殊效果：忽略低伤害
        if (Data.SpecialEffects.Contains(SpecialEffect.IgnoreLowDamage) && damage < 2)
            return false;
            
        Data.CurrentHealth -= damage;
        
        if (!Data.IsAlive)
        {
            OnDeath();
            return true;
        }
        return false;
    }
    
    public void Heal(int amount)
    {
        Data.CurrentHealth = Mathf.Min(Data.CurrentHealth + amount, Data.Health);
    }
    
    private void OnDeath()
    {
        // 卡牌死亡时减少主人的hope值
        Owner.OnCardDestroyed(this);
        
        // 触发死亡效果
        // TODO: 实现死亡特效和清理
    }
    
    public bool CanAttackTarget(CardEntity target)
    {
        // 检查远程攻击能力
        if (Data.SpecialEffects.Contains(SpecialEffect.RangedAttack))
            return true;
            
        // 近战只能攻击前排
        return target.Position.IsFrontRow;
    }
}