using UnityEngine;
using TMPro;

public class CharacterUIController : MonoBehaviour, ICharacterUI
{
    [Header("Hope显示")]
    public TextMeshProUGUI hopeText;
    public UnityEngine.UI.Slider hopeSlider;
    //public GameObject hopeChangeEffectPrefab;
    
    [Header("Faith显示")]
    public TextMeshProUGUI faithText;
    //public GameObject faithChangeEffectPrefab;
    
    //[Header("手牌显示")]
    //public TextMeshProUGUI handCountText;
    //public GameObject handCountPanel;
    
    //[Header("卡组显示")]
    //public TextMeshProUGUI deckCountText;
    //public GameObject deckCountPanel;
    
    //[Header("角色名称")]
    //public TextMeshProUGUI characterNameText;
    
    [Header("回合指示器")]
    public GameObject turnIndicator;
    //public ParticleSystem turnIndicatorParticles;
    
    //[Header("浮动文字")]
    //public GameObject floatingTextPrefab;
    //public Transform floatingTextParent;
    
    //[Header("视觉特效")]
    //public ParticleSystem damageEffectPrefab;
    //public ParticleSystem healEffectPrefab;
    
    [Header("位置偏移")]
    public Vector3 uiWorldOffset = new Vector3(0, 2.5f, 0);
    
    // 当前控制的角色
    private UniversalController _character;
    
    public void Initialize(UniversalController character)
    {
        _character = character;
        
        // 订阅角色事件
        if (_character != null)
        {
            _character.resourceSystem.OnHopeChangedWithDetails += UpdateHopeUI;
            _character.resourceSystem.OnFaithChangedWithDetails += UpdateFaithUI;
            //_character.resourceSystem.OnShowFloatingNumber += OnShowFloatingNumber;
            
            // 初始更新
            UpdateHopeUI(_character.resourceSystem.CurrentHope, 
                        _character.resourceSystem.maxHope, 0);
            UpdateFaithUI(_character.resourceSystem.CurrentFaith,
                         _character.resourceSystem.maxFaith, 0);
            
            // 更新名称
            //if (characterNameText != null)
            //{
            //    characterNameText.text = _character.characterName;
            //}
        }
        
        // 隐藏回合指示器
        if (turnIndicator != null)
        {
            turnIndicator.SetActive(false);
        }
    }
    
    public void UpdateHopeUI(int currentHope, int maxHope, int changeAmount)
    {
        if (hopeText != null)
        {
            hopeText.text = $"{currentHope}/{maxHope}";
            
            // 显示变化效果
            //if (changeAmount != 0)
            //{
            //    ShowChangeEffect(hopeText.transform.position, changeAmount, 
            //                   changeAmount > 0 ? Color.green : Color.red);
            //}
        }
        
        if (hopeSlider != null)
        {
            hopeSlider.maxValue = maxHope;
            hopeSlider.value = currentHope;
        }
    }
    
    public void UpdateFaithUI(int currentFaith, int maxFaith, int changeAmount)
    {

        Debug.Log("=====================================");
        if (faithText != null)
        {
            faithText.text = currentFaith.ToString();
            
            //if (changeAmount != 0)
            //{
            //    ShowChangeEffect(faithText.transform.position, changeAmount, Color.yellow);
            //}
        }
    }
    
    //public void UpdateHandCountUI(int handCount, int maxHandSize)
    //{
    //    if (handCountText != null)
    //    {
    //        handCountText.text = $"{handCount}/{maxHandSize}";
    //    }
    //}
    
    //public void UpdateDeckCountUI(int deckCount)
    //{
    //    if (deckCountText != null)
    //    {
    //        deckCountText.text = deckCount.ToString();
    //    }
    //}
    
    //public void ShowFloatingText(Vector3 worldPosition, string text, Color color)
    //{
    //    if (floatingTextPrefab == null || floatingTextParent == null) return;
    //    
    //    Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition + uiWorldOffset);
    //    
    //    GameObject floatingText = Instantiate(floatingTextPrefab, 
    //                                        screenPos, 
    //                                        Quaternion.identity, 
    //                                        floatingTextParent);
    //    
    //    TextMeshProUGUI textComponent = floatingText.GetComponent<TextMeshProUGUI>();
    //    if (textComponent != null)
    //    {
    //        textComponent.text = text;
    //        textComponent.color = color;
    //    }
    //
    //    // 销毁浮动文字
    //    Destroy(floatingText, 2f);
    //}
    
    //public void ShowDamageEffect(CardEntity target, int damage)
    //{
    //    if (damageEffectPrefab == null || target == null) return;
    //    
    //    ParticleSystem effect = Instantiate(damageEffectPrefab, 
    //                                      target.transform.position, 
    //                                      Quaternion.identity);
    //    effect.Play();
    //    
    //    // 显示伤害数字
    //    ShowFloatingText(target.transform.position, $"-{damage}", Color.red);
    //    
    //    Destroy(effect.gameObject, 2f);
    //}
    
    //public void ShowHealEffect(CardEntity target, int healAmount)
    //{
    //    if (healEffectPrefab == null || target == null) return;
    //    
    //    ParticleSystem effect = Instantiate(healEffectPrefab, 
    //                                      target.transform.position, 
    //                                      Quaternion.identity);
    //    effect.Play();
    //    
    //    ShowFloatingText(target.transform.position, $"+{healAmount}", Color.green);
    //    
    //    Destroy(effect.gameObject, 2f);
    //}
    
    public void ShowTurnIndicator(bool isMyTurn)
    {
        if (turnIndicator != null)
        {
            turnIndicator.SetActive(isMyTurn);
            
            //if (isMyTurn && turnIndicatorParticles != null)
            //{
            //    turnIndicatorParticles.Play();
            //}
            //else if (turnIndicatorParticles != null)
            //{
            //    turnIndicatorParticles.Stop();
            //}
        }
    }
    
    //public void UpdateCharacterName(string name)
    //{
    //    if (characterNameText != null)
    //    {
    //        characterNameText.text = name;
    //    }
    //}
    
    // 显示变化效果
    //private void ShowChangeEffect(Vector3 position, int amount, Color color)
    //{
    //    if (amount == 0) return;
    //    
    //    string sign = amount > 0 ? "+" : "";
    //    string text = $"{sign}{amount}";
    //    
    //    ShowFloatingText(position, text, color);
    //}
    
    // 资源系统显示浮动数值事件
    //private void OnShowFloatingNumber(Vector3 position, int amount, Color color)
    //{
    //    ShowChangeEffect(position, amount, color);
    //}
    
    // 更新手牌和卡组计数
    //public void UpdateCardCounts()
    //{
    //    if (_character != null)
    //    {
    //        UpdateHandCountUI(_character.GetHandCount(), 
    //                        _character.handZone?.maxCards ?? 10);
    //        UpdateDeckCountUI(_character.GetDeckCount());
    //    }
    //}
    
    private void OnDestroy()
    {
        // 取消订阅事件
        if (_character != null)
        {
            _character.resourceSystem.OnHopeChangedWithDetails -= UpdateHopeUI;
            _character.resourceSystem.OnFaithChangedWithDetails -= UpdateFaithUI;
            //_character.resourceSystem.OnShowFloatingNumber -= OnShowFloatingNumber;
        }
    }
}