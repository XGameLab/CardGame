using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    // シングルトンインスタンスを保持する静的プロパティ
    public static GameStateManager Instance { get; private set; }

    public bool[] isStageCleared; // 各ステージのクリア状況を保持する配列
    private AudioSource audioSource; // BGMや効果音を再生するためのAudioSource

    public AudioClip[] bgmClips; // BGMクリップを格納する配列
    public string[] sceneNames; // BGMに対応するシーン名を格納する配列

    public static int lastSelectedIndex = 0; // 最後に選択されたステージのインデックス

    public AudioSource AudioSource
    {
        get { return audioSource; }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 新しいシーンにロードしてもオブジェクトを保持
            isStageCleared = new bool[8]; // 8つのステージを仮定して初期化
            audioSource = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded; // シーンがロードされた時のイベントを登録
        }
        else
        {
            Destroy(gameObject); // シングルトンパターンを維持するために重複するオブジェクトを破棄
        }
    }

    void Start()
    {
        // 初期化時に現在のシーンのBGMをチェックして再生
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        PlayBGM(bgmClips[0]); // 手動で最初のBGMを再生
    }

    // シーンがロードされた時に呼び出されるコールバック
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name); // デバッグログ

        // シーン名に対応するBGMを探して再生
        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (scene.name == sceneNames[i])
            {
                PlayBGM(bgmClips[i]);
                break;
            }
        }
    }

    // 指定したBGMを再生するメソッド
    public void PlayBGM(AudioClip bgm)
    {
        if (audioSource.clip != bgm)
        {
            audioSource.clip = bgm;
            audioSource.Play();
            Debug.Log("Playing BGM: " + bgm.name); // デバッグログ
        }
    }

    // 効果音を再生するメソッド
    public void PlaySoundEffect(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
        Debug.Log("Playing Sound Effect: " + clip.name); // デバッグログ
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // シーンロードイベントの登録を解除
    }
}
