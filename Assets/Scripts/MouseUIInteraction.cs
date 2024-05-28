using UnityEngine;
using UnityEngine.UI;

public class MouseUIInteraction : MonoBehaviour
{
    public static bool isMouseOverUI = false;
    private Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();
    }

    void Update()
    {
        if (OnDragCard.isSelected)
        {
            Vector2 mousePosition = Input.mousePosition;
            RectTransform rectTransform = this.transform as RectTransform;

            // 获取Canvas和Camera
            Canvas canvas = GetComponentInParent<Canvas>();
            Camera camera = canvas.worldCamera;

            // 确保使用相机来检测屏幕坐标
            isMouseOverUI = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition, camera);

            if (isMouseOverUI)
            {
                outline.enabled = true;
                // Debug.Log("鼠标在当前物体上并且松开鼠标左键！");
            }
            else
            {
                outline.enabled = false;
            }
            // Debug.Log(isMouseOverUI);
        }
        else
        {
            outline.enabled = false;
        }
    }
}
