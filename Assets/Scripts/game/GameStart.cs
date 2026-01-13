using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    void Start()
    {
        // 只允许在 cardbattle 场景中启动战斗系统
        if (SceneManager.GetActiveScene().name != "cardbattle")
        {
            return;
        }

        GameInitializer gameInitializer = FindObjectOfType<GameInitializer>();
        if (gameInitializer != null)
        {
            gameInitializer.gameObject.SetActive(true);
        }
    }
}

