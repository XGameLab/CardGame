using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public bool[] isStageCleared; // 游戏状态变量
    private AudioSource audioSource;

    public AudioClip[] bgmClips; // 使用数组存储多个BGM
    public string[] sceneNames; // 对应的场景名称

    public static int lastSelectedIndex = 0;

    public AudioSource AudioSource
    {
        get { return audioSource; }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保持对象在加载新场景时不被销毁
            isStageCleared = new bool[8]; // 初始化数组，假设长度为8
            audioSource = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded; // 注册场景加载事件
        }
        else
        {
            Destroy(gameObject); // 保证只存在一个实例
        }
    }

    void Start()
    {
        // 初始化时检查并播放当前场景的BGM
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        PlayBGM(bgmClips[0]); // 手动触发播放第一个BGM
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name); // 调试日志

        // 查找当前场景对应的BGM
        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (scene.name == sceneNames[i])
            {
                PlayBGM(bgmClips[i]);
                break;
            }
        }
    }

    public void PlayBGM(AudioClip bgm)
    {
        if (audioSource.clip != bgm)
        {
            audioSource.clip = bgm;
            audioSource.Play();
            Debug.Log("Playing BGM: " + bgm.name); // 调试日志
        }
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
        Debug.Log("Playing Sound Effect: " + clip.name); // 调试日志
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 取消注册场景加载事件
    }
}
