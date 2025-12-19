using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    void Start()
    {
        GameInitializer gameInitializer = FindObjectOfType<GameInitializer>();
        gameInitializer.gameObject.SetActive(true);
    }
}
