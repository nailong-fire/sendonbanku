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
    [SerializeField] private SpriteRenderer cardArt;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("状态颜色")]
    [SerializeField] private Color normalColor = Color.white;
    // 新增回合制相关属性
    [Header("回合制属性")]
    public bool HasActedThisTurn = false;
    public bool HasActionAbility = true;

    [Header("位置属性")]
    public bool IsInFrontRow = false;
    public int positionindex = 0;

    [Header("特效属性")]
    public bool isguarded = false;
    public CardEntity guardian = null;
    public CardEntity guardedCard = null;

    [Header("动画引用")]
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioClip damageSound; // 受伤音效
    [SerializeField] [Range(0f, 1f)] private float damageSoundDelay = 0f; // 音效延迟时间(秒)，用于对轴


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

    private void Awake()
    {
        if (animator == null)
        {
            // 尝试从自身或子物体查找 Animator
            animator = GetComponentInChildren<Animator>();
        }

        // 初始化：如果 Animator 绑在特效层上，确保证它是透明的
        if (animator != null)
        {
            SpriteRenderer effectSR = animator.GetComponent<SpriteRenderer>();
            // 只有当这个 SR 不是卡牌自身的主 SR 时（防止把卡面清空了）
            if (effectSR != null && effectSR != cardBackground && effectSR != cardArt)
            {
                effectSR.sprite = null; 
            }
        }
    }

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
    }

    // 使用运行时数据初始化
    public void Initialize(CardRuntimeData runtimeData, PlayerController owner)
    {
        _cardData = runtimeData;
        _owner = owner;

        // TODO: 若需要可根据ID查找并缓存对应的 CardDataSO

        UpdateCardVisuals();
    }

    // 更新卡牌可视化
    private void UpdateCardVisuals()
    {
        if (_cardData == null) return;

        // 更新文字
        if (healthText) healthText.text = _cardData.CurrentHealth.ToString();
        if (powerText) powerText.text = _cardData.Power.ToString();
        if (speedText) speedText.text = _cardData.Speed.ToString();
        if (costText) costText.text = _cardData.FaithCost.ToString();
        if (nameText) nameText.text = _cardData.CardName;
        if (descriptionText) descriptionText.text = _cardData.Description;

        // 更新图片
        if (cardArt && _cardData.CardArt)
        {
            cardArt.sprite = _cardData.CardArt;
        }

        // 更新状态颜色
        UpdateStateColor();
    }


    private System.Collections.IEnumerator PlaySoundWithDelay()
    {
        if (damageSoundDelay > 0)
        {
            yield return new WaitForSeconds(damageSoundDelay);
        }
        
        AudioSource.PlayClipAtPoint(damageSound, transform.position);
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

        // 触发动画: 确保你在 Animator 面板里设置了一个叫 "Hurt" 的 Trigger
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        // 播放受伤音效 (带延迟控制)
        if (damageSound != null)
        {
            StartCoroutine(PlaySoundWithDelay());
        }

        _cardData.TakeDamage(damage);
        UpdateCardVisuals();

        // 播放受伤特效 (代码控制的简单特效，可保留作为叠加)
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