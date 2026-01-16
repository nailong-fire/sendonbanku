using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyImformation : MonoBehaviour
{
    public static EnemyImformation instance { get; set; }  // ← 单例

    public enemyai enemyAI = null;
    public CardDatabaseSO enemyCardDatabase = null;

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
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
