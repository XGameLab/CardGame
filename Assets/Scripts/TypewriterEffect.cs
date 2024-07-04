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
    public string fileName = "dialogue"; // 公共变量用于指定要读取的文本文件名（无扩展名）
    private string separator = "\n"; // 指定的分隔符

    private List<DialogueLine> dialogueLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;

    // 定义说话人颜色和图像的字典
    private Dictionary<string, Color> speakerColors = new Dictionary<string, Color>
    {
        { "渡辺", new Color(255 / 255f, 215 / 255f, 0 / 255f) },
        { "薛", new Color(127 / 255f, 255 / 255f, 212 / 255f) }, // rgb(127, 255, 212)
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

        // 加载图像资源
        speakerImages["渡辺"] = Resources.Load<Sprite>("Watanabe");
        speakerImages["薛"] = Resources.Load<Sprite>("Xue");
        // 可以根据需要添加更多说话人的图像

        speakerImage.enabled = false; // 默认情况下不显示说话人图像

        // 从txt文件中读取内容并按分隔符分割
        dialogueLines = ReadTextFromResources(fileName, separator);
        if (dialogueLines.Count > 0)
        {
            StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
        }
    }

    void Update()
    {
        // 检查是否按下空格键并且没有正在打字
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !isTyping)
        {
            currentLineIndex++;
            if (currentLineIndex < dialogueLines.Count)
            {
                StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
            }
            else
            {
                dialogueText.text = "THE END.";
                dialogueText.color = Color.black; 
                speakerNameText.text = "";
                speakerImage.sprite = null;
                speakerImage.enabled = false;
            }
        }
    }

    IEnumerator TypeText(DialogueLine dialogueLine)
    {
        isTyping = true;
        dialogueText.text = "";

        // 切换背景
        if (!string.IsNullOrEmpty(dialogueLine.Background))
        {
            Sprite newBackground = Resources.Load<Sprite>(dialogueLine.Background);
            if (newBackground != null)
            {
                background.sprite = newBackground;
                background.enabled = true;
                Debug.Log("背景切换为: " + dialogueLine.Background); // 添加调试信息
            }
            else
            {
                Debug.LogError("背景资源不存在: " + dialogueLine.Background);
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

        foreach (char letter in dialogueLine.Text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    private List<DialogueLine> ReadTextFromResources(string path, string separator)
    {
        List<DialogueLine> lines = new List<DialogueLine>();
        string currentBackground = null;
        
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
                        // 更新背景
                        currentBackground = rawLine.Trim('*').Trim();
                        Debug.Log("背景更新为: " + currentBackground); // 添加调试信息
                    }
                    else
                    {
                        string[] parts = rawLine.Split(new char[] { ':' }, 2); // 修改为2部分
                        if (parts.Length == 2)
                        {
                            string speaker = parts[0].Trim();
                            string text = parts[1].Trim();
                            lines.Add(new DialogueLine(speaker, text, currentBackground));
                        }
                        else
                        {
                            Debug.LogError("无法解析行: " + rawLine);
                        }
                    }
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

        public DialogueLine(string speaker, string text, string background)
        {
            Speaker = speaker;
            Text = text;
            Background = background;
        }
    }
}
