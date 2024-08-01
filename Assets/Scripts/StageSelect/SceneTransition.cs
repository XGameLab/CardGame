using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public CanvasGroup canvasGroup; // キャンバスグループのアルファ値を変更してフェード効果を実現
    public float fadeDurationFast = 0.3f; // 速いフェードアウトの持続時間
    public float fadeDurationSlow = 0.5f; // 遅いフェードアウトの持続時間

    // フェードインを開始
    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    // 速いフェードアウトを開始
    public void StartFadeOutFast()
    {
        StartCoroutine(FadeOutFast());
    }

    // 遅いフェードアウトを開始
    public void StartFadeOutSlow()
    {
        StartCoroutine(FadeOutSlow());
    }

    // フェードイン効果を実現するコルーチン
    private IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < fadeDurationFast)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDurationFast); // アルファ値を1から0に変更
            yield return null;
        }
        canvasGroup.alpha = 0; // 完全に透明にする
    }

    // 速いフェードアウト効果を実現するコルーチン
    private IEnumerator FadeOutFast()
    {
        float timer = 0f;
        while (timer < fadeDurationFast)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDurationFast); // アルファ値を0から1に変更
            yield return null;
        }
        canvasGroup.alpha = 1; // 完全に不透明にする
    }

    // 遅いフェードアウト効果を実現するコルーチン
    private IEnumerator FadeOutSlow()
    {
        float timer = 0f;
        while (timer < fadeDurationSlow)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDurationSlow); // アルファ値を0から1に変更
            yield return null;
        }
        canvasGroup.alpha = 1; // 完全に不透明にする
    }
}
