using UnityEngine;

public class CardFactory : MonoBehaviour
{
    [Header("预制体")]
    [SerializeField] private GameObject cardPrefab;

    [Header("卡牌数据库")]
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

    // 通过 ID 创建卡牌
    public CardEntity CreateCardById(string cardId, UniversalController owner, Transform parent = null)
    {
        CardDataSO cardData = cardDatabase.GetCardById(cardId);
        if (cardData == null)
        {
            Debug.LogError($"未找到卡牌 ID： {cardId}");
            return null;
        }

        return CreateCard(cardData, owner, parent);
    }

    // 通过 ScriptableObject 创建卡牌
    public CardEntity CreateCard(CardDataSO cardData, UniversalController owner, Transform parent = null)
    {
        if (cardPrefab == null)
        {
            Debug.LogError("卡牌预制体未指定!");
            return null;
        }

        GameObject cardObj = Instantiate(cardPrefab, parent);
        CardEntity cardEntity = cardObj.GetComponent<CardEntity>();

        if (cardEntity == null)
        {
            Debug.LogError("卡牌预制体上缺少 CardEntity 组件!");
            Destroy(cardObj);
            return null;
        }

        cardEntity.Initialize(cardData, owner);
        cardObj.name = $"Card_{cardData.cardName}";

        cardObj.transform.localScale = 0.1f * Vector3.one; // 设置卡牌初始缩放比例

        return cardEntity;
    }

    // 使用运行时数据创建卡牌
    public CardEntity CreateCard(CardRuntimeData runtimeData, PlayerController owner, Transform parent = null)
    {
        // 这里应根据 runtimeData 的 ID 查找对应的 CardDataSO（如需要）
        // 暂时直接用预制体创建并用运行时数据初始化
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


}