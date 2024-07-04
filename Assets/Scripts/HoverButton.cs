using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class HoverButton : MonoBehaviour
{
    public Button[] buttons; // 按钮数组
    public string startSceneName;  // Start按钮对应的场景名称
    public GameObject optionPanel; // Option按钮对应的面板
    public RawImage creditsImage; // Credits按钮对应的Raw Image

    public float hoverAlpha = 1f; // 悬浮时的Alpha值
    public float hoverScale = 1.2f; // 悬浮时的缩放比例
    public float scaleDuration = 0.2f; // 缩放持续时间

    private Color[] originalColors;
    private Vector3[] originalScales;
    private Coroutine[] scaleCoroutines;

    private void Awake()
    {
        originalColors = new Color[buttons.Length];
        originalScales = new Vector3[buttons.Length];
        scaleCoroutines = new Coroutine[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // 将索引存储在局部变量中
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

            // 添加事件监听器
            EventTrigger trigger = buttons[index].gameObject.AddComponent<EventTrigger>();

            // 鼠标进入事件
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { OnPointerEnter(index); });
            trigger.triggers.Add(entryEnter);

            // 鼠标离开事件
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => { OnPointerExit(index); });
            trigger.triggers.Add(entryExit);

            // 按钮点击事件
            buttons[index].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    public void OnPointerEnter(int index)
    {
        Image buttonImage = buttons[index].GetComponent<Image>();
        if (buttonImage != null)
        {
            // 设置Alpha为指定的hoverAlpha
            Color newColor = originalColors[index];
            newColor.a = hoverAlpha;
            buttonImage.color = newColor;
        }

        RectTransform rectTransform = buttons[index].GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // 停止当前缩放协程
            if (scaleCoroutines[index] != null)
            {
                StopCoroutine(scaleCoroutines[index]);
            }
            // 启动新的缩放协程
            scaleCoroutines[index] = StartCoroutine(ScaleButton(rectTransform, originalScales[index], originalScales[index] * hoverScale, scaleDuration));
        }
    }

    public void OnPointerExit(int index)
    {
        Image buttonImage = buttons[index].GetComponent<Image>();
        if (buttonImage != null)
        {
            // 还原原始颜色
            buttonImage.color = originalColors[index];
        }

        RectTransform rectTransform = buttons[index].GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // 停止当前缩放协程
            if (scaleCoroutines[index] != null)
            {
                StopCoroutine(scaleCoroutines[index]);
            }
            // 启动新的缩放协程
            scaleCoroutines[index] = StartCoroutine(ScaleButton(rectTransform, rectTransform.localScale, originalScales[index], scaleDuration));
        }
    }

    public void OnButtonClick(int index)
    {
        // 点击 Start 按钮（索引为0）时跳转场景
        if (index == 0 && !string.IsNullOrEmpty(startSceneName))
        {
            SceneManager.LoadScene(startSceneName);
        }
        else if (index == 1 && optionPanel != null) // 点击 Option 按钮（索引为1）时打开面板
        {
            optionPanel.SetActive(true);
        }
        else if (index == 2 && creditsImage != null) // 点击 Credits 按钮（索引为2）时显示Raw Image
        {
            creditsImage.gameObject.SetActive(true);
        }
        else if (index == 3) // 点击 Exit 按钮（索引为3）时退出游戏
        {
            Application.Quit();
        }
        else
        {
            Debug.Log("Button clicked: " + buttons[index].name);
        }
    }

    private IEnumerator ScaleButton(RectTransform rectTransform, Vector3 fromScale, Vector3 toScale, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            rectTransform.localScale = Vector3.Lerp(fromScale, toScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.localScale = toScale;
    }
}
