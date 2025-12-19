using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    [Header("选择设置")]
    public float selectionHighlightScale = 1.5f;
    public bool isSelectingCard = false;
    public bool isPlacingCard = false;
    
    //[Header("卡牌移动设置")]
    //public float cardMoveDuration = 0.5f;
    //public AnimationCurve cardMoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    // 单例
    private static SelectionManager _instance;
    public static SelectionManager Instance => _instance;
    
    // 选择状态
    private CardEntity selectedCard;

    private UniversalController targetplayer;
    private CardZone targetZone;
    private Vector3 targetPosition;
    
    // 高亮效果
    private Material originalCardMaterial;
    private Material highlightMaterial;
    private GameObject currentHighlight;
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // 创建高亮材质
        CreateHighlightMaterial();
        
        // 订阅游戏事件
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPhaseChange += OnGamePhaseChange;
            GameManager.Instance.OnGameOver += OnGameOver;
        }
    }
    
    void Update()
    {
        // 只在玩家回合且是玩家行动阶段处理点击
        if (GameManager.Instance != null && 
            GameManager.Instance.currentPhase == GameManager.TurnPhase.PlayerAction &&
            !GameManager.Instance.isGameOver)
        {
            HandlePlayerInput();
        }
    }
    
    // 处理玩家输入
    void HandlePlayerInput()
    {
        if (Input.GetMouseButtonDown(0)) // 左键点击
        {
            HandleLeftClick();
        }
        
        if (Input.GetMouseButtonDown(1)) // 右键点击
        {
            HandleRightClick();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC取消选择
        {
            CancelSelection();
        }
    }
    
    // 处理左键点击
    void HandleLeftClick()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        Debug.Log(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        
        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;
            
            // 检查点击的是卡牌
            if (clickedObject.CompareTag("Card"))
            {
                Debug.Log("点击到卡牌");
                CardEntity card = clickedObject.GetComponent<CardEntity>();
                if (card != null)
                {
                    OnCardClicked(card);
                }
            }
            // 检查点击的是战场
            else if (clickedObject.CompareTag("Battlefield"))
            {
                Transform tf = clickedObject.GetComponent<Transform>();
                CardZone zone = clickedObject.GetComponentInParent<CardZone>();
                UniversalController player = clickedObject.GetComponentInParent<UniversalController>();
                if (tf != null && zone != null)
                {
                    OnBattlefieldClicked(player, zone, tf.position);
                }
            }
        }
        else
        {
            Debug.Log("未点击到任何对象");
            CancelSelection();
            // 检查UI上的点击（如果是UI，使用不同方法）
        }
    }
    
    // 处理右键点击
    void HandleRightClick()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;
            
            // 右键查看卡牌详情
            if (clickedObject.CompareTag("Card"))
            {
                CardEntity card = clickedObject.GetComponent<CardEntity>();
                if (card != null)
                {
                    ShowCardDetails(card);
                }
            }
        }
    }
    
    // 卡牌被点击
    void OnCardClicked(CardEntity card)
    {
        // 检查卡牌是否在手牌区
        if (!card.IsOnHand)
        {
            Debug.Log("只能选择手牌中的卡牌");
            ShowMessage("只能选择手牌中的卡牌", 2f);
            return;
        }

        Debug.Log(card.Owner.characterName);
        
        // 检查是否是玩家自己的卡牌
        if (!card.Owner.isPlayerControlled)
        {
            Debug.Log("不能选择敌人的卡牌");
            ShowMessage("不能选择敌人的卡牌", 2f);
            return;
        }
        
        // 检查资源是否足够
        if (GameManager.Instance.player.resourceSystem.CurrentFaith < card.CardData.FaithCost)
        {
            Debug.Log($"信仰值不足！需要 {card.CardData.FaithCost}");
            ShowMessage($"信仰值不足！需要 {card.CardData.FaithCost} 信仰", 2f);
            return;
        }
        
        // 选择卡牌
        SelectCard(card);
        
        Debug.Log($"选中卡牌: {card.CardData.CardName}");
        ShowMessage($"选中: {card.CardData.CardName}，点击战场位置放置", 3f);
    }
    
    // 战场被点击
    void OnBattlefieldClicked(UniversalController player, CardZone zone, Vector3 position)
    {
        if (selectedCard == null)
        {
            Debug.Log("请先选择一张卡牌");
            ShowMessage("请先选择一张卡牌", 2f);
            return;
        }
        
        // 检查是否是玩家自己的战场
        if (zone != GameManager.Instance.playerBattlefield)
        {
            Debug.Log("只能放置到玩家自己的战场");
            ShowMessage("只能放置到玩家自己的战场", 2f);
            return;
        }
        
        // 检查战场是否有空位
        if (zone.GetCardCount() >= zone.maxCards)
        {
            Debug.Log("战场已满！");
            ShowMessage("战场已满！", 2f);
            return;
        }
        
        targetZone = zone;
        targetPosition = position;
        targetplayer = player;
        TryPlaceCard();
    }
    
    // 选择卡牌
    void SelectCard(CardEntity card)
    {
        // 如果已经选择了其他卡牌，先取消选择
        if (selectedCard != null && selectedCard != card)
        {
            DeselectCard();
        }
        
        selectedCard = card;
        isSelectingCard = true;
        
        // 高亮选中的卡牌
        //HighlightSelectedCard();
        
        // 播放音效（可选）
        // AudioManager.Instance.PlaySound("card_select");
    }
    
    // 取消选择
    public void CancelSelection()
    {
        if (selectedCard != null)
        {
            DeselectCard();
            Debug.Log("取消选择卡牌");
            ShowMessage("取消选择", 1f);
        }
    }
    
    // 取消选择卡牌
    void DeselectCard()
    {
        if (selectedCard != null)
        {
            // 移除高亮
            RemoveCardHighlight();
            
            selectedCard = null;
            isSelectingCard = false;
        }
    }
    
    // 尝试放置卡牌
    void TryPlaceCard()
    {
        if (selectedCard == null || targetZone == null)
        {
            Debug.LogWarning("无法放置：没有选中的卡牌或目标区域");
            return;
        }
        
        // 再次检查条件
        if (!CanPlaceCard(selectedCard, targetZone))
        {
            Debug.LogWarning("无法放置卡牌，条件不满足");
            ShowMessage("无法放置卡牌", 2f);
            return;
        }
        
        // 开始放置流程
        StartCoroutine(PlaceCardRoutine());
    }
    
    // 检查是否可以放置卡牌
    bool CanPlaceCard(CardEntity card, CardZone zone)
    {
        // 1. 检查游戏阶段
        if (GameManager.Instance.currentPhase != GameManager.TurnPhase.PlayerAction)
        {
            Debug.Log("不是玩家行动阶段");
            return false;
        }
        
        // 2. 检查是否是玩家自己的卡牌
        if (card.Owner != GameManager.Instance.player)
        {
            Debug.Log("不是玩家的卡牌");
            return false;
        }
        
        // 3. 检查资源
        if (GameManager.Instance.player.resourceSystem.CurrentFaith < card.CardData.FaithCost)
        {
            Debug.Log($"信仰值不足，需要 {card.CardData.FaithCost}");
            return false;
        }
        
        // 4. 检查目标区域
        if (zone != GameManager.Instance.playerBattlefield)
        {
            Debug.Log("只能放置到玩家战场");
            return false;
        }
        
        // 5. 检查区域容量
        if (zone.GetCardCount() >= zone.maxCards)
        {
            Debug.Log("战场已满");
            return false;
        }
        
        // 6. 检查卡牌是否在手牌中
        if (!card.IsOnHand)
        {
            Debug.Log("卡牌不在手牌中");
            return false;
        }
        
        return true;
    }
    
    // 放置卡牌协程
    IEnumerator PlaceCardRoutine()
    {
        isPlacingCard = true;
        bool isFrontRow;
        int positionIndex = targetZone.GetPositionIndexAtPoint_battlefield(targetPosition, out isFrontRow);
        CardZone handzone = targetplayer.handZone;
        // 通知玩家控制器放置卡牌
        bool success = targetZone.PlaceCardAtPosition(selectedCard, isFrontRow, positionIndex, handzone);
        
        if (success)
        {
            Debug.Log($"成功放置卡牌: {selectedCard.CardData.CardName}");
            ShowMessage($"打出: {selectedCard.CardData.CardName}", 2f);
            // 清除选择
            ClearSelection();
        }
        else
        {
            Debug.LogWarning("放置卡牌失败");
            ShowMessage("放置卡牌失败", 2f);
        }
        
        isPlacingCard = false;

        yield return null;
    }
    
    // 卡牌移动动画
    //IEnumerator PlayCardMoveAnimation(CardEntity card, Vector3 targetPos)
    //{
    //    float elapsed = 0f;
    //    Vector3 startPos = card.transform.position;
    //    Vector3 startScale = card.transform.localScale;
    //    
    //    // 先稍微抬高
    //    Vector3 midPos = (startPos + targetPos) / 2 + Vector3.up * 2f;
    //    
    //    while (elapsed < cardMoveDuration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = elapsed / cardMoveDuration;
    //        t = cardMoveCurve.Evaluate(t);
    //        
    //        // 贝塞尔曲线移动
    //        Vector3 position = CalculateBezierPoint(t, startPos, midPos, targetPos);
    //        card.transform.position = position;
    //        
    //        // 轻微旋转
    //        card.transform.rotation = Quaternion.Euler(0, t * 360, 0);
    //        
    //        yield return null;
    //    }
    //    
    //    // 确保最终位置正确
    //    card.transform.position = targetPos;
    //    card.transform.rotation = Quaternion.identity;
    //    card.transform.localScale = startScale;
    //}
    
    // 计算贝塞尔曲线点
    //Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    //{
    //    float u = 1 - t;
    //    float tt = t * t;
    //    float uu = u * u;
    //    
    //    Vector3 p = uu * p0;
    //    p += 2 * u * t * p1;
    //    p += tt * p2;
    //    
    //    return p;
    //}
    
    // 清除选择
    void ClearSelection()
    {
        DeselectCard();
        targetZone = null;
        targetPosition = Vector3.zero;
    }
    
    // 高亮选中的卡牌
    void HighlightSelectedCard()
    {
        if (selectedCard == null) return;
        
        // 创建高亮效果
        CreateCardHighlight(selectedCard.transform);
        
        // 放大效果
        selectedCard.transform.localScale *= selectionHighlightScale;
    }
    
    // 移除卡牌高亮
    void RemoveCardHighlight()
    {
        if (selectedCard != null)
        {
            // 恢复原始大小
            selectedCard.transform.localScale = Vector3.one * 0.15f;
        }
        
        // 销毁高亮对象
        if (currentHighlight != null)
        {
            Destroy(currentHighlight);
            currentHighlight = null;
        }
    }
    
    // 创建卡牌高亮
    void CreateCardHighlight(Transform cardTransform)
    {
        if (currentHighlight != null)
        {
            Destroy(currentHighlight);
        }
        
        // 创建高亮框
        currentHighlight = new GameObject("CardHighlight");
        currentHighlight.transform.SetParent(cardTransform);
        currentHighlight.transform.localPosition = Vector3.zero;
        currentHighlight.transform.localRotation = Quaternion.identity;
        currentHighlight.transform.localScale = Vector3.one * 0.18f;
        
        // 添加线框渲染器（可选）
        // 或者使用粒子效果
    }
    
    // 创建高亮材质
    void CreateHighlightMaterial()
    {
        highlightMaterial = new Material(Shader.Find("Standard"));
        highlightMaterial.SetFloat("_Mode", 3); // 透明模式
        highlightMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        highlightMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        highlightMaterial.EnableKeyword("_ALPHABLEND_ON");
        highlightMaterial.renderQueue = 3000;
    }
    
    // 显示卡牌详情
    void ShowCardDetails(CardEntity card)
    {
        Debug.Log($"=== 卡牌详情 ===");
        Debug.Log($"名称: {card.CardData.CardName}");
        Debug.Log($"类型: {card.CardData.Type}");
        Debug.Log($"费用: {card.CardData.FaithCost} Faith");
        Debug.Log($"攻击: {card.CardData.Power}");
        Debug.Log($"速度: {card.CardData.Speed}");
        Debug.Log($"描述: {card.CardData.Description}");
        
        // 可以在这里打开详细卡牌信息面板
        // UIManager.Instance.ShowCardDetail(card.CardData);
    }
    
    // 显示消息
    void ShowMessage(string message, float duration)
    {
        Debug.Log($"消息: {message}");
        // 可以调用UI系统显示消息
        // UIManager.Instance.ShowMessage(message, duration);
    }
    
    // 游戏阶段变化
    void OnGamePhaseChange(GameManager.TurnPhase newPhase)
    {
        if (newPhase != GameManager.TurnPhase.PlayerAction)
        {
            // 如果不是玩家行动阶段，清除选择
            ClearSelection();
        }
    }
    
    // 游戏结束
    void OnGameOver(bool playerWon)
    {
        ClearSelection();
    }
    
    // 公共方法：获取当前选中的卡牌
    public CardEntity GetSelectedCard()
    {
        return selectedCard;
    }
    
    // 公共方法：敌人AI放置卡牌
    public void PlaceCardForAI(CardEntity card, CardZone zone)
    {
        if (card == null || zone == null) return;
        
        // 直接放置，不需要选择过程
        StartCoroutine(AIPlaceCardRoutine(card, zone));
    }
    
    // AI放置卡牌协程
    IEnumerator AIPlaceCardRoutine(CardEntity card, CardZone zone)
    {
        yield return null;
       
       Debug.Log($"AI放置卡牌: {card.CardData.CardName}");
    }
    
    // 清除所有选择
    public void ClearAllSelections()
    {
        ClearSelection();
    }
    
    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPhaseChange -= OnGamePhaseChange;
            GameManager.Instance.OnGameOver -= OnGameOver;
        }
    }
}