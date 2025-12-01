using UnityEngine;

public class ClickMoveController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public bool isSelected = false;

    void Update()
    {
        // 鼠标点击检测
        HandleSelection();

        // 只有选中状态才能移动
        if (isSelected)
        {
            HandleMovement();
        }
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0)) // 0表示鼠标左键
        {
            // 将鼠标位置转换为世界坐标（2D）
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 使用射线检测判断是否点击到该物体
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            // 如果点击到当前物体则切换选中状态
            isSelected = (hit.collider != null && hit.collider.gameObject == this.gameObject);
            if (hit.collider != null)
            {
                Debug.Log($"Clicked on: {hit.collider.gameObject.name}" + " 1 ");
            }

        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical);

        // 限制对角线移动速度
        if (movement.magnitude > 1)
            movement.Normalize();

        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }

}