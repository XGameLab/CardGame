using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro を使用
using UnityEngine.UI; // UI を使用
using System.IO;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // TextMeshProUGUI を使用したダイアログテキスト
    public TextMeshProUGUI speakerNameText; // 話者の名前を表示する TextMeshProUGUI
    public Image speakerImage; // 話者の画像を表示するための Image コンポーネント
    public Image background; // 背景を表示するための Image コンポーネント
    public float typingSpeed = 0.05f;
    public GameObject battleStart; // 戦闘開始の GameObject
    public GameObject continueGame; // ゲーム継続の GameObject
    public GameObject playerChoose; // プレイヤー選択の GameObject
    public GameObject gameEnd; // ゲーム終了の GameObject
    public GameObject logObject; // ログの GameObject
    public Button[] historyButton; // 履歴ボタン
    public Button skipButton; // スキップボタン
    public Button logButton; // ログボタン
    public Button playerChoose1; // プレイヤー選択ボタン1
    public Button playerChoose2; // プレイヤー選択ボタン2
    public GameObject historyPanel; // 履歴パネル
    public TextMeshProUGUI historyText; // 履歴テキストを表示するための TextMeshProUGUI
    public string[] fileNames; // テキストファイル名の配列（拡張子なし）

    private string separator = "\n"; // 分割文字
    public AudioSource backupAudioSource; // 予備のAudioSource
    public AudioClip logSound; // ログ音声

    private List<DialogueLine> dialogueLines; // ダイアログ行のリスト
    private List<string> dialogueHistory = new List<string>(); // ダイアログ履歴のリスト
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool skip = false; // スキップ用フラグ
    private bool isViewingHistory = false; // 履歴表示中フラグ
    private int historyStartIndex = 0; // 履歴表示の開始インデックス
    private int historyViewCount = 6; // 履歴表示の行数
    private string lastBackground = "";
    private string lastSoundEffect = "";
    private string lastBgm = "";
    private bool shouldStartBattle = false; // 戦闘開始フラグ
    private bool shouldPlayerChoose = false; // プレイヤー選択フラグ
    private bool isGameEnd = false; // ゲーム終了フラグ
    private GameStateManager gameStateManager;

    // 話者の色と画像の辞書を定義
    private Dictionary<string, Color> speakerColors = new Dictionary<string, Color>
    {
        { "Wata", new Color(255 / 255f, 215 / 255f, 0 / 255f) },
        { "DarkWata", new Color(0 / 255f, 0 / 255f, 0 / 255f) },
        { "XP", new Color(128 / 255f, 0 / 255f, 43 / 255f) },
        { "X", new Color(127 / 255f, 255 / 255f, 212 / 255f) },
        { "Tai", new Color(135 / 255f, 246 / 255f, 0 / 255f) },
        { "NPC", Color.green }, // ナレーションの色
    };

    private Dictionary<string, Sprite> speakerImages = new Dictionary<string, Sprite>();

    void Start()
    {
        // TextMeshPro コンポーネントの割り当てを確認
        if (dialogueText == null || speakerNameText == null || speakerImage == null || background == null)
        {
            Debug.LogError("dialogueText、speakerNameText、speakerImage、または background が割り当てられていません。Inspector で割り当ててください");
            return;
        }

        // スキップボタンの割り当てを確認
        if (skipButton == null)
        {
            Debug.LogError("skipButton が割り当てられていません。Inspector で割り当ててください");
            return;
        }

        // 履歴コンポーネントの割り当てを確認
        if (historyText == null)
        {
            Debug.LogError("historyText が割り当てられていません。Inspector で割り当ててください");
            return;
        }

        // logButtonの割り当てを確認
        if (logButton == null)
        {
            Debug.LogError("logButton が割り当てられていません。Inspector で割り当ててください");
            return;
        }

        if (logObject == null)
        {
            Debug.LogError("logObject が割り当てられていません。Inspector で割り当ててください");
            return;
        }

        // 履歴パネルを非表示にする
        historyPanel.gameObject.SetActive(false);

        // スキップボタンにクリックイベントを追加
        skipButton.onClick.AddListener(OnSkipButtonClicked);

        // logButton にクリックイベントを追加
        logButton.onClick.AddListener(OnLogButtonClicked);
        logButton.interactable = false; // logButton を無効化

        for (int i = 0; i < 2; i++)
        {
            historyButton[i].onClick.AddListener(ToggleHistory);
        }
        playerChoose1.onClick.AddListener(OnPlayerChooseButtonClicked);
        playerChoose2.onClick.AddListener(OnPlayerChooseButtonClicked);

        // 画像リソースを読み込む
        speakerImages["Wata"] = Resources.Load<Sprite>("Characters/Wata");
        speakerImages["DarkWata"] = Resources.Load<Sprite>("Characters/DarkWata");
        speakerImages["XP"] = Resources.Load<Sprite>("Characters/XP");
        speakerImages["X"] = Resources.Load<Sprite>("Characters/X");
        speakerImages["Tai"] = Resources.Load<Sprite>("Characters/Tai");
        // 他の話者の画像を必要に応じて追加可能

        speakerImage.enabled = false; // デフォルトでは話者の画像を表示しない

        // GameStateManager.lastSelectedIndex に基づいて適切なテキストファイルを読み込む
        int lastSelectedIndex = GameStateManager.lastSelectedIndex;
        if (lastSelectedIndex >= 0 && lastSelectedIndex < fileNames.Length)
        {
            string fileName = fileNames[lastSelectedIndex];
            dialogueLines = ReadTextFromResources("Scenarios/" + fileName, separator);
        }
        else
        {
            Debug.LogError("無効な lastSelectedIndex: " + lastSelectedIndex);
            return;
        }

        if (dialogueLines.Count > 0)
        {
            StartCoroutine(TypeText(dialogueLines[currentLineIndex]));
        }
        battleStart.gameObject.SetActive(false);
        continueGame.gameObject.SetActive(false); // continueGame を false に初期化

        // 履歴パネルにクリックイベントを追加
        historyPanel.GetComponent<Button>().onClick.AddListener(HideHistoryPanel);

        // GameStateManagerのインスタンスを取得
        gameStateManager = GameStateManager.Instance;
    }

    void Update()
    {
        // 履歴を表示していないときにスペースキーが押され、タイピング中でない場合
        if (!isViewingHistory && (Input.GetKeyDown(KeyCode.Space)) && !isTyping)
        {
            if (logSound != null)
            {
                PlaySoundEffect(logSound);
            }
            AdvanceDialogue();
        }

        // Hキーが押されたときに履歴表示を切り替える
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleHistory();
        }

        // 履歴表示中にマウスホイールをスクロールするかどうかを確認
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
            if (logObject != null)
            {
                logObject.SetActive(false);
            }

            if (shouldStartBattle)
            {
                battleStart.gameObject.SetActive(true);
            }
            else if (shouldPlayerChoose)
            {
                playerChoose.gameObject.SetActive(true);
            }
            else if (isGameEnd)
            {
                gameEnd.gameObject.SetActive(true);
            }
            else
            {
                continueGame.gameObject.SetActive(true);
                gameStateManager.isStageCleared[lastSelectedIndex] = true; 
            }
        }
    }

    IEnumerator TypeText(DialogueLine dialogueLine)
    {
        isTyping = true;
        logButton.interactable = false; // タイピング開始時にlogButtonを無効化
        dialogueText.text = "";

        if (!skip)
        {
            // 背景を切り替える
            if (!string.IsNullOrEmpty(dialogueLine.Background) && dialogueLine.Background != lastBackground)
            {
                Sprite newBackground = Resources.Load<Sprite>("Backgrounds/" + dialogueLine.Background);
                if (newBackground != null)
                {
                    background.sprite = newBackground;
                    background.enabled = true;
                    Debug.Log("背景を切り替え: " + dialogueLine.Background); // デバッグ情報を追加
                    lastBackground = dialogueLine.Background;
                }
                else
                {
                    Debug.LogError("背景リソースが存在しません: " + dialogueLine.Background);
                }
            }

            // 効果音を再生する
            if (!string.IsNullOrEmpty(dialogueLine.SoundEffect))
            {
                AudioClip soundEffect = Resources.Load<AudioClip>("SoundEffects/" + dialogueLine.SoundEffect);
                if (soundEffect != null)
                {
                    PlaySoundEffect(soundEffect); // 効果音を再生
                    Debug.Log("効果音を再生: " + dialogueLine.SoundEffect); // デバッグ情報を追加
                    lastSoundEffect = dialogueLine.SoundEffect; // 最後に再生された効果音を更新
                }
                else
                {
                    Debug.LogError("効果音リソースが存在しません: " + dialogueLine.SoundEffect);
                }
            }

            // BGMを再生する
            if (!string.IsNullOrEmpty(dialogueLine.Bgm) && dialogueLine.Bgm != lastBgm)
            {
                AudioClip bgm = Resources.Load<AudioClip>("BGM/" + dialogueLine.Bgm);
                if (bgm != null)
                {
                    PlayBGM(bgm); // BGMを再生
                    Debug.Log("BGMを再生: " + dialogueLine.Bgm); // デバッグ情報を追加
                    lastBgm = dialogueLine.Bgm;
                }
                else
                {
                    Debug.LogError("BGMリソースが存在しません: " + dialogueLine.Bgm);
                }
            }
        }

        if (dialogueLine.Speaker == "NPC")
        {
            speakerNameText.text = "";
            speakerImage.enabled = false;
            dialogueText.color = speakerColors["NPC"]; // ナレーションの色を設定
        }
        else
        {
            speakerNameText.text = dialogueLine.Speaker;
            dialogueText.color = Color.white;

            // 話者の名前の色を設定
            if (speakerColors.TryGetValue(dialogueLine.Speaker, out Color speakerColor))
            {
                speakerNameText.color = speakerColor;
            }

            // 話者の画像を設定
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

        // 現在の行を履歴に追加
        dialogueHistory.Add(dialogueLine.Speaker + ": " + dialogueLine.Text);

        isTyping = false;
        logButton.interactable = true; // タイピング終了時にlogButtonを有効化

        // スキップモードの場合、自動的に次の行に進む
        if (skip)
        {
            AdvanceDialogue();
        }
    }

    private void OnSkipButtonClicked()
    {
        if (!isViewingHistory)
        {
            skip = true;
            StopAllCoroutines(); // 現在実行中のタイピング効果を停止

            // 残りの全てのダイアログ行を履歴に追加
            for (int i = currentLineIndex; i < dialogueLines.Count; i++)
            {
                if (i >= dialogueHistory.Count) // 重複しないように確認
                {
                    dialogueHistory.Add(dialogueLines[i].Speaker + ": " + dialogueLines[i].Text);
                }            
            }

            // 最後のダイアログ行のインデックスを対話終了状態に設定
            currentLineIndex = dialogueLines.Count;

            // 指定の GameObject を非表示にする
            if (logObject != null)
            {
                logObject.SetActive(false);
            }

            // ゲームのロジックに従って適切なUIコンポーネントを表示
            if (shouldStartBattle)
            {
                battleStart.gameObject.SetActive(true);
            }
            else if(shouldPlayerChoose)
            {
                playerChoose.gameObject.SetActive(true);
            }
            else if(isGameEnd)
            {
                gameEnd.gameObject.SetActive(true);
                gameStateManager.isStageCleared[GameStateManager.lastSelectedIndex] = true;
            }
            else
            {
                continueGame.gameObject.SetActive(true);
                gameStateManager.isStageCleared[GameStateManager.lastSelectedIndex] = true;
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

    private void OnPlayerChooseButtonClicked()
    {
        int lastSelectedIndex = GameStateManager.lastSelectedIndex;
        gameStateManager.isStageCleared[lastSelectedIndex] = true;
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
            historyStartIndex = Mathf.Max(0, dialogueHistory.Count - historyViewCount); // 最新の6行を表示
            UpdateHistoryView();
            historyPanel.gameObject.SetActive(true);
            isViewingHistory = true;
        }
    }

    private void ScrollHistory(int direction)
    {
        historyStartIndex += direction; // 正の値は下にスクロール、負の値は上にスクロール
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
        string currentSoundEffect = null;
        string currentBgm = null;

        try
        {
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            if (textAsset != null)
            {
                string content = textAsset.text;
                string[] rawLines = content.Split(new string[] { separator }, System.StringSplitOptions.None);
                foreach (string rawLine in rawLines)
                {
                    if (rawLine.StartsWith("*"))
                    {
                        currentBackground = rawLine.Trim('*').Trim();
                    }
                    else if (rawLine.StartsWith("#"))
                    {
                        currentSoundEffect = rawLine.Trim('#').Trim();
                    }
                    else if (rawLine.StartsWith("@"))
                    {
                        currentBgm = rawLine.Trim('@').Trim();
                    }
                    else if (rawLine.Trim() == "$Battle")
                    {
                        shouldStartBattle = true;
                    }
                    else if (rawLine.Trim() == "$PlayerChoose")
                    {
                        shouldPlayerChoose = true;
                    }
                    else if (rawLine.Trim() == "$GameEnd")
                    {
                        isGameEnd = true;
                    }
                    else
                    {
                        string[] parts = rawLine.Split(new char[] { ':' }, 2);
                        if (parts.Length == 2)
                        {
                            string speaker = parts[0].Trim();
                            string text = parts[1].Trim();
                            lines.Add(new DialogueLine(speaker, text, currentBackground, currentSoundEffect, currentBgm));
                            currentSoundEffect = null;
                            currentBgm = null;
                        }
                        else
                        {
                            Debug.LogError("行を解析できません: " + rawLine);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("ファイルが存在しません: " + path);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("ファイルの読み取り中にエラーが発生しました: " + e.Message);
        }
        return lines;
    }

    // ダイアログ行を保存するためのクラスを定義
    private class DialogueLine
    {
        public string Speaker { get; private set; }
        public string Text { get; private set; }
        public string Background { get; private set; }
        public string SoundEffect { get; private set; }
        public string Bgm { get; private set; } // BGMプロパティを追加

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
