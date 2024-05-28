using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPoint : MonoBehaviour
{
    private Outline outline;
    private bool isMouseOverMapPoint = false;
    public Image DefaultPhoto;
    public Image MapPhoto;

    void Start()
    {
        outline = GetComponent<Outline>();
        MapPhoto.enabled = false;
        DefaultPhoto.enabled = true;
    }

    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        RectTransform rectTransform = this.transform as RectTransform;

        // 获取Canvas和Camera
        Canvas canvas = GetComponentInParent<Canvas>();
        Camera camera = canvas.worldCamera;

        // 确保使用相机来检测屏幕坐标
        isMouseOverMapPoint = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition, camera);

        if (isMouseOverMapPoint)
        {
            if (Input.GetMouseButtonDown(0))
            {
                outline.enabled = true;
                // MapPhoto.enabled = true;
                DefaultPhoto.sprite = MapPhoto.sprite;
                // DefaultPhoto.enabled = false;
            }
        }
    }
}
