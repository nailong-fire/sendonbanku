using System.Collections;
using UnityEngine;

public class PlayerController : UniversalController
{
    [Header("玩家特有设置")]
    public bool allowManualCardPlay = true;  // 允许手动出牌
    public bool showPlayablePositions = true; // 显示可放置位置
    
    [Header("输入设置")]
    public KeyCode endTurnKey = KeyCode.Space;
    public KeyCode drawCardKey = KeyCode.D;
    
    [Header("视觉反馈")]
    public GameObject playableIndicatorPrefab;
    public Color playablePositionColor = Color.green;
    
    private GameObject[] _positionIndicators;
    private bool _isDraggingCard = false;
    private CardEntity _draggedCard;
    
    protected override void Awake()
    {
        base.Awake();
        
        characterName = "玩家";
        isPlayerControlled = true;
    }
    
    public override void Initialize(string name, bool isPlayer, int startingHope, int startingFaith)
    {
        base.Initialize("玩家", true, startingHope, startingFaith);
        
        // 初始化位置指示器
        if (showPlayablePositions && battlefield != null && playableIndicatorPrefab != null)
        {
            InitializePositionIndicators();
        }
    }
    
    // 初始化位置指示器
    private void InitializePositionIndicators()
    {
        int totalPositions = battlefield.frontRowCount + battlefield.backRowCount;
        _positionIndicators = new GameObject[totalPositions];
        
        for (int i = 0; i < totalPositions; i++)
        {
            _positionIndicators[i] = Instantiate(playableIndicatorPrefab, transform);
            _positionIndicators[i].SetActive(false);
        }
        
        UpdatePositionIndicators();
    }
    
    // 更新位置指示器
    private void UpdatePositionIndicators()
    {
        if (!showPlayablePositions || _positionIndicators == null) return;
        
        var positions = battlefield.GetAllPositions();
        
        for (int i = 0; i < positions.Count && i < _positionIndicators.Length; i++)
        {
            var position = positions[i];
            var indicator = _positionIndicators[i];
            
            if (position.positionTransform != null)
            {
                indicator.transform.position = position.positionTransform.position;
            }
            
            // 设置颜色：空位置为绿色，已占用为红色
            if (position.isOccupied)
            {
                SetIndicatorColor(indicator, Color.red);
            }
            else
            {
                SetIndicatorColor(indicator, playablePositionColor);
            }
        }
    }
    
    // 设置指示器颜色
    private void SetIndicatorColor(GameObject indicator, Color color)
    {
        var renderer = indicator.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
        
        var spriteRenderer = indicator.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
    
    private void Update()
    {
        if (!isActive || !isPlayerControlled) return;
        
        HandleInput();
        
        if (_isDraggingCard && _draggedCard != null)
        {
            UpdateCardDrag();
        }
    }
    
    // 处理输入
    private void HandleInput()
    {
        // 结束回合
        if (Input.GetKeyDown(endTurnKey) && isMyTurn)
        {
            EndTurn();
        }
        
        // 抽牌（测试用）
        if (Input.GetKeyDown(drawCardKey) && !isMyTurn)
        {
            DrawCard();
        }
        
        // 开始/结束拖拽
        if (Input.GetMouseButtonDown(0) && isMyTurn)
        {
            TryStartCardDrag();
        }
        
        if (Input.GetMouseButtonUp(0) && _isDraggingCard)
        {
            TryEndCardDrag();
        }
    }
    
    // 尝试开始拖拽卡牌
    private void TryStartCardDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            CardEntity card = hit.collider.GetComponent<CardEntity>();
            if (card != null && card.IsPlayable && _handCards.Contains(card))
            {
                StartCardDrag(card);
            }
        }
    }
    
    // 开始拖拽卡牌
    private void StartCardDrag(CardEntity card)
    {
        _isDraggingCard = true;
        _draggedCard = card;
        
        // 高亮可放置位置
        HighlightPlayablePositions(card);
        
        Debug.Log($"开始拖拽卡牌: {card.CardData.CardName}");
    }
    
    // 更新卡牌拖拽
    private void UpdateCardDrag()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10; // 确保在相机前面
        
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        _draggedCard.transform.position = worldPos;
    }
    
    // 尝试结束拖拽
    private void TryEndCardDrag()
    {
        if (!_isDraggingCard || _draggedCard == null) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            // 检查是否拖拽到位置指示器上
            for (int i = 0; i < _positionIndicators.Length; i++)
            {
                if (_positionIndicators[i].activeSelf && 
                    hit.collider.gameObject == _positionIndicators[i])
                {
                    var position = battlefield.GetAllPositions()[i];
                    if (!position.isOccupied && battlefield.CanPlaceCardAtPosition(_draggedCard, position.isFrontRow))
                    {
                        // 放置卡牌
                        PlayCardToPosition(_draggedCard, position);
                        EndCardDrag();
                        return;
                    }
                }
            }
        }
        
        // 没有放置到有效位置，返回手牌
        ReturnCardToHand();
        EndCardDrag();
    }
    
    // 结束拖拽
    private void EndCardDrag()
    {
        _isDraggingCard = false;
        _draggedCard = null;
        
        // 隐藏所有指示器
        HideAllPositionIndicators();
    }
    
    // 返回卡牌到手牌
    private void ReturnCardToHand()
    {
        if (_draggedCard == null || handZone == null) return;
        
        // 重置位置到手牌区域
        if (handZone.GetEmptyPositions().Count > 0)
        {
            var position = handZone.GetEmptyPositions()[0];
            _draggedCard.transform.position = position.positionTransform.position;
        }
    }
    
    // 高亮可放置位置
    private void HighlightPlayablePositions(CardEntity card)
    {
        if (!showPlayablePositions || _positionIndicators == null) return;
        
        var positions = battlefield.GetAllPositions();
        
        for (int i = 0; i < positions.Count && i < _positionIndicators.Length; i++)
        {
            var position = positions[i];
            var indicator = _positionIndicators[i];
            
            bool canPlace = !position.isOccupied && 
                          battlefield.CanPlaceCardAtPosition(card, position.isFrontRow);
            
            indicator.SetActive(canPlace);
            
            if (canPlace)
            {
                SetIndicatorColor(indicator, playablePositionColor);
            }
        }
    }
    
    // 隐藏所有位置指示器
    private void HideAllPositionIndicators()
    {
        if (_positionIndicators == null) return;
        
        foreach (var indicator in _positionIndicators)
        {
            if (indicator != null)
            {
                indicator.SetActive(false);
            }
        }
    }
    
    // 战场卡牌添加事件
    protected override void OnBattlefieldCardAdded(CardEntity card)
    {
        base.OnBattlefieldCardAdded(card);
        
        // 更新位置指示器
        if (showPlayablePositions)
        {
            UpdatePositionIndicators();
        }
    }
    
    // 战场卡牌移除事件
    protected override void OnBattlefieldCardRemoved(CardEntity card)
    {
        base.OnBattlefieldCardRemoved(card);
        
        // 更新位置指示器
        if (showPlayablePositions)
        {
            UpdatePositionIndicators();
        }
    }
    
    // 开始回合（玩家特有）
    public override void StartTurn()
    {
        base.StartTurn();
        
        // 显示可放置位置
        if (showPlayablePositions)
        {
            UpdatePositionIndicators();
            ShowAllPositionIndicators();
        }
    }
    
    // 显示所有位置指示器
    private void ShowAllPositionIndicators()
    {
        if (_positionIndicators == null) return;
        
        var positions = battlefield.GetAllPositions();
        
        for (int i = 0; i < positions.Count && i < _positionIndicators.Length; i++)
        {
            var indicator = _positionIndicators[i];
            var position = positions[i];
            
            indicator.SetActive(true);
            SetIndicatorColor(indicator, position.isOccupied ? Color.red : Color.gray);
        }
    }
    
    // 结束回合（玩家特有）
    public override void EndTurn()
    {
        // 隐藏位置指示器
        if (showPlayablePositions)
        {
            HideAllPositionIndicators();
        }
        
        base.EndTurn();
    }
}