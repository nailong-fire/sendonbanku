using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyai : MonoBehaviour
{
    private int turn = 0;

    public void AIDrawCard(string name)
    {
        if (name == "leader" && turn == 1)
        {
            Debug.Log($"AI第 {turn} 回合抽卡开始...");
            UniversalController enemy = GameManager.Instance.enemy;
            enemy.DrawCard();
            enemy.cardDatabase.AddCardToPlayerOwnedPile("010", 4);
        }
        else
        {
            Debug.Log($"AI第 {turn} 回合抽卡开始...");
            UniversalController enemy = GameManager.Instance.enemy;
            enemy.DrawCard();
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
                if (targetCard != null && enemy.resourceSystem.CurrentFaith >= 1)
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
        else if (name == "the lost")
        {
            Debug.Log($"Lost AI第 {turn} 回合行动开始...");
            UniversalController enemy = GameManager.Instance.enemy;
            CardZone enemyBattlefield = enemy.battlefield;
            CardZone enemyHandZone = enemy.handZone;
            List<CardEntity> playableCards = enemy.handZone.GetAllCards();
            CardEntity targetCard = null;
            if (playableCards.Count == 0)
                return;
            if (enemyBattlefield.GetFrontRowCards().Count >= 3)
            {
                targetCard = playableCards.Find(card => card.CardData.CardName == "飞天");
                if (targetCard != null && enemy.resourceSystem.CurrentFaith >= 2)
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
                if(enemy.resourceSystem.CurrentFaith <= 1)
                {
                    return;
                }
                else if (enemy.resourceSystem.CurrentFaith <= 2)
                {
                    targetCard = playableCards.Find(card => card.CardData.CardName == "力士");
                    if (targetCard != null)
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
                }
                else
                {
                    targetCard = playableCards.Find(card => card.CardData.CardName == "金刚力士");
                    if (targetCard != null)
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
                }
            }
            return;
        }
        else if (name == "the sorrowful")
        {
            Debug.Log($"Lost AI第 {turn} 回合行动开始...");
            UniversalController enemy = GameManager.Instance.enemy;
            CardZone enemyBattlefield = enemy.battlefield;
            CardZone enemyHandZone = enemy.handZone;
            List<CardEntity> playableCards = enemy.handZone.GetAllCards();
            CardEntity targetCard = null;
            if(enemy.resourceSystem.CurrentFaith <= 2)
            {
                return;
            }
            else if (enemy.resourceSystem.CurrentFaith >= 3)
            {
                targetCard = playableCards.Find(card => card.CardData.CardName == "佛首");
                if (targetCard != null)
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
            }

            if (enemy.resourceSystem.CurrentFaith >= 4)
            {
                targetCard = playableCards.Find(card => card.CardData.CardName == "书文殊菩萨");
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

                targetCard = playableCards.Find(card => card.CardData.CardName == "剑文殊菩萨");
                if (targetCard != null)
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
            }
            return;
        }
        else if (name == "leader")
        {
            Debug.Log($"Lost AI第 {turn} 回合行动开始...");
            UniversalController enemy = GameManager.Instance.enemy;
            CardZone enemyBattlefield = enemy.battlefield;
            CardZone enemyHandZone = enemy.handZone;
            List<CardEntity> playableCards = enemy.handZone.GetAllCards();
            CardEntity targetCard = null;
            targetCard = playableCards.Find(card => card.CardData.CardName == "佛首");
            if (targetCard != null && enemy.resourceSystem.CurrentFaith >= 5)
            {
                enemyBattlefield.PlaceCardAtPosition(targetCard, true, 2, enemyHandZone);
            }
            else if (targetCard == null)
            {
                if (enemy.resourceSystem.CurrentFaith >= 1)
                {
                    targetCard = playableCards.Find(card => card.CardData.CardName == "愈僧");
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
