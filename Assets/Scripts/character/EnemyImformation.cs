using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyImformation : MonoBehaviour
{
    public static EnemyImformation instance { get; set; }  // ← 单例

    public enemyai enemyAI = null;
    public CardDatabaseSO enemyCardDatabase = null;
    public CardDatabaseSO playerCardDatabase = null;
    public string enemyName = "unknow";

    private void Awake()
    {
        // 单例初始化
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 只在第一次初始化时清空卡牌
            if (playerCardDatabase != null)
            {
                playerCardDatabase.playerDeckCardIds.Clear();
                playerCardDatabase.playerOwnedCardIds.Clear();
            }
            
            Debug.Log("EnemyImformation 第一次初始化");
        }
        else
        {
            Debug.Log("EnemyImformation 检测到重复实例，销毁");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
