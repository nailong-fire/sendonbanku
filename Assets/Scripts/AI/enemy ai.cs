using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyai : MonoBehaviour
{
    private int turn = 0;

    private void AIDrawCard(string name)
    {
        if (name == "the stupid")
        {
            Debug.Log($"Stupid AI第 {turn} 回合抽卡开始...");
            UniversalController enemy = GameManager.Instance.enemy;
            enemy.DrawCard();
        }
        else if (name == "the little")
        {
            Debug.Log($"Little AI第 {turn} 回合抽卡开始...");
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
    }

    public void AIPlayCard(string name)
    {
        turn++;
        if (name == "the stupid")
        {
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
        else if (name == "the little")
        {
            Debug.Log($"Little AI第 {turn} 回合行动开始...");
            UniversalController enemy = GameManager.Instance.enemy;
            CardZone enemyBattlefield = enemy.battlefield;
            CardZone enemyHandZone = enemy.handZone;
            List<CardEntity> playableCards = enemy.handZone.GetAllCards();
            CardEntity targetCard = null;
            if (playableCards.Count == 0)
                return;
            if (enemyBattlefield.GetBackRowCards().Count >= 2)
            {
                targetCard = playableCards.Find(card => card.CardData.CardName == "守僧");
                if (targetCard != null)
                {
                    int i = 2; 
                    while (i >= 1)
                    {
                        if (enemyBattlefield.PlaceCardAtPosition(targetCard, true, i, enemyHandZone))
                        {
                            return;
                        }
                        i--;
                    }
                }
            }
            else
            {
                if(enemy.resourceSystem.CurrentFaith <= 1)
                {
                    return;
                }
                else if (enemy.resourceSystem.CurrentFaith <= 2)
                {
                    targetCard = playableCards.Find(card => card.CardData.CardName == "飞天");
                    if (targetCard != null)
                    {
                        int i = 1; 
                        while (i >= 0)
                        {
                            if (enemyBattlefield.PlaceCardAtPosition(targetCard, false, i, enemyHandZone))
                            {
                                return;
                            }
                            i--;
                        }
                    }
                }
                else
                {
                    targetCard = playableCards.Find(card => card.CardData.CardName == "莲花化身");
                    if (targetCard != null)
                    {
                        int i = 1; 
                        while (i >= 0)
                        {
                            if (enemyBattlefield.PlaceCardAtPosition(targetCard, false, i, enemyHandZone))
                            {
                                return;
                            }
                            i--;
                        }
                    }
                    targetCard = playableCards.Find(card => card.CardData.CardName == "天宫乐伎");
                    if (targetCard != null)
                    {
                        int i = 1; 
                        while (i >= 0)
                        {
                            if (enemyBattlefield.PlaceCardAtPosition(targetCard, false, i, enemyHandZone))
                            {
                                return;
                            }
                            i--;
                        }
                    }
                }
            }
            return;
        }
        
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
