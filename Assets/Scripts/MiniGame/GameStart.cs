using UnityEngine;
using TMPro;

public class GameStart : MonoBehaviour
{
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
        ScoreManager.OnRetryGame += RetrySpawnCards; // 监听重置事件
    }

    void OnDisable()
    {
        ChooseGameMode.OnGameOffline -= OfflineGame;
        ChooseGameMode.OnGameOnline -= OnlineGame;
        ScoreManager.OnRetryGame -= RetrySpawnCards; // 取消监听重置事件
    }

    public void OfflineGame()
    {
        SpawnCards();
    }

    public void OnlineGame()
    {
        SpawnCards();
    }

    private void SpawnCards()
    {
        ResetGame();

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

        foreach (var pos in cardPos)
        {
            bool cardSpawned = false;

            while (!cardSpawned)
            {
                int randomIndex = Random.Range(0, cardPrefabs.Length);

                if (prefabCounts[randomIndex] < maxPerPrefab)
                {
                    GameObject card = Instantiate(cardPrefabs[randomIndex], pos);
                    card.SetActive(true);

                    RectTransform cardRectTransform = card.GetComponent<RectTransform>();
                    if (cardRectTransform != null)
                    {
                        cardRectTransform.anchoredPosition = Vector2.zero;
                    }

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

    private void RetrySpawnCards()
    {
        // 在游戏重置时重新发牌
        SpawnCards();
    }
}