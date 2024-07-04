using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageAnime : MonoBehaviour
{
    public Button[] buttons; // 用于存储按钮的数组
    public GameObject targetObject; // 需要移动的GameObject
    public Sprite newButtonSprite; // 新的按钮图片

    void Start()
    {
        // 为每个按钮添加点击事件监听器
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => ChangePositionAndImage(button));
        }
    }

    void ChangePositionAndImage(Button clickedButton)
    {
        // 获取被点击按钮的x坐标
        float buttonX = clickedButton.transform.position.x;

        // 设置目标GameObject的x坐标
        Vector3 targetPosition = targetObject.transform.position;
        targetPosition.x = buttonX;
        targetObject.transform.position = targetPosition;

        // 改变按钮的图片
        Image buttonImage = clickedButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.sprite = newButtonSprite;

            // 确保Image覆盖TextMeshPro
            TextMeshProUGUI tmpText = clickedButton.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
            {
                // 将TextMeshPro的父级设置为按钮的父级，确保Image在TextMeshPro的上层
                tmpText.transform.SetParent(clickedButton.transform.parent, false);
                tmpText.transform.SetSiblingIndex(buttonImage.transform.GetSiblingIndex() + 1);
            }
        }
    }
}
