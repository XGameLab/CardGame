using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapPoint : MonoBehaviour
{
    private bool isMouseOverStage = false;
    public Image DefaultPhoto;
    public Image[] MapPhotos; // 存储多个关卡图片
    public TextMeshProUGUI StageName; // 用于显示关卡名称的UI
    private int currentStageIndex = 0; // 当前选中的关卡索引

    public Button[] stageButtons; // 存储多个关卡按钮
    public string[] stageNames; // 存储多个关卡名称

    void Start()
    {
        foreach (Image mapPhoto in MapPhotos)
        {
            mapPhoto.enabled = false; // 初始状态下，所有地图图片不可见
        }
        MapPhotos[0].enabled = true; // 默认显示第一个关卡的图片
        DefaultPhoto.enabled = true;
        StageName.text = stageNames[0]; // 默认显示第一个关卡的名称

        // 为每个按钮添加监听器
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int index = i; // 避免闭包问题
            stageButtons[i].onClick.AddListener(() => OnStageButtonClick(index));
        }
    }

    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        RectTransform rectTransform = this.transform as RectTransform;

        Canvas canvas = GetComponentInParent<Canvas>();
        Camera camera = canvas.worldCamera;

        isMouseOverStage = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition, camera);

        if (isMouseOverStage)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DefaultPhoto.sprite = MapPhotos[currentStageIndex].sprite; // 根据当前关卡索引显示相应的图片
            }
        }
    }

    // 按钮点击事件处理方法
    private void OnStageButtonClick(int index)
    {
        currentStageIndex = index;

        // 更新显示的关卡图片和名称
        for (int i = 0; i < MapPhotos.Length; i++)
        {
            MapPhotos[i].enabled = i == index;
        }
        DefaultPhoto.sprite = MapPhotos[index].sprite;
        StageName.text = stageNames[index]; // 设置关卡名称
    }
}
