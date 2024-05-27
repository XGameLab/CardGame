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
        isMouseOverMapPoint = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition);
        
        if(isMouseOverMapPoint)
        {
            if(Input.GetMouseButtonDown(0))
            {
                outline.enabled = true;
                // MapPhoto.enabled = true;
                DefaultPhoto.sprite = MapPhoto.sprite;
                // DefaultPhoto.enabled = false;
            }
        }
        // if(!isMouseOverMapPoint)
        // {
        //     outline.enabled = false;
        //     MapPhoto.enabled = false;
        //     DefaultPhoto.enabled = true;
        // }
    }
}
