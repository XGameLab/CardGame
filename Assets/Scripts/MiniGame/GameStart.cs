using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameStart : MonoBehaviourPunCallbacks
{
    public RectTransform[] cardPos;
    public GameObject[] cardPrefabs;

    private int maxPerPrefab = 4;
    private int[] prefabCounts;

    [SerializeField]
    private int seed; // 随机种子，公开到Inspector

    private const string SeedProperty = "GameSeed";
    private bool isGameStarted = false;

    void Start()
    {
        prefabCounts = new int[cardPrefabs.Length];
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnEnable()
    {
        ScoreManager.OnRetryGame += RetrySpawnCards;
        ChooseGameMode.OnGameOffline += StartOfflineGame;
        ChooseGameMode.OnGameOnline += StartOnlineGame;
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        ScoreManager.OnRetryGame -= RetrySpawnCards;
        ChooseGameMode.OnGameOffline -= StartOfflineGame;
        ChooseGameMode.OnGameOnline -= StartOnlineGame;
    }

    private void StartOfflineGame()
    {
        isGameStarted = true;
        GenerateSeedForOfflineMode();
        SpawnCards();
    }

    private void StartOnlineGame()
    {
        isGameStarted = true;
        // 联机模式的处理逻辑将在OnJoinedRoom中完成
    }

    private void GenerateAndSetSeed()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);
        Debug.Log("Host Seed: " + seed);

        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable
        {
            { SeedProperty, seed }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }

    private void GenerateSeedForOfflineMode()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);
        Debug.Log("Offline Mode Seed: " + seed);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GenerateAndSetSeed();
            if (isGameStarted)
            {
                SpawnCards();
            }
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(SeedProperty, out object seedValue))
            {
                seed = (int)seedValue;
                Debug.Log("Client received Seed: " + seed);
                if (isGameStarted)
                {
                    SpawnCards();
                }
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(SeedProperty))
        {
            seed = (int)propertiesThatChanged[SeedProperty];
            Debug.Log("Updated Seed from properties: " + seed);
            if (isGameStarted)
            {
                SpawnCards();
            }
        }
    }

    private void SpawnCards()
    {
        // 清理旧的卡牌
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

        Random.InitState(seed);
        Debug.Log("Using Seed: " + seed);

        foreach (var pos in cardPos)
        {
            bool cardSpawned = false;

            while (!cardSpawned)
            {
                int randomIndex = Random.Range(0, cardPrefabs.Length);

                if (prefabCounts[randomIndex] < maxPerPrefab)
                {
                    // 清理位置上已有的卡牌
                    ClearPreviousCard(pos);

                    // 使用PhotonNetwork.Instantiate在网络上创建卡牌
                    GameObject card;
                    if (PhotonNetwork.IsConnected)
                    {
                        card = PhotonNetwork.Instantiate(cardPrefabs[randomIndex].name, pos.position, Quaternion.identity);
                    }
                    else
                    {
                        card = Instantiate(cardPrefabs[randomIndex], pos.position, Quaternion.identity);
                    }
                    card.transform.SetParent(pos, false);

                    prefabCounts[randomIndex]++;
                    cardSpawned = true;
                }
            }
        }
    }

    private void ClearPreviousCard(RectTransform position)
    {
        foreach (Transform child in position)
        {
            if (child.gameObject.GetComponent<PhotonView>() != null)
            {
                PhotonNetwork.Destroy(child.gameObject);
            }
            else
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void ResetGame()
    {
        for (int i = 0; i < prefabCounts.Length; i++)
        {
            prefabCounts[i] = 0;
        }
    }

    private void RetrySpawnCards()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            GenerateAndSetSeed();
        }
        else if (!PhotonNetwork.IsConnected)
        {
            GenerateSeedForOfflineMode();
        }

        SpawnCards();
    }

    private void OnValidate()
    {
        if (Application.isPlaying && isGameStarted)
        {
            SpawnCards();
        }
    }
}