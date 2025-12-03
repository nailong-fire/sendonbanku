using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerData playerData;
    public CardZone handCardsZone;
    public CardZone fieldCardsZone;

    // Start is called before the first frame update
    void Start()
    {
        InitializePlayer();
    }

    // 初始化玩家数据
    private void InitializePlayer()
    {
        playerData.hope = playerData.maxHope; // Hope作为玩家血量
        playerData.faith = playerData.maxFaith;
    }

    // 更新玩家Hope血量
    public void UpdateHope(int amount)
    {
        playerData.hope = Mathf.Clamp(playerData.hope + amount, 0, playerData.maxHope);
        // 触发UI更新事件
    }

    // 更新Faith资源
    public void UpdateFaith(int amount)
    {
        playerData.faith = Mathf.Clamp(playerData.faith + amount, 0, playerData.maxFaith);
        // 触发UI更新事件
    }

    // 抽卡
    public void DrawCard(CardDataSO card)
    {
        handCardsZone.AddCard(card);
    }

    // 出牌到场上
    public bool PlayCardToField(CardDataSO card)
    {
        if (fieldCardsZone.CanAddCard(card) && handCardsZone.RemoveCard(card))
        {
            fieldCardsZone.AddCard(card);
            return true;
        }
        return false;
    }
}