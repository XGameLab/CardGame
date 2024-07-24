using UnityEngine;
using UnityEngine.UI;

public class ObjectShake : MonoBehaviour
{
    public RectTransform[] targetRectTransforms; // 要震动的对象数组
    public float shakeDuration = 0.5f; // 震动持续时间
    public float shakeAmount = 0.7f; // 震动强度
    public Button submitButton; // 提交按钮

    private Vector2[] originalPositions;
    private bool isSubmitButtonClicked = false;

    void OnEnable()
    {
        // 初始化原始位置数组
        originalPositions = new Vector2[targetRectTransforms.Length];
        
        for (int i = 0; i < targetRectTransforms.Length; i++)
        {
            if (targetRectTransforms[i] != null)
            {
                originalPositions[i] = targetRectTransforms[i].anchoredPosition;
            }
        }

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
        }
    }

    void Update()
    {
        if (isSubmitButtonClicked)
        {
            TriggerShake();
            isSubmitButtonClicked = false; // 重置按钮点击状态
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            TriggerShake();
        }
    }

    void OnSubmitButtonClicked()
    {
        isSubmitButtonClicked = true;
    }

    public void TriggerShake()
    {
        for (int i = 0; i < targetRectTransforms.Length; i++)
        {
            if (targetRectTransforms[i] != null)
            {
                RectTransform target = targetRectTransforms[i];
                
                // 停止当前的所有动画，防止叠加
                LeanTween.cancel(target.gameObject);

                // 还原到原始位置
                target.anchoredPosition = originalPositions[i];

                // 创建震动效果
                LeanTween.move(target, originalPositions[i] + (Vector2)Random.insideUnitCircle * shakeAmount, shakeDuration)
                         .setEase(LeanTweenType.easeShake)
                         .setLoopPingPong(1)
                         .setOnComplete(() => 
                         {
                             // 确保回到初始位置
                             target.anchoredPosition = originalPositions[i];
                         });
            }
        }
    }
}
