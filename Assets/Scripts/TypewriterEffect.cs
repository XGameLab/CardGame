using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 使用 TextMeshPro
using UnityEngine.UI; // 使用 UI
using System.IO;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // 使用 TextMeshProUGUI
    public TextMeshProUGUI speakerNameText; // 用于显示说话人姓名的 TextMeshProUGUI
    public Image speakerImage; // 用于显示说话人图像的 Image 组件
    public Image background; // 用于显示背景的 Image 组件
    public float typingSpeed = 0.05f;
    public GameObject battleStart;
    public GameObject continueGame; // 新添加的 GameObject
    public GameObject logObject; 
    public Button historyButton;
    public Button skipButton; // 跳过按钮
    public Button logButton; // 新添加的按钮
    public GameObject historyPanel;
    public TextMeshProUGUI historyText; // 用于显示历史记录的 TextMeshProUGUI
    public string[] fileNames; // 公共变量数组，用于指定不同的文本文件名（无扩展名）
    private string separator = "\n"; // 指定的分隔符
    public AudioSource backupAudioSource; // 备用AudioSource
    public AudioClip logSound;

    private List<DialogueLine> dialogueLines;
    private List<string> dialogueHistory = new List<string>(); // 用于存储对话历史记录
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool skip = false; // 用于跳过剧情
    private bool isViewingHistory = false; // 用于标记是否正在查看历史记录
    private int historyStartIndex = 0; // 历史记录显示的起始索引
    private int historyViewCount = 6; // 每次显示的历史记录行数
    private string lastBackground = "";
    private string lastSoundEffect = "";
    private string lastBgm = "";
    private bool shouldStartBattle = false; // 新增的标志变量
    private GameStateManager gameStateManager;

    // 定义说话人颜色和图像的字典
    private Dictionary<string, Color> speakerColors = new Dictionary<string, Color>
    {
        { "Wata", new Color(255 / 255f, 215 / 255f, 0 / 255f) },
        { "X", new Color(127 / 255f, 255 / 255f, 212 / 255f) }, // rgb(127, 255, 212)
        { "Tai", new Color(135 / 255f, 246 / 255f, 0 / 255f) },
        { "NPC", Color.green }, // 旁白颜色
        // 可以根据需要添加更多说话人
    };

    private Dictionary<string, Sprite> speakerImages = new Dictionary<string, Sprite>();

    void Start()
    {
        // 检查是否正确分配了 TextMeshPro 组件
        if (dialogueText == null || speakerNameText == null || speakerImage == null || background == null)
        {
            Debug.LogError("dialogueText、speakerNameText、speakerImage 或 background 未分配，请在 Inspector 中分配相应的组件");
            return;
        }

        // 检查是否分配了跳过按钮
        if (skipButton == null)
        {
            Debug.LogError("skipButton 未分配，请在 Inspector 中分配跳过按钮");
            return;
        }

        // 检查是否分配了历史记录组件
        if (historyText == null)
        {
            Debug.LogError("historyText 未分配，请在 Inspector 中分配历史记录组件");
            return;
        }

        // 检查是否分配了 logButton
        if (logButton == null)
        {
            Debug.LogError("logButton 未分配，请在 Inspector 中分配 logButton");
            return;
        }

        if (logObject == null)
        {
            Debug.LogError("logObject 未分配，请在 Inspector 中分配相应的 GameObject");
            return;
        }

        // 隐藏历史记录组件
        historyPanel.gameObject.SetActive(false);

        // 给跳过按钮添加点击事件监听
        skipButton.onClick.AddListener(OnSkipButtonClicked);

        // 给 logButton 添加点击事件监听
        logButton.onClick.AddListener(OnLogButtonClicked);
        logButton.interactable = false; // 禁用 logButton

        historyButton.onClick.AddListener(ToggleHistory);

        // 加载图像资源
        speakerImages["Wata"] = Resources.Load<Sprite>("Characters/Watanabe");
        speakerImages["X"] = Resources.Load<Sprite>("Characters/Xue");
        speakerImages["Tai"] = Resources.Load<Sprite>("Characters/Taira");
        // 可以根据需要添加更多说话人的图像

        speakerImage.enabled = false; // 默认情况下不显示说话人图像

        // 根据GameStateManager.lastSelectedIndex读取相应的txt文件
        int lastSelectedIndex = GameStateManager.lastSelectedIndex;
        if (lastSelectedIndex >= 0 && lastSelectedIndex < fileNames.Length)
        {
            string fileName = fileNames[lastSelectedIndex];
            dialogueLines = ReadTextFromResources("Scenarios/" + fileName, separator);
        }
        else
        {
            Debug.LogError("无效的 lastSelectedIndex: " + lastSelectedIndex);
            return;
        }

        if (dialogueLines.Count > 0)
        {
            StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
        }
        battleStart.gameObject.SetActive(false);
        continueGame.gameObject.SetActive(false); // 初始化 continueGame 为 false

        // 给历史记录面板添加点击事件监听
        historyPanel.GetComponent<Button>().onClick.AddListener(HideHistoryPanel);

        // 获取GameStateManager实例
        gameStateManager = GameStateManager.Instance;
    }

    void Update()
    {
        // 检查是否按下空格键并且没有正在打字且未查看历史记录
        if (!isViewingHistory && (Input.GetKeyDown(KeyCode.Space)) && !isTyping)
        {
            if (logSound != null)
            {
                PlaySoundEffect(logSound);
            }
            AdvanceDialogue();
        }

        // 检查是否按下 H 键并切换历史记录的显示
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleHistory();
        }

        // 检查是否在查看历史记录时滚动鼠标滚轮
        if (isViewingHistory && Input.mouseScrollDelta.y != 0)
        {
            ScrollHistory((int)-Input.mouseScrollDelta.y);
        }
    }

    private void AdvanceDialogue()
    {
        int lastSelectedIndex = GameStateManager.lastSelectedIndex;
        currentLineIndex++;
        if (currentLineIndex < dialogueLines.Count)
        {
            StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
        }
        else
        {
            if (shouldStartBattle)
            {
                battleStart.gameObject.SetActive(true);
            }
            else
            {
                continueGame.gameObject.SetActive(true);
                gameStateManager.isStageCleared[lastSelectedIndex] = true;
            }
            logObject.SetActive(false);
        }
    }

    IEnumerator TypeText(DialogueLine dialogueLine)
    {
        isTyping = true;
        logButton.interactable = false; // 打字开始时禁用 logButton
        dialogueText.text = "";

        if (!skip)
        {
            // 切换背景
            if (!string.IsNullOrEmpty(dialogueLine.Background) && dialogueLine.Background != lastBackground)
            {
                Sprite newBackground = Resources.Load<Sprite>("Backgrounds/" + dialogueLine.Background);
                if (newBackground != null)
                {
                    background.sprite = newBackground;
                    background.enabled = true;
                    Debug.Log("背景切换为: " + dialogueLine.Background); // 添加调试信息
                    lastBackground = dialogueLine.Background;
                }
                else
                {
                    Debug.LogError("背景资源不存在: " + dialogueLine.Background);
                }
            }

            // 播放效果音
            if (!string.IsNullOrEmpty(dialogueLine.SoundEffect))
            {
                AudioClip soundEffect = Resources.Load<AudioClip>("SoundEffects/" + dialogueLine.SoundEffect);
                if (soundEffect != null)
                {
                    PlaySoundEffect(soundEffect); // 播放效果音
                    Debug.Log("播放效果音: " + dialogueLine.SoundEffect); // 添加调试信息
                    lastSoundEffect = dialogueLine.SoundEffect; // 更新最后播放的效果音
                }
                else
                {
                    Debug.LogError("效果音资源不存在: " + dialogueLine.SoundEffect);
                }
            }

            // 播放BGM
            if (!string.IsNullOrEmpty(dialogueLine.Bgm) && dialogueLine.Bgm != lastBgm)
            {
                AudioClip bgm = Resources.Load<AudioClip>("BGM/" + dialogueLine.Bgm);
                if (bgm != null)
                {
                    PlayBGM(bgm); // 播放BGM
                    Debug.Log("播放BGM: " + dialogueLine.Bgm); // 添加调试信息
                    lastBgm = dialogueLine.Bgm;
                }
                else
                {
                    Debug.LogError("BGM资源不存在: " + dialogueLine.Bgm);
                }
            }
        }

        if (dialogueLine.Speaker == "NPC")
        {
            speakerNameText.text = "";
            speakerImage.enabled = false;
            dialogueText.color = speakerColors["NPC"]; // 设置旁白颜色
        }
        else
        {
            speakerNameText.text = dialogueLine.Speaker;
            dialogueText.color = Color.white;

            // 设置说话人姓名的颜色
            if (speakerColors.TryGetValue(dialogueLine.Speaker, out Color speakerColor))
            {
                speakerNameText.color = speakerColor;
            }

            // 设置说话人图像
            if (speakerImages.TryGetValue(dialogueLine.Speaker, out Sprite speakerSprite))
            {
                speakerImage.sprite = speakerSprite;
                speakerImage.enabled = true;
            }
            else
            {
                speakerImage.enabled = false;
            }
        }

        System.Random random = new System.Random();

        foreach (char letter in dialogueLine.Text.ToCharArray())
        {
            if (skip)
            {
                dialogueText.text = dialogueLine.Text;
                break;
            }
            dialogueText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        // 添加当前行到历史记录
        dialogueHistory.Add(dialogueLine.Speaker + ": " + dialogueLine.Text);

        isTyping = false;
        logButton.interactable = true; // 打字结束时启用 logButton

        // 如果处于跳过模式，则自动跳到下一行
        if (skip)
        {
            AdvanceDialogue();
        }
    }

    private void OnSkipButtonClicked()
    {
        if (!isViewingHistory)
        {
            int lastSelectedIndex = GameStateManager.lastSelectedIndex;
            skip = true;
            StopAllCoroutines(); // 停止当前正在进行的打字效果
            
            // 隐藏指定的 GameObject
            if (logObject != null)
            {
                logObject.SetActive(false);
            }

            if (shouldStartBattle)
            {
                battleStart.gameObject.SetActive(true); // 显示 battleStart 对象
            }
            else
            {
                continueGame.gameObject.SetActive(true); // 显示 continueGame 对象
                gameStateManager.isStageCleared[lastSelectedIndex] = true;
            }

            if (currentLineIndex < dialogueLines.Count)
            {
                StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
            }
        }
    }

    private void OnLogButtonClicked()
    {
        if (logSound != null)
        {
            PlaySoundEffect(logSound);
        }
        AdvanceDialogue();
    }

    private void ToggleHistory()
    {
        if (historyPanel.gameObject.activeSelf)
        {
            historyPanel.gameObject.SetActive(false);
            isViewingHistory = false;
        }
        else
        {
            historyStartIndex = Mathf.Max(0, dialogueHistory.Count - historyViewCount); // 设置开始显示的位置为最新的6行
            UpdateHistoryView();
            historyPanel.gameObject.SetActive(true);
            isViewingHistory = true;
        }
    }

    private void ScrollHistory(int direction)
    {
        historyStartIndex += direction; // 正值向下滚动，负值向上滚动
        historyStartIndex = Mathf.Clamp(historyStartIndex, 0, Mathf.Max(0, dialogueHistory.Count - historyViewCount));
        UpdateHistoryView();
    }

    private void UpdateHistoryView()
    {
        int endIndex = Mathf.Min(historyStartIndex + historyViewCount, dialogueHistory.Count);
        historyText.text = string.Join("\n", dialogueHistory.GetRange(historyStartIndex, endIndex - historyStartIndex));
    }

    private void HideHistoryPanel()
    {
        historyPanel.gameObject.SetActive(false);
        isViewingHistory = false;
    }

    private List<DialogueLine> ReadTextFromResources(string path, string separator)
    {
        List<DialogueLine> lines = new List<DialogueLine>();
        string currentBackground = null;
        string currentSoundEffect = null; // 新增的效果音变量
        string currentBgm = null; // 新增的BGM变量

        try
        {
            // 加载 Resources 文件夹中的文本文件
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            if (textAsset != null)
            {
                string content = textAsset.text;
                string[] rawLines = content.Split(new string[] { separator }, System.StringSplitOptions.None);
                foreach (string rawLine in rawLines)
                {
                    if (rawLine.StartsWith("*"))
                    {
                        // 添加背景
                        currentBackground = rawLine.Trim('*').Trim();
                        Debug.Log("背景添加: " + currentBackground); // 添加调试信息
                    }
                    else if (rawLine.StartsWith("#"))
                    {
                        // 添加效果音
                        currentSoundEffect = rawLine.Trim('#').Trim();
                        Debug.Log("效果音添加: " + currentSoundEffect); // 添加调试信息
                    }
                    else if (rawLine.StartsWith("@"))
                    {
                        // 添加BGM
                        currentBgm = rawLine.Trim('@').Trim();
                        Debug.Log("BGM添加: " + currentBgm); // 添加调试信息
                    }
                    else
                    {
                        string[] parts = rawLine.Split(new char[] { ':' }, 2); // 修改为2部分
                        if (parts.Length == 2)
                        {
                            string speaker = parts[0].Trim();
                            string text = parts[1].Trim();
                            lines.Add(new DialogueLine(speaker, text, currentBackground, currentSoundEffect, currentBgm));
                            currentSoundEffect = null; // 重置效果音变量
                            currentBgm = null; // 重置BGM变量
                        }
                        else
                        {
                            Debug.LogError("无法解析行: " + rawLine);
                        }
                    }
                }
                // 检查最后一行是否是 "$Battle"
                if (rawLines.Length > 0 && rawLines[rawLines.Length - 1].Trim() == "$Battle")
                {
                    shouldStartBattle = true;
                }
            }
            else
            {
                Debug.LogError("文件不存在: " + path);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("读取文件时发生错误: " + e.Message);
        }
        return lines;
    }

    // 定义用于存储对话行的类
    private class DialogueLine
    {
        public string Speaker { get; private set; }
        public string Text { get; private set; }
        public string Background { get; private set; }
        public string SoundEffect { get; private set; }
        public string Bgm { get; private set; } // 新增的BGM属性

        public DialogueLine(string speaker, string text, string background, string soundEffect, string bgm)
        {
            Speaker = speaker;
            Text = text;
            Background = background;
            SoundEffect = soundEffect;
            Bgm = bgm;
        }
    }

    private void PlaySoundEffect(AudioClip clip)
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.PlaySoundEffect(clip);
        }
        else if (backupAudioSource != null)
        {
            backupAudioSource.PlayOneShot(clip);
        }
    }

    private void PlayBGM(AudioClip bgm)
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.PlayBGM(bgm);
        }
        else if (backupAudioSource != null)
        {
            backupAudioSource.clip = bgm;
            backupAudioSource.Play();
        }
    }
}
