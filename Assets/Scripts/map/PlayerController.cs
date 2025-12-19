using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb2d;

    public float moveSpeedX = 2.5f;
    public float moveSpeedY = 1.5f;

    private bool canMove = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove)
        {
            // 停止动画
            animator.SetFloat("speed", 0);
            rb2d.velocity = Vector2.zero;
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0)
        {
            animator.SetFloat("horizontal", horizontal);
            animator.SetFloat("vertical", 0);
        }
        else if (vertical != 0)
        {
            animator.SetFloat("vertical", vertical);
            animator.SetFloat("horizontal", 0);
        }

        Vector2 dir = new Vector2(horizontal, vertical);
        animator.SetFloat("speed", dir.magnitude);

        rb2d.velocity = new Vector2(
            horizontal * moveSpeedX,
            vertical * moveSpeedY
        );
    }

    // ⭐ 对外接口：控制能不能移动
    public void EnableMove(bool enable)
    {
        canMove = enable;

        if (!enable)
        {
            rb2d.velocity = Vector2.zero;
            animator.SetFloat("speed", 0);
        }
    }
}

