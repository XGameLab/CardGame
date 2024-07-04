using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonArrayHandler : MonoBehaviour
{
    public Button[] buttons;
    public RawImage[] images;
    public RawImage[] stageIMG;
    public RawImage littleBar;
    public RawImage bigBar;
    public RawImage circle;
    public GameObject targetObject;
    public GameObject[] startButtons; // 修改这里
    public GameObject buttonBG;
    public float moveSpeed = 5f;
    public float sizeChangeSpeed = 10f;
    public Button initialButton;
    public Button buttonL;
    public Button buttonR;
    public Button buttonM;
    public Button buttonQuit;
    public TextMeshProUGUI tmpText;
    public Canvas canvas;
    public float buttonSpacing = 200f;
    public float imageSpacing = 1100f;
    public string[] buttonTexts;
    public string[] titles;
    public TextMeshProUGUI[] stageTexts;
    public TextMeshProUGUI titleText;

    private Vector3 targetPosition;
    private Color targetColor = new Color(1f, 0.933f, 0.396f, 1f); // #FFEE65
    private Vector2 defaultSize = new Vector2(60f, 60f);
    private Vector2 clickedSize = new Vector2(100f, 100f);
    private Button lastClickedButton;
    private Vector2[] buttonTargetPositions;
    private Vector2[] imageTargetPositions;
    private int lastSelectedIndex = 0;

    private SceneTransition sceneTransition;
    private float scrollCooldown = 0.15f; // 滚动延迟时间
    private float scrollTimer = 0f; // 滚动计时器

    void Start()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        }

        buttonL.onClick.AddListener(MoveLeft);
        buttonR.onClick.AddListener(MoveRight);
        buttonM.onClick.AddListener(EnterStage);
        buttonM.onClick.AddListener(TriggerSceneTransition2);
        buttonQuit.onClick.AddListener(ExitStage);
        buttonQuit.onClick.AddListener(TriggerSceneTransition1);

        targetPosition = targetObject.transform.position;

        if (initialButton != null)
        {
            SetButtonColorAndSize(initialButton, targetColor, clickedSize);
            lastClickedButton = initialButton;
        }

        buttonTargetPositions = new Vector2[buttons.Length];
        imageTargetPositions = new Vector2[images.Length];
        UpdateButtonAndImagePositions(initialButton, true);

        if (buttonTexts.Length != buttons.Length)
        {
            Debug.LogError("buttonTexts 数组的长度必须与 buttons 数组的长度一致！");
        }

        if (titles.Length != buttons.Length || stageTexts.Length != buttons.Length)
        {
            Debug.LogError("titles 和 stageTexts 数组的长度必须与 buttons 数组的长度一致！");
        }

        buttonQuit.gameObject.SetActive(false);
        foreach (var startButton in startButtons) // 修改这里
        {
            startButton.gameObject.SetActive(false);
        }
        littleBar.gameObject.SetActive(false);
        bigBar.gameObject.SetActive(true);
        circle.gameObject.SetActive(false);
        foreach (var text in stageTexts)
        {
            text.gameObject.SetActive(false);
        }
        titleText.gameObject.SetActive(false);

        GameObject transitionCanvas = GameObject.Find("Main Camera");
        if (transitionCanvas != null)
        {
            sceneTransition = transitionCanvas.GetComponent<SceneTransition>();
        }
    }

    void Update()
    {
        targetObject.transform.position = Vector3.Lerp(targetObject.transform.position, targetPosition, moveSpeed * Time.deltaTime);

        foreach (Button button in buttons)
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            if (button == lastClickedButton)
            {
                rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, clickedSize, sizeChangeSpeed * Time.deltaTime);
            }
            else
            {
                rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, defaultSize, sizeChangeSpeed * Time.deltaTime);
            }

            int index = System.Array.IndexOf(buttons, button);
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, buttonTargetPositions[index], moveSpeed * Time.deltaTime);
        }

        foreach (RawImage image in images)
        {
            RectTransform rectTransform = image.GetComponent<RectTransform>();
            int index = System.Array.IndexOf(images, image);
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, imageTargetPositions[index], moveSpeed * Time.deltaTime);
        }

        // 检测鼠标滚轮输入并更新按钮和图片位置
        scrollTimer += Time.deltaTime;
        if (scrollTimer >= scrollCooldown)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.05f)
            {
                int newIndex = lastSelectedIndex + (scroll > 0 ? -1 : 1);
                if (newIndex >= 0 && newIndex < buttons.Length)
                {
                    OnButtonClick(buttons[newIndex]);
                    scrollTimer = 0f; // 重置计时器
                }
            }
        }
    }

    void OnButtonClick(Button clickedButton)
    {
        UpdateButtonAndImagePositions(clickedButton, false);

        int clickedIndex = System.Array.IndexOf(buttons, clickedButton);
        lastSelectedIndex = clickedIndex;

        if (clickedIndex >= 0 && clickedIndex < buttonTexts.Length)
        {
            tmpText.text = buttonTexts[clickedIndex];
        }

        foreach (Button button in buttons)
        {
            if (button == clickedButton)
            {
                SetButtonColor(button, targetColor);
                lastClickedButton = clickedButton;
            }
            else
            {
                SetButtonColor(button, Color.white);
            }
        }
    }

    void MoveLeft()
    {
        if (lastClickedButton != buttons[0])
        {
            int currentIndex = System.Array.IndexOf(buttons, lastClickedButton);
            OnButtonClick(buttons[currentIndex - 1]);
        }
    }

    void MoveRight()
    {
        if (lastClickedButton != buttons[buttons.Length - 1])
        {
            int currentIndex = System.Array.IndexOf(buttons, lastClickedButton);
            OnButtonClick(buttons[currentIndex + 1]);
        }
    }

    void EnterStage()
    {
        if (lastSelectedIndex >= 0 && lastSelectedIndex < stageIMG.Length)
        {
            stageIMG[lastSelectedIndex].gameObject.SetActive(true);
        }
        buttonQuit.gameObject.SetActive(true);
        buttonM.gameObject.SetActive(false);
        startButtons[lastSelectedIndex].gameObject.SetActive(true); // 修改这里
        buttonBG.gameObject.SetActive(true);

        foreach (RawImage image in images)
        {
            image.gameObject.SetActive(false);
        }

        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(false);
        }

        targetObject.gameObject.SetActive(false);
        littleBar.gameObject.SetActive(true);
        bigBar.gameObject.SetActive(false);
        circle.gameObject.SetActive(true);

        if (lastSelectedIndex >= 0 && lastSelectedIndex < titles.Length && lastSelectedIndex < stageTexts.Length)
        {
            titleText.text = titles[lastSelectedIndex];
            titleText.gameObject.SetActive(true);
            stageTexts[lastSelectedIndex].gameObject.SetActive(true);
        }
    }

    void ExitStage()
    {
        if (lastSelectedIndex >= 0 && lastSelectedIndex < stageIMG.Length)
        {
            stageIMG[lastSelectedIndex].gameObject.SetActive(false);
        }
        buttonQuit.gameObject.SetActive(false);
        buttonM.gameObject.SetActive(true);
        startButtons[lastSelectedIndex].gameObject.SetActive(false); // 修改这里
        buttonBG.gameObject.SetActive(false);

        foreach (RawImage image in images)
        {
            image.gameObject.SetActive(true);
        }

        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(true);
        }

        targetObject.gameObject.SetActive(true);
        littleBar.gameObject.SetActive(false);
        bigBar.gameObject.SetActive(true);
        circle.gameObject.SetActive(false);

        titleText.gameObject.SetActive(false);
        foreach (var text in stageTexts)
        {
            text.gameObject.SetActive(false);
        }
    }

    void TriggerSceneTransition1()
    {
        if (sceneTransition != null)
        {
            sceneTransition.StartFadeOutFast();
        }
        else
        {
            Debug.LogError("SceneTransition component not found.");
        }
    }

    void TriggerSceneTransition2()
    {
        if (sceneTransition != null)
        {
            sceneTransition.StartFadeOutSlow();
        }
        else
        {
            Debug.LogError("SceneTransition component not found.");
        }
    }

    void UpdateButtonAndImagePositions(Button clickedButton, bool initialSetup)
    {
        int clickedIndex = System.Array.IndexOf(buttons, clickedButton);
        for (int i = 0; i < buttons.Length; i++)
        {
            float newX = (i - clickedIndex) * buttonSpacing;
            buttonTargetPositions[i] = new Vector2(newX, buttons[i].GetComponent<RectTransform>().anchoredPosition.y);

            if (initialSetup)
            {
                RectTransform rectTransform = buttons[i].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = buttonTargetPositions[i];
            }
        }

        for (int i = 0; i < images.Length; i++)
        {
            float newX = (i - clickedIndex) * imageSpacing;
            imageTargetPositions[i] = new Vector2(newX, images[i].GetComponent<RectTransform>().anchoredPosition.y);

            if (initialSetup)
            {
                RectTransform rectTransform = images[i].GetComponent<RectTransform>();
                rectTransform.anchoredPosition = imageTargetPositions[i];
            }
        }
    }

    void SetButtonColor(Button button, Color color)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = color;
        colors.pressedColor = color;
        colors.selectedColor = color;
        button.colors = colors;
    }

    void SetButtonColorAndSize(Button button, Color color, Vector2 size)
    {
        SetButtonColor(button, color);
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
    }
}
