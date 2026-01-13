using UnityEngine;

public class SceneSwitchTrigger : MonoBehaviour
{
    public Camera mainCamera; // 在Inspector中拖入Main Camera
    public float moveDistance = 17.82f; // 移动距离，可在Inspector中调整
    private bool isTriggered = false; // 标记是否已触发

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTriggered) // 确保角色GameObject的Tag设置为"Player"，且未触发过
        {
            isTriggered = true; // 设置为已触发
            float playerX = other.transform.position.x;
            float triggerX = transform.position.x;

            if (playerX > triggerX)
            {
                // 从右侧触碰（角色X > Trigger X），Camera向左移动
                mainCamera.transform.position += new Vector3(-moveDistance, 0, 0);
            }
            else
            {
                // 从左侧触碰（角色X <= Trigger X），Camera向右移动
                mainCamera.transform.position += new Vector3(moveDistance, 0, 0);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isTriggered = false; // 重置触发状态
        }
    }
}