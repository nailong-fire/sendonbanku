using System;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class CardEntity : MonoBehaviour
{
    [Header("�������")]
    [SerializeField] private SpriteRenderer cardBackground;
    [SerializeField] private SpriteRenderer cardFrame;
    [SerializeField] private SpriteRenderer cardArt;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private ParticleSystem cardGlow;

    [Header("״̬��ɫ")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color damagedColor = new Color(1, 0.5f, 0.5f, 1);
    [SerializeField] private Color selectedColor = new Color(0.5f, 0.8f, 1, 1);
    [SerializeField] private Color playableColor = new Color(0.5f, 1, 0.5f, 1);

    // ��������
    private CardRuntimeData _cardData;
    private CardDataSO _cardDataSO;

    // ״̬
    private bool _isSelected = false;
    private bool _isPlayable = false;
    private bool _isOnBoard = false;

    // ������
    private PlayerController _owner;

    // �¼�
    public System.Action<CardEntity> OnCardClicked;
    public System.Action<CardEntity> OnCardDestroyed;


    public CardRuntimeData CardData => _cardData;
    public CardDataSO CardDataSO => _cardDataSO;
    public PlayerController Owner => _owner;
    public bool IsPlayable => _isPlayable;
    public bool IsOnBoard => _isOnBoard;

    // ��ʼ������
    public void Initialize(CardDataSO cardSO, PlayerController owner)
    {
        _cardDataSO = cardSO;
        _cardData = cardSO.CreateRuntimeData();
        _owner = owner;

        UpdateCardVisuals();
        UpdateRarityEffects();
    }

    // ʹ������ʱ���ݳ�ʼ��
    public void Initialize(CardRuntimeData runtimeData, PlayerController owner)
    {
        _cardData = runtimeData;
        _owner = owner;

        // ������Ҫ����ID���Ҷ�Ӧ��CardDataSO
        // ���ߴ洢һ������

        UpdateCardVisuals();
        UpdateRarityEffects();
    }

    // ���¿����Ӿ�Ч��
    private void UpdateCardVisuals()
    {
        if (_cardData == null) return;

        // �����ı�
        if (healthText) healthText.text = _cardData.CurrentHealth.ToString();
        if (powerText) powerText.text = _cardData.Power.ToString();
        if (costText) costText.text = _cardData.FaithCost.ToString();
        if (nameText) nameText.text = _cardData.CardName;
        if (descriptionText) descriptionText.text = _cardData.Description;

        // ����ͼ��
        if (cardArt && _cardData.CardArt)
        {
            cardArt.sprite = _cardData.CardArt;
        }

        // ����ϡ�ж����ñ߿���ɫ
        UpdateFrameColor();

        // ����״̬��ɫ
        UpdateStateColor();
    }

    // ����ϡ�жȸ��±߿���ɫ
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

    // ����ϡ�ж���Ч
    private void UpdateRarityEffects()
    {
        if (!cardGlow) return;

        // ����ϡ�жȿ���/�ر���Ч
        bool shouldGlow = _cardData.Rarity >= Rarity.Rare;

        if (shouldGlow && !cardGlow.isPlaying)
        {
            cardGlow.Play();

            // ����������ɫ
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

    // ����״̬��ɫ
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
            // ����Ѫ�����������ɫ
            float healthPercent = (float)_cardData.CurrentHealth / _cardData.MaxHealth;
            targetColor = Color.Lerp(damagedColor, normalColor, healthPercent);
        }

        cardBackground.color = targetColor;
    }

    // ��������
    public void TakeDamage(int damage)
    {
        if (_cardData == null || !_cardData.IsAlive) return;

        _cardData.TakeDamage(damage);
        UpdateCardVisuals();

        // ������Ч
        StartCoroutine(DamageEffect());

        if (!_cardData.IsAlive)
        {
            OnDeath();
        }
    }

    // ���ƿ���
    public void Heal(int amount)
    {
        if (_cardData == null || !_cardData.IsAlive) return;

        _cardData.Heal(amount);
        UpdateCardVisuals();

        // ������Ч
        StartCoroutine(HealEffect());
    }

    // ������Ч
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

    // ������Ч
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

    // ��������
    private void OnDeath()
    {
        // ������Ч
        StartCoroutine(DeathEffect());

        // �����¼�
        OnCardDestroyed?.Invoke(this);

        // �ӳ�����
        Destroy(gameObject, 0.5f);
    }

    // ������Ч
    private System.Collections.IEnumerator DeathEffect()
    {
        // ����Ч��
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

    // �����Ƿ��ѡ
    public void SetSelectable(bool selectable)
    {
        // ���������Ӿ�Ч��
    }

    // �����Ƿ�ɴ��
    public void SetPlayable(bool playable)
    {
        _isPlayable = playable;
        UpdateStateColor();
    }

    // �����Ƿ��ڳ���
    public void SetOnBoard(bool onBoard)
    {
        _isOnBoard = onBoard;
        UpdateStateColor();
    }

    // �����
    private void OnMouseDown()
    {
        OnCardClicked?.Invoke(this);
    }

    // �����ͣ
    private void OnMouseEnter()
    {
        if (!_isSelected)
        {
            // ��ͣЧ������΢�Ŵ�
            transform.localScale = Vector3.one * 1.1f;
        }
    }

    // ����뿪
    private void OnMouseExit()
    {
        if (!_isSelected)
        {
            transform.localScale = Vector3.one;
        }
    }

    // ѡ�п���
    public void Select()
    {
        _isSelected = true;
        transform.localScale = Vector3.one * 1.2f;
        UpdateStateColor();
    }

    // ȡ��ѡ��
    public void Deselect()
    {
        _isSelected = false;
        transform.localScale = Vector3.one;
        UpdateStateColor();
    }

    // ��ȡ���ƹ�����Χ
    public bool CanAttackTarget(CardEntity target)
    {
        if (target == null || !target.CardData.IsAlive) return false;

        // �����Զ�̹��������Թ����κ�Ŀ��
        if (_cardData.HasEffect(SpecialEffect.RangedAttack))
            return true;

        // ����ֻ�ܹ���ǰ��
        // ������Ҫ������Ϸ����ʵ��
        return true;
    }

    // ��ȡ������
    public int GetAttackPower()
    {
        return _cardData.Power;
    }

    // ��ȡ������
    public int GetHealPower()
    {
        return _cardData.HasEffect(SpecialEffect.Healer) ? _cardData.Power : 0;
    }
}