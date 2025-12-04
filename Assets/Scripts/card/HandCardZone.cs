using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCardZone : MonoBehaviour
{
    public string zoneName;
    public int maxCards = 7;
    public List<CardDataSO> cards = new List<CardDataSO>();

    // 添加卡牌
    public bool AddCard(CardDataSO card)
    {
        if (cards.Count < maxCards)
        {
            cards.Add(card);
            // 触发卡牌添加事件
            return true;
        }
        return false;
    }

    // 移除卡牌
    public bool RemoveCard(CardDataSO card)
    {
        if (cards.Contains(card))
        {
            cards.Remove(card);
            // 触发卡牌移除事件
            return true;
        }
        return false;
    }

    // 检查是否可以添加卡牌
    public bool CanAddCard(CardDataSO card)
    {
        return cards.Count < maxCards;
    }

    // 获取区域内的卡牌数量
    public int GetCardCount()
    {
        return cards.Count;
    }
}