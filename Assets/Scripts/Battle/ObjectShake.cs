using UnityEngine;
using UnityEngine.UI;

public class ObjectShake : MonoBehaviour
{
    public RectTransform[] targetRectTransforms; // 要震动的对象数组
    public float shakeDuration = 0.5f; // 震动持续时间
    public float shakeAmount = 0.7f; // 震动强度
    public float shakeDelay = 0f; // 震动开始的延迟时间
    public Button submitButton; // 提交按钮

    private Vector2[] originalPositions;

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
    }

    public void TriggerShake()
    {
        for (int i = 0; i < targetRectTransforms.Length; i++)
        {
            if (targetRectTransforms[i] != null)
            {
                RectTransform target = targetRectTransforms[i];
                Vector2 originalPosition = originalPositions[i]; // 捕获当前的原始位置
                
                // 停止当前的所有动画，防止叠加
                LeanTween.cancel(target.gameObject);

                // 还原到原始位置
                target.anchoredPosition = originalPosition;

                // 创建震动效果并延迟执行
                LeanTween.delayedCall(shakeDelay, () => 
                {
                    LeanTween.move(target, originalPosition + (Vector2)Random.insideUnitCircle * shakeAmount, shakeDuration)
                            .setEase(LeanTweenType.easeShake)
                            .setLoopPingPong(1)
                            .setOnComplete(() => 
                            {
                                // 确保回到初始位置
                                target.anchoredPosition = originalPosition;
                            });
                });
            }
        }
    }
}
