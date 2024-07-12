using UnityEngine;

public class ShakeOnKeyPress : MonoBehaviour
{
    private CameraShake cameraShake;

    void Start()
    {
        // 获取 CameraShake 组件
        cameraShake = GetComponent<CameraShake>();
        
        // 确保 CameraShake 组件存在
        if (cameraShake == null)
        {
            Debug.LogError("CameraShake component not found on the parent object.");
        }
    }

    void Update()
    {
        // 检测按下 "S" 键
        if (Input.GetKeyDown(KeyCode.S))
        {
            // 触发父对象震动
            if (cameraShake != null)
            {
                cameraShake.TriggerShake();
            }
        }
    }
}
