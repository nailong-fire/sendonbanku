// === 8. 卡牌效果系统（示例）===
public class CardEffectSystem
{
    public static void ApplySpecialEffect(CardEntity source, CardEntity target, SpecialEffect effect)
    {
        switch (effect)
        {
            case SpecialEffect.RangedAttack:
                // 远程攻击无视前排保护
                ApplyRangedAttack(source, target);
                break;
                
            case SpecialEffect.IgnoreLowDamage:
                // 此效果在TakeDamage方法中已处理
                break;
                
            case SpecialEffect.Healer:
                ApplyHeal(source, target);
                break;
                
            case SpecialEffect.QuickStrike:
                ApplyQuickStrike(source, target);
                break;
        }
    }
    
    private static void ApplyRangedAttack(CardEntity source, CardEntity target)
    {
        // 远程攻击可以直接攻击后排
        // 攻击逻辑...
    }
    
    private static void ApplyHeal(CardEntity source, CardEntity target)
    {
        if (target != null && target.Owner == source.Owner)
        {
            target.Heal(source.Data.Power);
        }
    }
    
    private static void ApplyQuickStrike(CardEntity source, CardEntity target)
    {
        // 先攻效果：在对方攻击前先攻击
        // 实现需要修改行动顺序逻辑
    }
}