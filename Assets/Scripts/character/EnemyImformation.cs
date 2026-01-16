using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyImformation : MonoBehaviour
{
    public static EnemyImformation instance { get; set; }

    public enemyai enemyAI = null;
    public CardDatabaseSO enemyCardDatabase = null;
    public CardDatabaseSO playerCardDatabase = null;
    public string enemyName = "unknow";
    
    // 备份敌人卡牌数据库
    private List<string> backupEnemyDeckCardIds = new List<string>();
    private List<string> backupEnemyOwnedCardIds = new List<string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 备份敌人卡牌数据库
            if (enemyCardDatabase != null)
            {
                backupEnemyDeckCardIds = new List<string>(enemyCardDatabase.playerDeckCardIds);
                backupEnemyOwnedCardIds = new List<string>(enemyCardDatabase.playerOwnedCardIds);
                Debug.Log("已备份敌人卡牌数据库");
            }
            
            // 清空玩家卡牌
            if (playerCardDatabase != null)
            {
                playerCardDatabase.playerDeckCardIds.Clear();
                playerCardDatabase.playerOwnedCardIds.Clear();
            }
        }
        else
        {
            Debug.Log("EnemyImformation 检测到重复实例，销毁");
            Destroy(gameObject);
        }
    }
    
    // 恢复敌人卡牌数据库
    public void RestoreEnemyCardDatabase()
    {
        if (enemyCardDatabase != null)
        {
            enemyCardDatabase.playerDeckCardIds = new List<string>(backupEnemyDeckCardIds);
            enemyCardDatabase.playerOwnedCardIds = new List<string>(backupEnemyOwnedCardIds);
            Debug.Log("已恢复敌人卡牌数据库");
        }
    }
}
