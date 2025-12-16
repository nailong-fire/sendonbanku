using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "卡牌/卡牌数据库")]
public class CardDatabaseSO : ScriptableObject
{
    [Header("卡牌列表")]
    [Tooltip("所有可用的卡牌")]
    public List<CardDataSO> allCards = new List<CardDataSO>();

    [Header("玩家拥有卡牌")]
    [Tooltip("玩家拥有的卡牌 ID 列表（可重复，表示多张）")]
    public List<string> playerOwnedCardIds = new List<string>();

    [Header("玩家牌组")]
    [Tooltip("玩家当前牌组中的卡牌 ID 列表（可重复）")]
    public List<string> playerDeckCardIds = new List<string>();

    [Header("玩家弃牌组")]
    [Tooltip("玩家弃牌组中的卡牌 ID 列表（可重复）")]
    public List<string> playerDiscardPileCardIds = new List<string>();

    // 用于记录和恢复的牌组备份
    private List<string> deckBackup = new List<string>();

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
        return allCards[UnityEngine.Random.Range(0, allCards.Count)];
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
                int randomIndex = UnityEngine.Random.Range(0, tempList.Count);
                result.Add(tempList[randomIndex]);
                tempList.RemoveAt(randomIndex);
            }
        }

        return result;
    }

    // === 新增功能 ===

    // 从玩家牌组中随机抽取不重复的N张牌，并从牌组中移除
    public List<CardDataSO> DrawCardsFromPlayerDeck(int count, bool removeFromDeck = true)
    {
        List<CardDataSO> drawnCards = new List<CardDataSO>();
        
        if (playerDeckCardIds.Count == 0)
        {
            Debug.LogWarning("牌组为空，无法抽牌");
            return drawnCards;
        }

        if (count > playerDeckCardIds.Count)
        {
            Debug.LogWarning($"抽牌数量{count}超过牌组剩余数量{playerDeckCardIds.Count}");
            count = playerDeckCardIds.Count;
        }

        // 复制当前牌组用于随机选择
        List<string> tempDeck = new List<string>(playerDeckCardIds);

        for (int i = 0; i < count; i++)
        {
            if (tempDeck.Count == 0) break;

            int randomIndex = UnityEngine.Random.Range(0, tempDeck.Count);
            string cardId = tempDeck[randomIndex];
            CardDataSO card = GetCardById(cardId);
            
            if (card != null)
            {
                drawnCards.Add(card);
                // 从临时牌组移除
                tempDeck.RemoveAt(randomIndex);
                
                // 如果需要从真实牌组移除
                if (removeFromDeck)
                {
                    playerDeckCardIds.Remove(cardId);
                }
            }
        }

        return drawnCards;
    }

    // 从玩家弃牌组中随机抽取不重复的N张牌，并从弃牌组中移除
    public List<CardDataSO> DrawCardsFromDiscardPile(int count, bool removeFromDiscardPile = true)
    {
        List<CardDataSO> drawnCards = new List<CardDataSO>();
        
        if (playerDiscardPileCardIds.Count == 0)
        {
            Debug.LogWarning("弃牌组为空，无法抽牌");
            return drawnCards;
        }

        if (count > playerDiscardPileCardIds.Count)
        {
            Debug.LogWarning($"抽牌数量{count}超过弃牌组数量{playerDiscardPileCardIds.Count}");
            count = playerDiscardPileCardIds.Count;
        }

        // 复制当前弃牌组用于随机选择
        List<string> tempDiscardPile = new List<string>(playerDiscardPileCardIds);

        for (int i = 0; i < count; i++)
        {
            if (tempDiscardPile.Count == 0) break;

            int randomIndex = UnityEngine.Random.Range(0, tempDiscardPile.Count);
            string cardId = tempDiscardPile[randomIndex];
            CardDataSO card = GetCardById(cardId);
            
            if (card != null)
            {
                drawnCards.Add(card);
                // 从临时弃牌组移除
                tempDiscardPile.RemoveAt(randomIndex);
                
                // 如果需要从真实弃牌组移除
                if (removeFromDiscardPile)
                {
                    playerDiscardPileCardIds.Remove(cardId);
                }
            }
        }

        return drawnCards;
    }

    // 向玩家拥有牌添加卡牌
    public void AddCardToPlayerOwnedPile(string cardId, int count = 1)
    {
        CardDataSO card = GetCardById(cardId);
        if (card == null)
        {
            Debug.LogWarning($"未找到卡牌ID: {cardId}");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            playerOwnedCardIds.Add(cardId);
        }

        Debug.Log($"已向玩家拥有牌添加{count}张 {card.cardName}");
    }

    // 向玩家牌组添加卡牌
    public void AddCardToPlayerDeck(string cardId, int count = 1)
    {
        CardDataSO card = GetCardById(cardId);
        if (card == null)
        {
            Debug.LogWarning($"未找到卡牌ID: {cardId}");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            playerDeckCardIds.Add(cardId);
        }

        Debug.Log($"已向牌组添加{count}张 {card.cardName}");
    }

    // 向玩家弃牌组添加卡牌
    public void AddCardToDiscardPile(string cardId, int count = 1)
    {
        CardDataSO card = GetCardById(cardId);
        if (card == null)
        {
            Debug.LogWarning($"未找到卡牌ID: {cardId}");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            playerDiscardPileCardIds.Add(cardId);
        }

        Debug.Log($"已向弃牌组添加{count}张 {card.cardName}");
    }

    // 清空弃牌组
    public void ClearDiscardPile()
    {
        int count = playerDiscardPileCardIds.Count;
        playerDiscardPileCardIds.Clear();
        Debug.Log($"已清空弃牌组，共移除{count}张卡牌");
    }

    // 记录当前玩家牌组（创建备份）
    public void BackupPlayerDeck()
    {
        // 清空现有备份
        deckBackup.Clear();
        
        // 深度复制当前牌组
        foreach (string cardId in playerDeckCardIds)
        {
            deckBackup.Add(cardId);
        }
        
        Debug.Log($"已备份牌组，共{deckBackup.Count}张卡牌");
    }

    // 从备份恢复玩家牌组
    public void RestorePlayerDeckFromBackup()
    {
        if (deckBackup.Count == 0)
        {
            Debug.LogWarning("没有可用的牌组备份");
            return;
        }
        
        // 清空当前牌组
        playerDeckCardIds.Clear();
        
        // 从备份恢复
        foreach (string cardId in deckBackup)
        {
            playerDeckCardIds.Add(cardId);
        }
        
        Debug.Log($"已从备份恢复牌组，共{playerDeckCardIds.Count}张卡牌");
    }

    // 获取玩家当前牌组（返回卡牌数据对象列表）
    public List<CardDataSO> GetPlayerDeck()
    {
        List<CardDataSO> deck = new List<CardDataSO>();

        foreach (string cardId in playerDeckCardIds)
        {
            CardDataSO card = GetCardById(cardId);
            if (card != null)
            {
                deck.Add(card);
            }
            else
            {
                Debug.LogWarning($"牌组中未找到卡牌: {cardId}");
            }
        }

        return deck;
    }

    // 获取玩家弃牌组（返回卡牌数据对象列表）
    public List<CardDataSO> GetPlayerDiscardPile()
    {
        List<CardDataSO> discardPile = new List<CardDataSO>();

        foreach (string cardId in playerDiscardPileCardIds)
        {
            CardDataSO card = GetCardById(cardId);
            if (card != null)
            {
                discardPile.Add(card);
            }
            else
            {
                Debug.LogWarning($"弃牌组中未找到卡牌: {cardId}");
            }
        }

        return discardPile;
    }

    // 获取玩家拥有卡牌（返回卡牌数据对象列表）
    public List<CardDataSO> GetPlayerOwnedCards()
    {
        List<CardDataSO> ownedCards = new List<CardDataSO>();

        foreach (string cardId in playerOwnedCardIds)
        {
            CardDataSO card = GetCardById(cardId);
            if (card != null)
            {
                ownedCards.Add(card);
            }
            else
            {
                Debug.LogWarning($"玩家拥有卡牌中未找到卡牌: {cardId}");
            }
        }

        return ownedCards;
    }

    // 检查是否有牌组备份
    public bool HasDeckBackup()
    {
        return deckBackup != null && deckBackup.Count > 0;
    }

    // 获取备份牌组信息（用于调试或显示）
    public List<CardDataSO> GetBackupDeckInfo()
    {
        List<CardDataSO> backupDeck = new List<CardDataSO>();

        foreach (string cardId in deckBackup)
        {
            CardDataSO card = GetCardById(cardId);
            if (card != null)
            {
                backupDeck.Add(card);
            }
        }

        return backupDeck;
    }

    // 清空牌组备份
    public void ClearDeckBackup()
    {
        deckBackup.Clear();
        Debug.Log("已清空牌组备份");
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