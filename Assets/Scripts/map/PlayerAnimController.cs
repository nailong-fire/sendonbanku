using UnityEngine;

namespace Map
{
    public class PlayerAnimController : MonoBehaviour
    {
        [Header("移动设置")]
        [Tooltip("玩家移动速度")]
        public float moveSpeed = 5f;

        [Header("组件引用")]
        public Animator animator;
        public Rigidbody2D rb;

        // 记录当前朝向状态，默认为向左（因为原图是向左的）
        private bool isFacingLeft = true;
        
        // 控制是否可以移动
        private bool canMove = true;

        void Awake()
        {
            // 自动获取组件
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (animator == null) animator = GetComponent<Animator>();
        }

        void Update()
        {
            if (!canMove)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                if (animator != null) animator.SetBool("IsWalking", false);
                return;
            }

            // 1. 获取输入 (只响应左右)
            float moveInput = 0f;
            // 使用 GetKey 确保按住持续移动
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                moveInput = -1f;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                moveInput = 1f;
            }

            // 2. 应用移动逻辑
            // 保持原有的Y轴速度（如果有重力的话），只改变X轴
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            // 3. 动画状态控制
            bool isMoving = Mathf.Abs(moveInput) > 0.01f;
            if (animator != null)
            {
                // 设置Animator中的参数 "IsWalking"
                animator.SetBool("IsWalking", isMoving);
            }

            // 4. 镜像翻转逻辑
            // 原图朝左：
            // - 向右移动 (moveInput > 0) -> 需要翻转 (Flip)
            // - 向左移动 (moveInput < 0) -> 保持原状 (不翻转)
            if (moveInput > 0 && isFacingLeft)
            {
                Flip();
            }
            else if (moveInput < 0 && !isFacingLeft)
            {
                Flip();
            }
        }

        void Flip()
        {
            // 切换朝向状态
            isFacingLeft = !isFacingLeft;

            // 获取当前缩放
            Vector3 scaler = transform.localScale;
            // X轴取反，实现镜像
            scaler.x *= -1;
            transform.localScale = scaler;
        }

        public void EnableMove(bool enable)
        {
            canMove = enable;
        }
    }
}
