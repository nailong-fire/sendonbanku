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
        }
        else
        {
            Destroy(gameObject);
        }

        playerCardDatabase.playerDeckCardIds.Clear();
        playerCardDatabase.playerOwnedCardIds.Clear();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
