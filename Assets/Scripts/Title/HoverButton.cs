using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverButton : MonoBehaviour
{
    public Button[] buttons; // ボタン配列
    public string startSceneName; // Startボタンに対応するシーン名
    public string miniGameSceneName; // MiniGameボタンに対応するシーン名
    public GameObject optionPanel; // Optionボタンに対応するパネル
    public RawImage creditsImage; // Creditsボタンに対応するRaw Image

    public float hoverAlpha = 1f; // ホバー時のAlpha値
    public float hoverScale = 1.2f; // ホバー時のスケール
    public float scaleDuration = 0.2f; // スケールの変化にかかる時間

    private Color[] originalColors; // ボタンの元の色を保持
    private Vector3[] originalScales; // ボタンの元のスケールを保持
    private Coroutine[] scaleCoroutines; // スケールの変更を行うためのコルーチン

    private void Awake()
    {
        originalColors = new Color[buttons.Length];
        originalScales = new Vector3[buttons.Length];
        scaleCoroutines = new Coroutine[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // インデックスをローカル変数に保存
            Image buttonImage = buttons[index].GetComponent<Image>();
            if (buttonImage != null)
            {
                originalColors[index] = buttonImage.color;
            }

            RectTransform rectTransform = buttons[index].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                originalScales[index] = rectTransform.localScale;
            }

            // イベントトリガーを追加
            EventTrigger trigger = buttons[index].gameObject.AddComponent<EventTrigger>();

            // マウスオーバーイベント
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { OnPointerEnter(index); });
            trigger.triggers.Add(entryEnter);

            // マウスアウトイベント
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => { OnPointerExit(index); });
            trigger.triggers.Add(entryExit);

            // ボタンクリックイベント
            buttons[index].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    private void OnDisable()
    {
        // シーン切り替えやオブジェクト破棄時にコルーチンとイベントをクリア
        for (int i = 0; i < buttons.Length; i++)
        {
            if (scaleCoroutines[i] != null)
            {
                StopCoroutine(scaleCoroutines[i]);
                scaleCoroutines[i] = null;
            }

            if (buttons[i] != null)
            {
                buttons[i].onClick.RemoveAllListeners();
            }
        }
    }

    public void OnPointerEnter(int index)
    {
        Image buttonImage = buttons[index].GetComponent<Image>();
        if (buttonImage != null)
        {
            // Alphaを指定のhoverAlphaに設定
            Color newColor = originalColors[index];
            newColor.a = hoverAlpha;
            buttonImage.color = newColor;
        }

        RectTransform rectTransform = buttons[index].GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // 現在のスケールコルーチンを停止
            if (scaleCoroutines[index] != null)
            {
                StopCoroutine(scaleCoroutines[index]);
                scaleCoroutines[index] = null;
            }
            // 新しいスケールコルーチンを開始
            scaleCoroutines[index] = StartCoroutine(ScaleButton(rectTransform, originalScales[index], originalScales[index] * hoverScale, scaleDuration));
        }
    }

    public void OnPointerExit(int index)
    {
        Image buttonImage = buttons[index].GetComponent<Image>();
        if (buttonImage != null)
        {
            // 元の色に戻す
            buttonImage.color = originalColors[index];
        }

        RectTransform rectTransform = buttons[index].GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // 現在のスケールコルーチンを停止
            if (scaleCoroutines[index] != null)
            {
                StopCoroutine(scaleCoroutines[index]);
                scaleCoroutines[index] = null;
            }
            // 新しいスケールコルーチンを開始
            scaleCoroutines[index] = StartCoroutine(ScaleButton(rectTransform, rectTransform.localScale, originalScales[index], scaleDuration));
        }
    }

    public void OnButtonClick(int index)
    {
        // 各ボタンの機能を実装
        switch (index)
        {
            case 0: // Startボタン
                if (!string.IsNullOrEmpty(startSceneName))
                {
                    SceneManager.LoadScene(startSceneName);
                }
                break;
            case 1: // MiniGameボタン
                if (!string.IsNullOrEmpty(miniGameSceneName))
                {
                    SceneManager.LoadScene(miniGameSceneName);
                }
                break;
            case 2: // Optionボタン
                if (optionPanel != null)
                {
                    optionPanel.SetActive(true);
                }
                break;
            case 3: // Creditsボタン
                if (creditsImage != null)
                {
                    creditsImage.gameObject.SetActive(true);
                    GameStateManager.Instance.AudioSource.mute = true;
                }
                break;
            case 4: // Exitボタン
                Application.Quit();
                break;
            case 5: // Backボタン
                if (optionPanel != null)
                {
                    optionPanel.SetActive(false);
                }
                break;
            case 6: // Creditsボタンで表示されたRaw Imageを非表示にする
                if (creditsImage != null)
                {
                    creditsImage.gameObject.SetActive(false);
                    GameStateManager.Instance.AudioSource.mute = false;
                }
                break;
            default:
                Debug.Log("Button clicked: " + buttons[index].name);
                break;
        }
    }

    private IEnumerator ScaleButton(RectTransform rectTransform, Vector3 fromScale, Vector3 toScale, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (rectTransform == null)
            {
                yield break; // コルーチン中にオブジェクトが破棄された場合、処理を中止
            }
            rectTransform.localScale = Vector3.Lerp(fromScale, toScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (rectTransform != null)
        {
            rectTransform.localScale = toScale;
        }
    }
}
