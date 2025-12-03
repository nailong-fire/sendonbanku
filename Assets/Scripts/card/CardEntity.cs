using System;
using TMPro;
using UnityEngine;


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

    // 卡牌运行时数据
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

    // 初始化（使用 CardDataSO）
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
            // 根据当前生命值混合受伤颜色
            float healthPercent = (float)_cardData.CurrentHealth / _cardData.MaxHealth;
            targetColor = Color.Lerp(damagedColor, normalColor, healthPercent);
        }

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

        if (!_cardData.IsAlive)
        {
            OnDeath();
        }
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

    // 设置是否可选
    public void SetSelectable(bool selectable)
    {
        // TODO: 添加可选时的视觉/交互提示
    }

    // 设置是否可被使用（可打出）
    public void SetPlayable(bool playable)
    {
        _isPlayable = playable;
        UpdateStateColor();
    }

    // 设置是否已上场
    public void SetOnBoard(bool onBoard)
    {
        _isOnBoard = onBoard;
        UpdateStateColor();
    }

    // 鼠标按下
    private void OnMouseDown()
    {
        OnCardClicked?.Invoke(this);
    }

    // 鼠标进入（悬停）
    private void OnMouseEnter()
    {
        if (!_isSelected)
        {
            // 悬停效果：略微放大
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

    // 选中
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
}