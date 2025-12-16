using System.Collections;
using UnityEngine;

public class EnemyController : UniversalController
{
    [Header("AI设置")]
    public EnemyAI aiBehavior;
    public float thinkTimeMin = 0.5f;
    public float thinkTimeMax = 1.5f;
    
    [Header("攻击策略")]
    public bool prioritizeHighThreatTargets = true;
    public bool focusOnPlayerHope = false;
    
    [Header("状态")]
    public bool isThinking = false;
    
    private PlayerController _targetPlayer;
    
    protected override void Awake()
    {
        base.Awake();
        
        characterName = "敌人";
        isPlayerControlled = false;
    }
    
    public override void Initialize(string name, bool isPlayer, int startingHope, int startingFaith)
    {
        base.Initialize("敌人", false, startingHope, startingFaith);
        
        // 查找目标玩家
        _targetPlayer = FindObjectOfType<PlayerController>();
        
    }
    
    
}