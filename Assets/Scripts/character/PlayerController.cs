using System.Collections;
using UnityEngine;

public class PlayerController : UniversalController
{
    protected override void Awake()
    {
        base.Awake();
        
        characterName = "玩家";
        isPlayerControlled = true;
    }
    
    public override void Initialize(string name, bool isPlayer, int startingHope, int startingFaith)
    {
        base.Initialize("玩家", true, startingHope, startingFaith);
    }
}