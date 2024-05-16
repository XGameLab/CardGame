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
        if(OnDragCard.isSelected == true)
        {
            Vector2 mousePosition = Input.mousePosition;
            RectTransform rectTransform = this.transform as RectTransform;
            isMouseOverUI = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition);

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
        if(OnDragCard.isSelected == false)
        {
            outline.enabled = false;
        }
    }
}