using System;
using TMPro;
using UnityEngine;

//临时类
public class PlayerController
{

}


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
    [SerializeField] private Color damagedColor = new Color(1, 0.5f, 0.5f, 1);
    [SerializeField] private Color selectedColor = new Color(0.5f, 0.8f, 1, 1);
    [SerializeField] private Color playableColor = new Color(0.5f, 1, 0.5f, 1);

    // 卡牌数据
    private CardRuntimeData _cardData;
    private CardDataSO _cardDataSO;

    // 状态
    private bool _isSelected = false;
    private bool _isPlayable = false;
    private bool _isOnBoard = false;

    // 所有者
    private PlayerController _owner;

    // 事件
    public System.Action<CardEntity> OnCardClicked;
    public System.Action<CardEntity> OnCardDestroyed;


    public CardRuntimeData CardData => _cardData;
    public CardDataSO CardDataSO => _cardDataSO;
    public PlayerController Owner => _owner;
    public bool IsPlayable => _isPlayable;
    public bool IsOnBoard => _isOnBoard;

    // 初始化卡牌
    public void Initialize(CardDataSO cardSO, PlayerController owner)
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

        // 这里需要根据ID查找对应的CardDataSO
        // 或者存储一个引用

        UpdateCardVisuals();
        UpdateRarityEffects();
    }

    // 更新卡牌视觉效果
    private void UpdateCardVisuals()
    {
        if (_cardData == null) return;

        // 设置文本
        if (healthText) healthText.text = _cardData.CurrentHealth.ToString();
        if (powerText) powerText.text = _cardData.Power.ToString();
        if (costText) costText.text = _cardData.FaithCost.ToString();
        if (nameText) nameText.text = _cardData.CardName;
        if (descriptionText) descriptionText.text = _cardData.Description;

        // 设置图像
        if (cardArt && _cardData.CardArt)
        {
            cardArt.sprite = _cardData.CardArt;
        }

        // 根据稀有度设置边框颜色
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

            // 设置粒子颜色
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

    // 更新状态颜色
    private void UpdateStateColor()
    {
        if (!cardBackground) return;

        Color targetColor = normalColor;

        if (_isSelected)
        {
            targetColor = selectedColor;
        }
        else if (_isPlayable && !_isOnBoard)
        {
            targetColor = playableColor;
        }
        else if (_cardData.CurrentHealth < _cardData.MaxHealth)
        {
            // 根据血量比例混合颜色
            float healthPercent = (float)_cardData.CurrentHealth / _cardData.MaxHealth;
            targetColor = Color.Lerp(damagedColor, normalColor, healthPercent);
        }

        cardBackground.color = targetColor;
    }

    // 卡牌受伤
    public void TakeDamage(int damage)
    {
        if (_cardData == null || !_cardData.IsAlive) return;

        _cardData.TakeDamage(damage);
        UpdateCardVisuals();

        // 受伤特效
        StartCoroutine(DamageEffect());

        if (!_cardData.IsAlive)
        {
            OnDeath();
        }
    }

    // 治疗卡牌
    public void Heal(int amount)
    {
        if (_cardData == null || !_cardData.IsAlive) return;

        _cardData.Heal(amount);
        UpdateCardVisuals();

        // 治疗特效
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

    // 卡牌死亡
    private void OnDeath()
    {
        // 死亡特效
        StartCoroutine(DeathEffect());

        // 触发事件
        OnCardDestroyed?.Invoke(this);

        // 延迟销毁
        Destroy(gameObject, 0.5f);
    }

    // 死亡特效
    private System.Collections.IEnumerator DeathEffect()
    {
        // 淡出效果
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

    // 设置是否可选
    public void SetSelectable(bool selectable)
    {
        // 可以添加视觉效果
    }

    // 设置是否可打出
    public void SetPlayable(bool playable)
    {
        _isPlayable = playable;
        UpdateStateColor();
    }

    // 设置是否在场上
    public void SetOnBoard(bool onBoard)
    {
        _isOnBoard = onBoard;
        UpdateStateColor();
    }

    // 鼠标点击
    private void OnMouseDown()
    {
        OnCardClicked?.Invoke(this);
    }

    // 鼠标悬停
    private void OnMouseEnter()
    {
        if (!_isSelected)
        {
            // 悬停效果：轻微放大
            transform.localScale = Vector3.one * 1.1f;
        }
    }

    // 鼠标离开
    private void OnMouseExit()
    {
        if (!_isSelected)
        {
            transform.localScale = Vector3.one;
        }
    }

    // 选中卡牌
    public void Select()
    {
        _isSelected = true;
        transform.localScale = Vector3.one * 1.2f;
        UpdateStateColor();
    }

    // 取消选中
    public void Deselect()
    {
        _isSelected = false;
        transform.localScale = Vector3.one;
        UpdateStateColor();
    }

    // 获取卡牌攻击范围
    public bool CanAttackTarget(CardEntity target)
    {
        if (target == null || !target.CardData.IsAlive) return false;

        // 如果有远程攻击，可以攻击任何目标
        if (_cardData.HasEffect(SpecialEffect.RangedAttack))
            return true;

        // 否则只能攻击前排
        // 这里需要根据游戏规则实现
        return true;
    }

    // 获取攻击力
    public int GetAttackPower()
    {
        return _cardData.Power;
    }

    // 获取治疗量
    public int GetHealPower()
    {
        return _cardData.HasEffect(SpecialEffect.Healer) ? _cardData.Power : 0;
    }
}