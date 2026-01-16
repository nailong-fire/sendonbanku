using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stupidai : enemyai
{
    private int turn = 0;

    private CardEntity AIDrawCard()
    {
        return null;
    }

    public override void AIPlayCard()
    {
        turn++;
        Debug.Log($"Stupid AI第 {turn} 回合行动开始...");
        UniversalController enemy = GameManager.Instance.enemy;
        CardZone enemyBattlefield = enemy.battlefield;
        CardZone enemyHandZone = enemy.handZone;
        List<CardEntity> playableCards = enemy.handZone.GetAllCards();
        CardEntity targetCard = null;
        if (playableCards.Count > 0)
            targetCard = playableCards[0];
        if (enemy.resourceSystem.CurrentFaith >= 3 && targetCard != null)
        {
            int i = 2; 
            while (i >= 0)
            {
                if (enemyBattlefield.PlaceCardAtPosition(targetCard, true, i, enemyHandZone))
                {
                    return;
                }
                i--;
            }
        }
        return;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
