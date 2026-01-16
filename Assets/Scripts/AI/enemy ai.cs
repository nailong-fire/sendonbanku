using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyai : MonoBehaviour
{
    private int turn = 0;

    private CardEntity AIDrawCard()
    {
        return null;
    }

    public virtual void AIPlayCard()
    {
        Debug.Log($"Enemy AI第 {turn} 回合行动开始...");
        return;
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
