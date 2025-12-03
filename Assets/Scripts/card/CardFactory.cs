using UnityEngine;

public class CardFactory : MonoBehaviour
{
    [Header("预制体")]
    [SerializeField] private GameObject cardPrefab;

    [Header("数据库")]
    [SerializeField] private CardDatabaseSO cardDatabase;

    private static CardFactory _instance;
    public static CardFactory Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 通过ID创建卡牌
    public CardEntity CreateCardById(string cardId, PlayerController owner, Transform parent = null)
    {
        CardDataSO cardData = cardDatabase.GetCardById(cardId);
        if (cardData == null)
        {
            Debug.LogError($"卡牌ID不存在: {cardId}");
            return null;
        }

        return CreateCard(cardData, owner, parent);
    }

    // 通过ScriptableObject创建卡牌
    public CardEntity CreateCard(CardDataSO cardData, PlayerController owner, Transform parent = null)
    {
        if (cardPrefab == null)
        {
            Debug.LogError("卡牌预制体未设置!");
            return null;
        }

        GameObject cardObj = Instantiate(cardPrefab, parent);
        CardEntity cardEntity = cardObj.GetComponent<CardEntity>();

        if (cardEntity == null)
        {
            Debug.LogError("卡牌预制体上没有CardEntity组件!");
            Destroy(cardObj);
            return null;
        }

        cardEntity.Initialize(cardData, owner);
        cardObj.name = $"Card_{cardData.cardName}";

        return cardEntity;
    }

    // 创建运行时数据卡牌
    public CardEntity CreateCard(CardRuntimeData runtimeData, PlayerController owner, Transform parent = null)
    {
        // 这里需要根据ID查找对应的CardDataSO
        // 暂时先创建一个简单的卡牌
        GameObject cardObj = Instantiate(cardPrefab, parent);
        CardEntity cardEntity = cardObj.GetComponent<CardEntity>();
        cardEntity.Initialize(runtimeData, owner);

        return cardEntity;
    }

    // 创建随机卡牌
    public CardEntity CreateRandomCard(PlayerController owner, Transform parent = null)
    {
        CardDataSO randomCard = cardDatabase.GetRandomCard();
        if (randomCard == null) return null;

        return CreateCard(randomCard, owner, parent);
    }

    // 创建初始卡组
    public CardEntity[] CreateStarterDeck(PlayerController owner, Transform parent = null)
    {
        var starterDeck = cardDatabase.GetStarterDeck();
        CardEntity[] cards = new CardEntity[starterDeck.Count];

        for (int i = 0; i < starterDeck.Count; i++)
        {
            cards[i] = CreateCard(starterDeck[i], owner, parent);
        }

        return cards;
    }
}