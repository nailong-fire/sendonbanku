using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCardItem : MonoBehaviour
{
    [Header("UI References")]
    public Image cardImage;
    public TMP_Text nameText;
    public TMP_Text costText;

    /// <summary>
    /// 用 CardDataSO 初始化背包里的卡牌 UI
    /// </summary>
    public void Setup(CardDataSO card)
    {
        if (card == null)
        {
            Debug.LogWarning("[InventoryCardItem] Setup 传入了空的 CardDataSO");
            return;
        }

        // 卡图
        if (cardImage != null)
            cardImage.sprite = card.cardArt;

        // 名字
        if (nameText != null)
            nameText.text = card.cardName;

        // 费用
        if (costText != null)
            costText.text = card.faithCost.ToString();
    }
}
