using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryCardItem : MonoBehaviour
{
    [Header("UI References")]
    public Image cardImage;          // 你这个一般就是 CardImage（背景）
    public Image artImage;           // 可选：如果你想显示 cardArt，就再做一个子物体 Image
    public TMP_Text nameText;
    public TMP_Text costText;

    public void Setup(CardDataSO card)
    {
        if (card == null) return;

        // 文字
        if (nameText != null) nameText.text = card.cardName;
        if (costText != null) costText.text = card.faithCost.ToString();

        // 背景图：只有 cardBackground 不为空才覆盖，否则保持 prefab 自带的 Source Image
        if (cardImage != null && card.cardBackground != null)
            cardImage.sprite = card.cardBackground;

        // 卡图（可选）
        if (artImage != null && card.cardArt != null)
            artImage.sprite = card.cardArt;
    }
}
