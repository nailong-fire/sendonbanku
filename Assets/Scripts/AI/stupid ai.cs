using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stupidai : MonoBehaviour
{
    private int turn = 0;

    private CardEntity AIDrawCard()
    {
        return null;
    }

    private CardEntity AIPlayCard()
    {
        turn++;
        UniversalController enemy = GameManager.Instance.enemy;
        List<CardEntity> playableCards = enemy.handZone.GetAllCards();

        return null;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
