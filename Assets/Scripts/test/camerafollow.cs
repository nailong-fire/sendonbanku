using UnityEngine;

public class CameraFollowX : MonoBehaviour
{
    [Header("跟随目标")]
    [SerializeField] private Transform target; // 要跟随的目标（角色）
    
    [Header("跟随设置")]
    [SerializeField] private float smoothSpeed = 0.125f; // 平滑跟随速度
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f); // 相机偏移量
    
    private void Start()
    {
        // 如果没有手动指定目标，尝试查找玩家标签的对象
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogWarning("CameraFollowX: 没有找到跟随目标，请在Inspector中手动分配或给玩家添加'Player'标签");
            }
        }
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        // 计算目标位置（只在X轴上跟随）
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, transform.position.z);
        
        // 平滑移动到目标位置
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition + offset, smoothSpeed * Time.deltaTime);
        
        // 应用位置
        transform.position = smoothedPosition;
    }
    
    // 公共方法：动态设置跟随目标
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}