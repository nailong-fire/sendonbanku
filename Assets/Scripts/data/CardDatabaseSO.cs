using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "卡牌系统/卡牌数据库")]
public class CardDatabaseSO : ScriptableObject
{
    [Header("卡牌列表")]
    [Tooltip("所有可用的卡牌")]
    public List<CardDataSO> allCards = new List<CardDataSO>();

    [Header("起始套牌")]
    [Tooltip("起始套牌包含的卡牌 ID 列表")]
    public List<string> starterDeckCardIds = new List<string>();

    // 通过 ID 查找卡牌
    public CardDataSO GetCardById(string cardId)
    {
        return allCards.Find(card => card.cardId == cardId);
    }

    // 获取指定类型的卡牌
    public List<CardDataSO> GetCardsByType(CardType type)
    {
        return allCards.FindAll(card => card.cardType == type);
    }

    // 获取指定稀有度的卡牌
    public List<CardDataSO> GetCardsByRarity(Rarity rarity)
    {
        return allCards.FindAll(card => card.rarity == rarity);
    }

    // 获取费用在指定范围内的卡牌
    public List<CardDataSO> GetCardsByCost(int minCost, int maxCost)
    {
        return allCards.FindAll(card => card.faithCost >= minCost && card.faithCost <= maxCost);
    }

    // 获取随机卡牌
    public CardDataSO GetRandomCard()
    {
        if (allCards.Count == 0) return null;
        return allCards[Random.Range(0, allCards.Count)];
    }

    // 获取 N 张随机卡牌
    public List<CardDataSO> GetRandomCards(int count, bool allowDuplicates = true)
    {
        if (allCards.Count == 0) return new List<CardDataSO>();

        List<CardDataSO> result = new List<CardDataSO>();

        if (allowDuplicates)
        {
            // 允许重复，从整个列表随机抽取
            for (int i = 0; i < count; i++)
            {
                result.Add(GetRandomCard());
            }
        }
        else
        {
            // 不允许重复，从列表中抽取不重复项
            if (count > allCards.Count)
                count = allCards.Count;

            List<CardDataSO> tempList = new List<CardDataSO>(allCards);

            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(0, tempList.Count);
                result.Add(tempList[randomIndex]);
                tempList.RemoveAt(randomIndex);
            }
        }

        return result;
    }

    // 获取起始套牌
    public List<CardDataSO> GetStarterDeck()
    {
        List<CardDataSO> deck = new List<CardDataSO>();

        foreach (string cardId in starterDeckCardIds)
        {
            CardDataSO card = GetCardById(cardId);
            if (card != null)
            {
                deck.Add(card);
            }
            else
            {
                Debug.LogWarning($"起始套牌中未找到卡牌: {cardId}");
            }
        }

        return deck;
    }

    // 编辑器校验（检查重复 ID）
    private void OnValidate()
    {
        // 检查是否存在重复的 ID
        HashSet<string> ids = new HashSet<string>();
        foreach (var card in allCards)
        {
            if (card != null)
            {
                if (ids.Contains(card.cardId))
                {
                    Debug.LogWarning($"卡牌数据库中存在重复ID: {card.cardId}");
                }
                else
                {
                    ids.Add(card.cardId);
                }
            }
        }
    }
}