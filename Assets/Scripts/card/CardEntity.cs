using System;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;


[RequireComponent(typeof(Collider2D))]
public class CardEntity : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private SpriteRenderer cardBackground;
    [SerializeField] private SpriteRenderer cardFrame;
    [SerializeField] private SpriteRenderer cardArt;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private ParticleSystem cardGlow;

    [Header("状态颜色")]
    [SerializeField] private Color normalColor = Color.white;
    // 新增回合制相关属性
    [Header("回合制属性")]
    public bool HasActedThisTurn = false;
    public bool HasActionAbility = false;

    [Header("位置属性")]
    public bool IsInFrontRow = false;
    public int positionindex = 0;

    // 卡牌运行时数据
    private CardRuntimeData _cardData;
    private CardDataSO _cardDataSO;

    // 状态
    private bool _isSelected = false;
    private bool _isPlayable = false;
    private bool _isOnHand = false;

    // 所有者
    [SerializeField]
    private UniversalController _owner;

    // 事件
    public System.Action<CardEntity> OnCardDestroyed;


    public CardRuntimeData CardData => _cardData;
    public CardDataSO CardDataSO => _cardDataSO;
    public UniversalController Owner => _owner;
    public bool IsPlayable => _isPlayable;
    public bool IsOnHand => _isOnHand;

    // 初始化（使用 CardDataSO）
    public void Initialize(CardDataSO cardSO, UniversalController owner)
    {
        _cardDataSO = cardSO;
        _cardData = cardSO.CreateRuntimeData();
        _owner = owner;

        UpdateCardVisuals();
        UpdateRarityEffects();
    }

    // 使用运行时数据初始化
    public void Initialize(CardRuntimeData runtimeData, PlayerController owner)
    {
        _cardData = runtimeData;
        _owner = owner;

        // TODO: 若需要可根据ID查找并缓存对应的 CardDataSO

        UpdateCardVisuals();
        UpdateRarityEffects();
    }

    // 更新卡牌可视化
    private void UpdateCardVisuals()
    {
        if (_cardData == null) return;

        // 更新文字
        if (healthText) healthText.text = _cardData.CurrentHealth.ToString();
        if (powerText) powerText.text = _cardData.Power.ToString();
        if (costText) costText.text = _cardData.FaithCost.ToString();
        if (nameText) nameText.text = _cardData.CardName;
        if (descriptionText) descriptionText.text = _cardData.Description;

        // 更新图片
        if (cardArt && _cardData.CardArt)
        {
            cardArt.sprite = _cardData.CardArt;
        }

        // 更新边框颜色（稀有度）
        UpdateFrameColor();

        // 更新状态颜色
        UpdateStateColor();
    }

    // 根据稀有度更新边框颜色
    private void UpdateFrameColor()
    {
        if (!cardFrame) return;

        Color frameColor = _cardData.Rarity switch
        {
            Rarity.Common => Color.gray,
            Rarity.Uncommon => Color.green,
            Rarity.Rare => Color.blue,
            Rarity.Epic => Color.magenta,
            Rarity.Legendary => Color.yellow,
            _ => Color.white
        };

        cardFrame.color = frameColor;
    }

    // 更新稀有度特效
    private void UpdateRarityEffects()
    {
        if (!cardGlow) return;

        // 根据稀有度开启/关闭特效
        bool shouldGlow = _cardData.Rarity >= Rarity.Rare;

        if (shouldGlow && !cardGlow.isPlaying)
        {
            cardGlow.Play();

            // 设置发光颜色
            var main = cardGlow.main;
            Color glowColor = _cardData.Rarity switch
            {
                Rarity.Rare => Color.blue,
                Rarity.Epic => Color.magenta,
                Rarity.Legendary => Color.yellow,
                _ => Color.white
            };

            main.startColor = new ParticleSystem.MinMaxGradient(glowColor);
        }
        else if (!shouldGlow && cardGlow.isPlaying)
        {
            cardGlow.Stop();
        }
    }

    // 更新状态颜色（背景）
    private void UpdateStateColor()
    {
        if (!cardBackground) return;

        Color targetColor = normalColor;

        cardBackground.color = targetColor;
    }

    // 受伤处理
    public void TakeDamage(int damage)
    {
        if (_cardData == null || !_cardData.IsAlive) return;

        _cardData.TakeDamage(damage);
        UpdateCardVisuals();

        // 播放受伤特效
        StartCoroutine(DamageEffect());

        //if (!_cardData.IsAlive)
        //{
        //    OnDeath();
        //}
    }

    // 治疗
    public void Heal(int amount)
    {
        if (_cardData == null || !_cardData.IsAlive) return;

        _cardData.Heal(amount);
        UpdateCardVisuals();

        // 播放治疗特效
        StartCoroutine(HealEffect());
    }

    // 受伤特效
    private System.Collections.IEnumerator DamageEffect()
    {
        if (cardArt)
        {
            Color originalColor = cardArt.color;
            cardArt.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            cardArt.color = originalColor;
        }
    }

    // 治疗特效
    private System.Collections.IEnumerator HealEffect()
    {
        if (cardArt)
        {
            Color originalColor = cardArt.color;
            cardArt.color = Color.green;
            yield return new WaitForSeconds(0.1f);
            cardArt.color = originalColor;
        }
    }

    // 死亡处理
    private void OnDeath()
    {
        // 播放死亡特效
        StartCoroutine(DeathEffect());

        // 触发销毁事件
        OnCardDestroyed?.Invoke(this);

        // 销毁对象（延迟以播放特效）
        Destroy(gameObject, 0.5f);
    }

    // 死亡特效协程
    private System.Collections.IEnumerator DeathEffect()
    {
        // 特效淡出
        float duration = 0.5f;
        float elapsed = 0f;

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        TextMeshPro[] texts = GetComponentsInChildren<TextMeshPro>();

        while (elapsed < duration)
        {
            float alpha = 1 - (elapsed / duration);

            foreach (var renderer in renderers)
            {
                Color color = renderer.color;
                color.a = alpha;
                renderer.color = color;
            }

            foreach (var text in texts)
            {
                Color color = text.color;
                color.a = alpha;
                text.color = color;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    // 设置是否可被使用（可打出）
    public void SetPlayable(bool playable)
    {
        _isPlayable = playable;
        UpdateStateColor();
    }

    // 设置是否在手牌中
    public void SetOnHand(bool onHand)
    {
        _isOnHand = onHand;
        UpdateStateColor();
    }


    // 是否可以攻击目标
    public bool CanAttackTarget(CardEntity target)
    {
        if (target == null || !target.CardData.IsAlive) return false;

        // 如果具有远程攻击效果，则可以攻击任意目标
        if (_cardData.HasEffect(SpecialEffect.RangedAttack))
            return true;

        // 暂时返回 true，具体规则由游戏逻辑决定
        return true;
    }

    // 获取攻击力
    public int GetAttackPower()
    {
        return _cardData.Power;
    }

    // 获取治疗能力
    public int GetHealPower()
    {
        return _cardData.HasEffect(SpecialEffect.Healer) ? _cardData.Power : 0;
    }


    // 新增方法
    // 执行行动
    public IEnumerator ExecuteAction()
    {
        // 这里实现卡牌的行动逻辑
        // 例如：治疗友军、施放法术等
        
        Debug.Log($"{CardData.CardName} 执行行动");

        if(_cardData.HasEffect(SpecialEffect.MeleeAttack))
        {
            
        }
        
        yield return new WaitForSeconds(0.5f);
    }
    
    // 处理回合结束效果
    public void ProcessEndTurnEffects()
    {
        // 处理持续伤害/治疗等效果
        // 例如：中毒每回合扣血，恢复每回合回血
        
        if (_cardData.HasStatusEffect(ContinuousEffect.Poison))
        {
            TakeDamage(1);
            Debug.Log($"{CardData.CardName} 受到中毒伤害");
        }
        
        if (_cardData.HasStatusEffect(ContinuousEffect.Regeneration))
        {
            // Heal(1);
            Debug.Log($"{CardData.CardName} 受到恢复效果");
        }
    }
    
    
    private void HealAllies()
    {
        // 治疗所有友军卡牌
        // 实现治疗逻辑
    }
}