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
        List<CardEntity> battlefieldCards = GetComponent<UniversalController>().GetBattlefieldCards();
        foreach (var card in battlefieldCards)
        {
            if (card.CardData.Power > 0 && !card.HasActedThisTurn && card.HasActionAbility)
            {
                return card;
            }
        }
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
