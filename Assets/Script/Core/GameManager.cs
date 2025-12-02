using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerController player1Prefab;
    public PlayerController player2Prefab;
    
    private PlayerController player1;
    private PlayerController player2;

    void Start()
    {
        // 实例化玩家
        player1 = Instantiate(player1Prefab, new Vector3(-5, 0, 0), Quaternion.identity);
        player2 = Instantiate(player2Prefab, new Vector3(5, 0, 0), Quaternion.identity);
        
        // 设置玩家名称
        player1.playerData.playerName = "Player 1";
        player2.playerData.playerName = "Player 2";
        
        // 初始化卡组
        InitializeDecks();
    }
    
    // 初始化卡组
    private void InitializeDecks()
    {
        // 从CardDatabase获取初始卡组
        List<CardDataSO> player1Deck = CardDatabaseSO.Instance.GetStarterDeck();
        List<CardDataSO> player2Deck = CardDatabaseSO.Instance.GetStarterDeck();
        
        // 给玩家发初始手牌
        foreach (CardDataSO card in player1Deck.GetRange(0, 5))
        {
            player1.DrawCard(card);
        }
        
        foreach (CardDataSO card in player2Deck.GetRange(0, 5))
        {
            player2.DrawCard(card);
        }
    }
}