using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintFloat : MonoBehaviour
{
    public float amplitude = 0.08f;   // 浮动高度
    public float speed = 3f;          // 浮动速度

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * speed) * amplitude;
        transform.localPosition = startPos + new Vector3(0, y, 0);
    }
}

