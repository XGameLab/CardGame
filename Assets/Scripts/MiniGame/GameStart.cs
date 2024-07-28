using UnityEngine;
using TMPro;

public class GameStart : MonoBehaviour
{
    public TextMeshProUGUI gameModeText; // 用于显示游戏模式的文本
    public RectTransform[] cardPos; // 使用 RectTransform 来定义卡牌生成的位置
    public GameObject[] cardPrefabs; // 卡牌预制体数组

    private int maxPerPrefab = 4; // 每种卡牌最大生成数量
    private int[] prefabCounts; // 跟踪每种Prefab的生成次数

    void Start()
    {
        prefabCounts = new int[cardPrefabs.Length];
        ResetGame();
    }

    void OnEnable()
    {
        ChooseGameMode.OnGameOffline += OfflineGame;
        ChooseGameMode.OnGameOnline += OnlineGame;
    }

    void OnDisable()
    {
        ChooseGameMode.OnGameOffline -= OfflineGame;
        ChooseGameMode.OnGameOnline -= OnlineGame;
    }

    public void OfflineGame()
    {
        gameModeText.text = "オフライン モード";
        SpawnCards();
    }

    public void OnlineGame()
    {
        gameModeText.text = "オンライン モード";
        SpawnCards();
    }

    private void SpawnCards()
    {
        if (cardPrefabs.Length != 5)
        {
            Debug.LogError("Prefab array must contain exactly 5 elements.");
            return;
        }

        if (cardPos.Length != 20)
        {
            Debug.LogError("Spawn points array must contain exactly 20 elements.");
            return;
        }

        // 为每个位置生成4个随机Prefab
        foreach (var pos in cardPos)
        {
            bool cardSpawned = false;

            // 尝试找到一个未达到最大限制的Prefab进行生成
            while (!cardSpawned)
            {
                int randomIndex = Random.Range(0, cardPrefabs.Length);

                if (prefabCounts[randomIndex] < maxPerPrefab)
                {
                    // 生成卡牌
                    GameObject card = Instantiate(cardPrefabs[randomIndex], pos);
                    card.SetActive(true);

                    // 调整卡牌的UI位置
                    RectTransform cardRectTransform = card.GetComponent<RectTransform>();
                    if (cardRectTransform != null)
                    {
                        cardRectTransform.anchoredPosition = Vector2.zero;
                    }

                    // 增加该Prefab的生成计数
                    prefabCounts[randomIndex]++;
                    cardSpawned = true;
                }
            }
        }
    }

    private void ResetGame()
    {
        // 重置每种Prefab的计数
        for (int i = 0; i < prefabCounts.Length; i++)
        {
            prefabCounts[i] = 0;
        }
    }
}