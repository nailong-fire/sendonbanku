using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFloat : MonoBehaviour
{
    public float speed = 15f;      // 抖动速度（越大越快）
    public float height = 30f;    // 抖动幅度（越大越明显）

    Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * speed) * height;
        transform.localPosition = startPos + new Vector3(0, y, 0);
    }
}

