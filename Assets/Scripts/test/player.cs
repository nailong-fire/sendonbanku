using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f; // 移动速度
    
    [Header("组件引用")]
    [SerializeField] private Rigidbody2D rb; // Rigidbody2D组件


    private SpriteRenderer sr;
    private bool canMove = true;
    private Vector2 movement; // 存储输入方向
    
    void Start()
    {
        // 如果未手动分配Rigidbody2D，自动获取
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
        
        // 如果没有Rigidbody2D组件，添加一个
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; // 2D游戏通常不需要重力
        }
    }
    
    void Update()
    {
        // 获取输入
        movement.x = Input.GetAxisRaw("Horizontal"); // 水平输入：A/D 或 左右箭头
        //movement.y = Input.GetAxisRaw("Vertical");   // 垂直输入：W/S 或 上下箭头
        
        // 标准化向量，确保斜向移动不会更快
        movement = movement.normalized;
    }
    
    void FixedUpdate()
    {
        // 在FixedUpdate中应用物理移动，确保平滑
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        if (movement.x == -1)
        {
            sr.flipX = false;
        }
        else if (movement.x == 1)
        {
            sr.flipX = true;
        }

    }
    
    // 可选：如果不需要物理碰撞，可以使用Transform直接移动
    void SimpleMove()
    {
        // 使用Transform移动（非物理方式）
        transform.position += new Vector3(movement.x, movement.y, 0) * moveSpeed * Time.deltaTime;
    }
    public void EnableMove(bool enable)
    {
        canMove = enable;

        if (!enable)
        {
            rb.velocity = Vector2.zero;
        }
    }
}