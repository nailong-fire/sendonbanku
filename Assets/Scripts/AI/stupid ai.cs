using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UnityEngine;

public class stupidai : MonoBehaviour
{
    private int turn = 0;

    private Cardenetity AIDrawCard()
    {
        var handCards = GetComponent<UniversalController>().GetHandCards();
        if (handCards.Count == 0)
            return null;

        // 简单策略：随机选择一张手牌
        int randomIndex = Random.Range(0, handCards.Count);
        return handCards[randomIndex];
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
