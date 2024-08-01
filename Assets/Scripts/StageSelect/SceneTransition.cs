using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDurationFast = 0.3f;
    public float fadeDurationSlow = 0.5f;

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public void StartFadeOutFast()
    {
        StartCoroutine(FadeOutFast());
    }

    public void StartFadeOutSlow()
    {
        StartCoroutine(FadeOutSlow());
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < fadeDurationFast)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDurationFast);
            yield return null;
        }
        canvasGroup.alpha = 0;
    }

    private IEnumerator FadeOutFast()
    {
        float timer = 0f;
        while (timer < fadeDurationFast)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDurationFast);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOutSlow()
    {
        float timer = 0f;
        while (timer < fadeDurationSlow)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDurationSlow);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }
}
