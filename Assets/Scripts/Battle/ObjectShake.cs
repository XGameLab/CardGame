using UnityEngine;
using UnityEngine.UI;

public class ObjectShake : MonoBehaviour
{
    public RectTransform[] targetRectTransforms; // 震動させるオブジェクトの配列
    public float shakeDuration = 0.5f; // 震動の持続時間
    public float shakeAmount = 0.7f; // 震動の強度
    public float shakeDelay = 0f; // 震動開始の遅延時間
    public Button submitButton; // 提出ボタン

    private Vector2[] originalPositions; // オリジナルの位置を保存するための配列

    void OnEnable()
    {
        // オリジナルの位置配列を初期化
        originalPositions = new Vector2[targetRectTransforms.Length];
        
        for (int i = 0; i < targetRectTransforms.Length; i++)
        {
            if (targetRectTransforms[i] != null)
            {
                originalPositions[i] = targetRectTransforms[i].anchoredPosition; // 元の位置を保存
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
                Vector2 originalPosition = originalPositions[i]; // 現在の元の位置をキャプチャ
                
                // 現在のすべてのアニメーションをキャンセルして重複を防ぐ
                LeanTween.cancel(target.gameObject);

                // 元の位置にリセット
                target.anchoredPosition = originalPosition;

                // 震動効果を作成し、遅延実行
                LeanTween.delayedCall(shakeDelay, () => 
                {
                    LeanTween.move(target, originalPosition + (Vector2)Random.insideUnitCircle * shakeAmount, shakeDuration)
                            .setEase(LeanTweenType.easeShake)
                            .setLoopPingPong(1) // ピンポンループを1回設定
                            .setOnComplete(() => 
                            {
                                // 初期位置に戻ることを確認
                                target.anchoredPosition = originalPosition;
                            });
                });
            }
        }
    }
}
